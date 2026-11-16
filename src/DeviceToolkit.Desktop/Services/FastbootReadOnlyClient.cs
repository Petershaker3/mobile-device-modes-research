using System.Diagnostics;
using System.IO;
using System.Text;
using DeviceToolkit.Core.Services;

namespace DeviceToolkit.Desktop.Services;

/// <summary>Runs a fixed, bounded AOSP Fastboot read allowlist. Huawei OEM extensions are deliberately separate.</summary>
public sealed class FastbootReadOnlyClient
{
    const int MaxOutputCharacters = 64 * 1024;
    readonly Func<string?> _findExecutable;

    public FastbootReadOnlyClient() : this(AdbExecutableLocator.FindFastbootExecutable) { }

    internal FastbootReadOnlyClient(Func<string?> findExecutable)
        => _findExecutable = findExecutable ?? throw new ArgumentNullException(nameof(findExecutable));

    public async Task<IReadOnlyList<FastbootTarget>> ListDevicesAsync(CancellationToken ct)
    {
        var executable = RequireExecutable();
        var result = await RunAsync(executable, ["devices"], ct).ConfigureAwait(false);
        if (result.ExitCode != 0)
            throw new InvalidOperationException("Fastboot device discovery failed. Check Platform Tools and the USB connection.");
        return FastbootOutputParser.ParseDevices(result.Output);
    }

    public Task<FastbootDeviceFacts> ReadFactsAsync(FastbootTarget target, CancellationToken ct)
        => ReadFactsAsync(target, FastbootDeviceProfile.Android, ct);

    public async Task<FastbootDeviceFacts> ReadFactsAsync(FastbootTarget target, FastbootDeviceProfile profile, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(target);
        var variables = FastbootOutputParser.GetReadVariables(profile);
        if (string.IsNullOrWhiteSpace(target.Serial) || target.Serial.Length > 128 || target.Serial[0] == '-'
            || target.Serial.Any(character => !(char.IsAsciiLetterOrDigit(character) || character is '-' or '_' or '.' or ':' or '/')))
            throw new ArgumentException("The selected Fastboot target is invalid.", nameof(target));

        var executable = RequireExecutable();
        var oemReads = profile == FastbootDeviceProfile.Huawei
            ? FastbootOutputParser.GetHuaweiOemReadCommands()
            : Array.Empty<FastbootReadCommand>();
        var facts = new List<FastbootFact>(variables.Count + oemReads.Count);
        foreach (var variable in variables)
        {
            ct.ThrowIfCancellationRequested();
            var result = await RunAsync(executable, ["-s", target.Serial, "getvar", variable], ct).ConfigureAwait(false);
            var fact = profile == FastbootDeviceProfile.Huawei
                ? ParseHuaweiProfileVariable(variable, result.Output, result.ExitCode)
                : FastbootOutputParser.ParseVariable(variable, result.Output, result.ExitCode);
            facts.Add(fact);
        }
        foreach (var read in oemReads)
        {
            ct.ThrowIfCancellationRequested();
            var arguments = new List<string>(read.Arguments.Count + 2) { "-s", target.Serial };
            arguments.AddRange(read.Arguments);
            var result = await RunAsync(executable, arguments, ct).ConfigureAwait(false);
            var fact = FastbootOutputParser.ParseHuaweiOemRead(read.Name, result.Output, result.ExitCode);
            facts.Add(fact);
            if (read.Name == "Build version" && fact.Status == "Success")
                facts.Add(FastbootOutputParser.ParseHuaweiEquipmentModelFromBuildVersion(fact.Value));
        }
        return new FastbootDeviceFacts(FastbootOutputParser.ClassifyMode(facts), facts.AsReadOnly());
    }

    static FastbootFact ParseHuaweiProfileVariable(string name, string output, int exitCode)
        => name is "rescue_version" or "vendorcountry"
            ? FastbootOutputParser.ParseHuaweiVariable(name, output, exitCode)
            : FastbootOutputParser.ParseVariable(name, output, exitCode);

    string RequireExecutable()
        => _findExecutable() is { } path && File.Exists(path)
            ? path
            : throw new InvalidOperationException("Android Platform Tools fastboot.exe was not found. Install Platform Tools or add its folder to PATH.");

    static async Task<(string Output, int ExitCode)> RunAsync(string executable, IReadOnlyList<string> arguments, CancellationToken ct)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = executable,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };
        foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);

        try
        {
            if (!process.Start())
                throw new InvalidOperationException("Could not start Android Platform Tools fastboot.exe.");
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException("Could not start Android Platform Tools fastboot.exe.", ex);
        }

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, timeout.Token);
        var stdoutTask = ReadBoundedAndDrainAsync(process.StandardOutput, MaxOutputCharacters, linked.Token);
        var stderrTask = ReadBoundedAndDrainAsync(process.StandardError, MaxOutputCharacters, linked.Token);
        try
        {
            await process.WaitForExitAsync(linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            try { process.Kill(entireProcessTree: true); } catch { }
            try { await process.WaitForExitAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false); } catch { }
            try { await Task.WhenAll(stdoutTask, stderrTask).WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false); } catch { }
            if (ct.IsCancellationRequested) throw;
            throw new TimeoutException("Fastboot did not respond within 15 seconds. Confirm the phone is already in Fastboot mode and retry.");
        }

        var stdout = await stdoutTask.ConfigureAwait(false);
        var stderr = await stderrTask.ConfigureAwait(false);
        if (stdout.Truncated || stderr.Truncated || stdout.Text.Length + stderr.Text.Length > MaxOutputCharacters)
            throw new InvalidOperationException("Fastboot output exceeded the 64 KiB safety limit.");
        var combined = string.Join(Environment.NewLine, new[] { stdout.Text, stderr.Text }.Where(value => value.Length > 0));
        return (combined, process.ExitCode);
    }

    static async Task<(string Text, bool Truncated)> ReadBoundedAndDrainAsync(StreamReader reader, int limit, CancellationToken ct)
    {
        var buffer = new char[4096];
        var output = new StringBuilder(Math.Min(limit, 4096));
        var truncated = false;
        while (true)
        {
            var count = await reader.ReadAsync(buffer.AsMemory(), ct).ConfigureAwait(false);
            if (count == 0) break;
            var remaining = limit - output.Length;
            if (remaining > 0) output.Append(buffer, 0, Math.Min(count, remaining));
            if (count > remaining) truncated = true;
        }
        return (output.ToString(), truncated);
    }
}
