using System.Text;

namespace logcat.Services;

/// <summary>Path rules for the read-only shared-storage browser.</summary>
public static class AndroidSharedStoragePaths
{
    public const string Root = "/sdcard";
    const int MaximumUtf8PathBytes = 4096;

    public static string NormalizeDirectory(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (path.Length == 0 || path.Contains('\0') || Encoding.UTF8.GetByteCount(path) > MaximumUtf8PathBytes)
            throw new ArgumentException("The Android directory path is invalid.", nameof(path));
        if (!path.StartsWith("/", StringComparison.Ordinal))
            throw new ArgumentException("Android directory paths must be absolute.", nameof(path));

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0 || !string.Equals(segments[0], "sdcard", StringComparison.Ordinal))
            throw new ArgumentException("The file browser is limited to shared storage.", nameof(path));
        if (segments.Any(segment => segment is "." or ".."))
            throw new ArgumentException("Relative path segments are not allowed.", nameof(path));

        return "/" + string.Join('/', segments);
    }

    public static string CombineChild(string parent, string childName)
    {
        var normalizedParent = NormalizeDirectory(parent);
        if (string.IsNullOrEmpty(childName) || childName is "." or ".." ||
            childName.Contains('/') || childName.Contains('\0'))
            throw new ArgumentException("The Android child name is invalid.", nameof(childName));
        return NormalizeDirectory($"{normalizedParent.TrimEnd('/')}/{childName}");
    }

    public static string? GetParent(string path)
    {
        var normalized = NormalizeDirectory(path);
        if (normalized == Root) return null;
        var separator = normalized.LastIndexOf('/');
        var parent = separator <= 0 ? Root : normalized[..separator];
        return parent.Length < Root.Length ? Root : parent;
    }

    public static string ToDisplayText(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var display = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (char.IsControl(character))
                display.Append("\\u").Append(((int)character).ToString("X4", System.Globalization.CultureInfo.InvariantCulture));
            else
                display.Append(character);
        }
        return display.ToString();
    }
}
