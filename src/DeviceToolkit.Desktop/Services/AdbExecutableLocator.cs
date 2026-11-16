using System.IO;
using System.Diagnostics;

namespace DeviceToolkit.Desktop.Services;

internal static class AdbExecutableLocator
{
    public static string? FindFastbootExecutable()
    {
        var candidates = new List<string>();
        AddSdkFastbootCandidate(candidates, Environment.GetEnvironmentVariable("ANDROID_HOME"));
        AddSdkFastbootCandidate(candidates, Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT"));
        AddSdkFastbootCandidate(candidates, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Android", "Sdk"));
        AddSdkFastbootCandidate(candidates, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Android", "Android Studio", "sdk"));
        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
                     .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            candidates.Add(Path.Combine(directory.Trim('"'), "fastboot.exe"));

        return candidates.Distinct(StringComparer.OrdinalIgnoreCase).FirstOrDefault(File.Exists);
    }

    static void AddSdkFastbootCandidate(List<string> candidates, string? sdk)
    {
        if (!string.IsNullOrWhiteSpace(sdk))
            candidates.Add(Path.Combine(sdk, "platform-tools", "fastboot.exe"));
    }

    public static bool EnsureAvailable()
    {
        if (CanStart("adb")) return true;

        var candidates = new List<string>();
        AddSdkCandidate(candidates, Environment.GetEnvironmentVariable("ANDROID_HOME"));
        AddSdkCandidate(candidates, Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT"));
        AddSdkCandidate(candidates, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Android", "Sdk"));

        foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!File.Exists(candidate)) continue;
            var directory = Path.GetDirectoryName(candidate)!;
            Environment.SetEnvironmentVariable("PATH", directory + Path.PathSeparator + (Environment.GetEnvironmentVariable("PATH") ?? string.Empty));
            return CanStart("adb");
        }

        return false;
    }

    static void AddSdkCandidate(List<string> candidates, string? sdk)
    {
        if (!string.IsNullOrWhiteSpace(sdk))
            candidates.Add(Path.Combine(sdk, "platform-tools", "adb.exe"));
    }

    static bool CanStart(string fileName)
    {
        try
        {
            using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = fileName,
                Arguments = "version",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            if (process is null) return false;
            if (!process.WaitForExit(3000))
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(1000);
            }
            return process.HasExited && process.ExitCode == 0;
        }
        catch { return false; }
    }
}

internal static class ApkAnalyzerLocator
{
    public static string? Find()
    {
        var roots = new[]
        {
            Environment.GetEnvironmentVariable("ANDROID_HOME"),
            Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT")
        };
        foreach (var root in roots.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var candidates = new[]
            {
                Path.Combine(root!, "cmdline-tools", "latest", "lib", "apkanalyzer-classpath.jar"),
                Path.Combine(root!, "tools", "lib", "apkanalyzer-classpath.jar")
            };
            var found = candidates.FirstOrDefault(File.Exists);
            if (found is not null) return found;
        }
        return null;
    }

    public static string? FindJavaExecutable()
    {
        var candidates = new[]
        {
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("JAVA_HOME"))
                ? null
                : Path.Combine(Environment.GetEnvironmentVariable("JAVA_HOME")!, "bin", "java.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Android", "Android Studio", "jbr", "bin", "java.exe")
        };
        var found = candidates.FirstOrDefault(candidate => candidate is not null && File.Exists(candidate));
        if (found is not null) return found;
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "java",
                Arguments = "-version",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            if (process is null) return null;
            if (!process.WaitForExit(3000)) { process.Kill(entireProcessTree: true); return null; }
            return process.ExitCode == 0 ? "java" : null;
        }
        catch { return null; }
    }
}
