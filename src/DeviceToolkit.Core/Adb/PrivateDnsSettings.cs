namespace logcat.Services;

public enum PrivateDnsMode
{
    Off,
    Opportunistic,
    Hostname,
    Unknown
}

public sealed record PrivateDnsSettings(PrivateDnsMode Mode, string? ProviderHostname)
{
    public static PrivateDnsSettings Parse(string? mode, string? defaultMode, string? specifier)
    {
        var effectiveMode = IsUnset(mode) ? defaultMode?.Trim() : mode?.Trim();
        var parsed = effectiveMode?.ToLowerInvariant() switch
        {
            "off" => PrivateDnsMode.Off,
            "opportunistic" => PrivateDnsMode.Opportunistic,
            "hostname" => PrivateDnsMode.Hostname,
            _ => PrivateDnsMode.Unknown
        };
        var hostname = parsed == PrivateDnsMode.Hostname && !IsUnset(specifier) ? specifier!.Trim() : null;
        return new PrivateDnsSettings(parsed, hostname);
    }

    static bool IsUnset(string? value)
        => string.IsNullOrWhiteSpace(value) || string.Equals(value.Trim(), "null", StringComparison.OrdinalIgnoreCase);
}
