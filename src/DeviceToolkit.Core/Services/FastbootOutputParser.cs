using System.Text.RegularExpressions;

namespace DeviceToolkit.Core.Services;

public sealed record FastbootTarget(string Serial)
{
    public string DisplayName => $"Fastboot device ({Serial})";
}

public sealed record FastbootFact(string Name, string Value, string Status);

public sealed record FastbootDeviceFacts(string Mode, IReadOnlyList<FastbootFact> Facts);

public enum FastbootDeviceProfile
{
    Android,
    Huawei
}

public sealed record FastbootProfileOption(FastbootDeviceProfile Profile, string DisplayName);
public sealed record FastbootReadCommand(string Name, IReadOnlyList<string> Arguments);

/// <summary>Parses only explicitly requested, non-identifying Fastboot output.</summary>
public static partial class FastbootOutputParser
{
    static readonly HashSet<string> AllowedVariables = new(StringComparer.Ordinal)
    {
        "is-userspace", "product", "current-slot", "unlocked"
    };

    static readonly HashSet<string> AllowedHuaweiVariables = new(StringComparer.Ordinal)
    {
        "rescue_version", "vendorcountry"
    };

    static readonly string[] AndroidReadVariables = ["is-userspace", "product", "current-slot", "unlocked"];
    static readonly string[] HuaweiReadVariables = ["is-userspace", "product", "current-slot", "unlocked", "rescue_version", "vendorcountry"];
    static readonly IReadOnlyList<FastbootReadCommand> HuaweiOemReadCommands = Array.AsReadOnly(new[]
    {
        new FastbootReadCommand("Product model", Array.AsReadOnly(new[] { "oem", "get-product-model" })),
        new FastbootReadCommand("Build version", Array.AsReadOnly(new[] { "oem", "get-build-number" })),
        new FastbootReadCommand("Base version", Array.AsReadOnly(new[] { "oem", "oeminforead-BASE_VERSION" })),
        new FastbootReadCommand("Custom version", Array.AsReadOnly(new[] { "oem", "oeminforead-CUSTOM_VERSION" })),
        new FastbootReadCommand("Preload version", Array.AsReadOnly(new[] { "oem", "oeminforead-PRELOAD_VERSION" })),
        new FastbootReadCommand("Android version", Array.AsReadOnly(new[] { "oem", "oeminforead-ANDROID_VERSION" })),
        new FastbootReadCommand("HWNFF version", Array.AsReadOnly(new[] { "oem", "get_hwnff_ver" })),
        new FastbootReadCommand("Key version", Array.AsReadOnly(new[] { "oem", "get_key_version" })),
        new FastbootReadCommand("Boot lock state", Array.AsReadOnly(new[] { "oem", "lock-state", "info" }))
    });
    static readonly IReadOnlyList<string> AndroidReadOnlyView = Array.AsReadOnly(AndroidReadVariables);
    static readonly IReadOnlyList<string> HuaweiReadOnlyView = Array.AsReadOnly(HuaweiReadVariables);

    public static IReadOnlyList<FastbootProfileOption> Profiles { get; } =
    [
        new(FastbootDeviceProfile.Android, "Android Fastboot"),
        new(FastbootDeviceProfile.Huawei, "Huawei Fastboot (experimental read-only)")
    ];

    public static IReadOnlyList<string> GetReadVariables(FastbootDeviceProfile profile)
        => profile switch
        {
            FastbootDeviceProfile.Android => AndroidReadOnlyView,
            FastbootDeviceProfile.Huawei => HuaweiReadOnlyView,
            _ => throw new ArgumentOutOfRangeException(nameof(profile))
        };

    public static IReadOnlyList<FastbootReadCommand> GetHuaweiOemReadCommands()
        => HuaweiOemReadCommands;

    [GeneratedRegex(@"^\s*(?<serial>\S+)\s+fastboot(?:\s|$)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex DeviceLineRegex();

    public static IReadOnlyList<FastbootTarget> ParseDevices(string output)
    {
        ArgumentNullException.ThrowIfNull(output);
        return output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => DeviceLineRegex().Match(line))
            .Where(match => match.Success)
            .Select(match => new FastbootTarget(match.Groups["serial"].Value))
            .DistinctBy(target => target.Serial, StringComparer.Ordinal)
            .ToArray();
    }

    public static FastbootFact ParseVariable(string name, string output, int exitCode)
        => ParseAllowlistedVariable(name, output, exitCode, AllowedVariables);

    public static FastbootFact ParseHuaweiVariable(string name, string output, int exitCode)
        => ParseAllowlistedVariable(name, output, exitCode, AllowedHuaweiVariables);

    public static FastbootFact ParseHuaweiProductModel(string output, int exitCode)
    {
        ArgumentNullException.ThrowIfNull(output);
        const string factName = "Product model";
        foreach (var line in output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            var text = line.Trim();
            if (text.StartsWith("(bootloader)", StringComparison.OrdinalIgnoreCase))
                text = text["(bootloader)".Length..].Trim();
            if (text.StartsWith("product model:", StringComparison.OrdinalIgnoreCase))
                text = text["product model:".Length..].Trim();
            if (text.StartsWith("product-model:", StringComparison.OrdinalIgnoreCase))
                text = text["product-model:".Length..].Trim();

            if (Regex.IsMatch(text, @"^[A-Za-z0-9][A-Za-z0-9._-]{1,63}$", RegexOptions.CultureInvariant)
                && !text.Equals("OKAY", StringComparison.OrdinalIgnoreCase)
                && !text.Equals("FAILED", StringComparison.OrdinalIgnoreCase))
                return new FastbootFact(factName, text, "Success");
        }

        if (IsPermissionDeniedResponse(output))
            return new FastbootFact(factName, string.Empty, "PermissionDenied");
        if (IsUnsupportedResponse(output) || (exitCode == 0 && !IsFailureResponse(output)))
            return new FastbootFact(factName, string.Empty, "NotReported");
        throw new InvalidOperationException("Fastboot could not read the Huawei product model.");
    }

    public static FastbootFact ParseHuaweiOemRead(string name, string output, int exitCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(output);
        if (!HuaweiOemReadCommands.Any(command => command.Name == name))
            throw new ArgumentOutOfRangeException(nameof(name), "The requested Huawei OEM read is not allowlisted.");
        if (name == "Product model") return ParseHuaweiProductModel(output, exitCode);

        if (IsPermissionDeniedResponse(output)) return new FastbootFact(name, string.Empty, "PermissionDenied");
        if (IsUnsupportedResponse(output)) return new FastbootFact(name, string.Empty, "NotReported");
        if (IsTransportFailure(output))
            throw new InvalidOperationException($"Fastboot transport failed while reading '{name}'.");

        if (name == "Boot lock state")
        {
            var states = new List<string>(2);
            foreach (var line in output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
            {
                var text = StripBootloaderPrefix(line.Trim());
                var match = Regex.Match(text, @"^(?<lock>FB|USER)\s+LockState\s*:\s*(?<state>LOCKED|UNLOCKED)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                if (match.Success)
                    states.Add($"{match.Groups["lock"].Value.ToUpperInvariant()}: {match.Groups["state"].Value.ToUpperInvariant()}");
            }
            if (states.Count > 0) return new FastbootFact(name, string.Join("; ", states.Distinct()), "Success");
        }
        else
        {
            foreach (var line in output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
            {
                var text = StripBootloaderPrefix(line.Trim()).TrimStart(':', ' ').Trim();
                if (text.Length == 0 || IsFastbootControlLine(text)) continue;
                if (text.Length <= 160 && Regex.IsMatch(text, @"^[A-Za-z0-9][A-Za-z0-9._() /+-]{0,159}$", RegexOptions.CultureInvariant))
                    return new FastbootFact(name, text, "Success");
            }
        }

        if (IsFailureResponse(output)) return new FastbootFact(name, string.Empty, "NotReported");
        if (exitCode == 0) return new FastbootFact(name, string.Empty, "NotReported");
        throw new InvalidOperationException($"Fastboot could not read the Huawei '{name}' value.");
    }

    public static FastbootFact ParseHuaweiEquipmentModelFromBuildVersion(string buildVersion)
    {
        ArgumentNullException.ThrowIfNull(buildVersion);
        var match = Regex.Match(buildVersion.Trim(), @"^(?<model>[A-Za-z][A-Za-z0-9._-]{1,63})(?=\s+\d)", RegexOptions.CultureInvariant);
        return match.Success
            ? new FastbootFact("Equipment model", match.Groups["model"].Value, "Derived")
            : new FastbootFact("Equipment model", string.Empty, "NotReported");
    }

    static FastbootFact ParseAllowlistedVariable(string name, string output, int exitCode, HashSet<string> allowedVariables)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(output);
        if (!allowedVariables.Contains(name))
            throw new ArgumentOutOfRangeException(nameof(name), "The requested Fastboot variable is not allowlisted.");

        foreach (var line in output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            var value = ParseValueLine(name, line);
            if (value is not null)
                return new FastbootFact(name, value, "Success");
        }

        if (IsUnsupportedResponse(output))
            return new FastbootFact(name, string.Empty, "NotReported");

        if (IsPermissionDeniedResponse(output))
            return new FastbootFact(name, string.Empty, "PermissionDenied");

        if (exitCode == 0 && !IsFailureResponse(output))
            return new FastbootFact(name, string.Empty, "NotReported");

        throw new InvalidOperationException($"Fastboot could not read the allowlisted '{name}' value.");
    }

    public static string ClassifyMode(IEnumerable<FastbootFact> facts)
    {
        ArgumentNullException.ThrowIfNull(facts);
        var userspace = facts.FirstOrDefault(fact => fact.Name == "is-userspace" && fact.Status == "Success")?.Value;
        return userspace?.Trim().ToLowerInvariant() switch
        {
            "yes" => "Fastbootd",
            "no" => "Bootloader Fastboot",
            _ => "Fastboot mode (unclassified)"
        };
    }

    static string? ParseValueLine(string name, string line)
    {
        var text = line.Trim();
        if (text.StartsWith("(bootloader)", StringComparison.OrdinalIgnoreCase))
            text = text["(bootloader)".Length..].Trim();
        var separator = text.IndexOf(':');
        if (separator < 0 || !string.Equals(text[..separator].Trim(), name, StringComparison.Ordinal))
            return null;
        var value = text[(separator + 1)..].Trim();
        return value.Length == 0 ? null : value;
    }

    static bool IsUnsupportedResponse(string output)
        => output.Contains("unknown variable", StringComparison.OrdinalIgnoreCase)
            || output.Contains("variable not found", StringComparison.OrdinalIgnoreCase)
            || output.Contains("not supported", StringComparison.OrdinalIgnoreCase)
            || output.Contains("unsupported", StringComparison.OrdinalIgnoreCase)
            || output.Contains("not implemented", StringComparison.OrdinalIgnoreCase)
            || output.Contains("unknown command", StringComparison.OrdinalIgnoreCase)
            || output.Contains("unknown status code", StringComparison.OrdinalIgnoreCase);

    static bool IsPermissionDeniedResponse(string output)
        => output.Contains("command not allowed", StringComparison.OrdinalIgnoreCase)
            || output.Contains("not permitted", StringComparison.OrdinalIgnoreCase)
            || output.Contains("access denied", StringComparison.OrdinalIgnoreCase)
            || output.Contains("no permissions", StringComparison.OrdinalIgnoreCase);

    static bool IsFailureResponse(string output)
        => output.Contains("failed", StringComparison.OrdinalIgnoreCase)
            || output.Contains("error", StringComparison.OrdinalIgnoreCase)
            || output.Contains("disconnected", StringComparison.OrdinalIgnoreCase);

    static bool IsTransportFailure(string output)
        => output.Contains("disconnected", StringComparison.OrdinalIgnoreCase)
            || output.Contains("transport error", StringComparison.OrdinalIgnoreCase)
            || output.Contains("write failed", StringComparison.OrdinalIgnoreCase)
            || output.Contains("timed out", StringComparison.OrdinalIgnoreCase);

    static string StripBootloaderPrefix(string text)
        => text.StartsWith("(bootloader)", StringComparison.OrdinalIgnoreCase)
            ? text["(bootloader)".Length..].Trim()
            : text;

    static bool IsFastbootControlLine(string text)
        => text.Equals("OKAY", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("OKAY ", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("FAILED", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("Finished", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("Total time:", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("INFO", StringComparison.OrdinalIgnoreCase);
}
