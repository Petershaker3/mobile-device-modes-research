namespace logcat.Services;

/// <summary>Installed application inventory fields returned by the ADB package manager.</summary>
public sealed record InstalledApp(string PackageId, string ApkPath, bool IsThirdParty = false)
{
    public string AppKind => IsThirdParty ? "User app" : "System app";
}

public static class InstalledAppProjection
{
    public static IReadOnlyList<InstalledApp> FromPackagePaths(
        IReadOnlyDictionary<string, string> packages,
        IReadOnlySet<string>? thirdPartyPackageIds = null)
        => packages
            .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
            .Select(item => new InstalledApp(item.Key, item.Value, thirdPartyPackageIds?.Contains(item.Key) ?? false))
            .ToArray();
}
