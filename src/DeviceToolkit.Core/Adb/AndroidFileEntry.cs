using System.Globalization;
using AdvancedSharpAdbClient.Models;

namespace logcat.Services;

public enum AndroidFileKind
{
    File,
    Directory,
    SymbolicLink,
    Other,
    Unknown
}

public sealed record AndroidFileEntry(
    string Name,
    string DisplayName,
    string RemotePath,
    AndroidFileKind Kind,
    ulong? SizeBytes,
    DateTimeOffset? ModifiedAt,
    UnixErrorCode Error)
{
    public string TypeText => Error != UnixErrorCode.Default
        ? $"Error: {Error}"
        : Kind switch
        {
            AndroidFileKind.Directory => "Folder",
            AndroidFileKind.File => "File",
            AndroidFileKind.SymbolicLink => "Link",
            AndroidFileKind.Other => "Other",
            _ => "Unknown"
        };

    public string SizeText => SizeBytes is ulong size
        ? string.Format(CultureInfo.CurrentCulture, "{0:N0} bytes", size)
        : string.Empty;

    public string ModifiedText => ModifiedAt?.ToLocalTime().ToString("g", CultureInfo.CurrentCulture) ?? string.Empty;

    public bool CanNavigate => Error == UnixErrorCode.Default && Kind == AndroidFileKind.Directory;
}

public static class AndroidFileEntryProjection
{
    public static AndroidFileEntry? FromSyncEntry(string parentDirectory, FileStatisticsEx stat)
    {
        ArgumentNullException.ThrowIfNull(stat);
        var name = stat.Path;
        if (name is "." or "..") return null;

        var path = AndroidSharedStoragePaths.CombineChild(parentDirectory, name);
        var error = stat.Error;
        var kind = error == UnixErrorCode.Default
            ? GetKind(stat.FileMode)
            : AndroidFileKind.Unknown;
        DateTimeOffset? modified = null;
        if (error == UnixErrorCode.Default)
        {
            try { modified = stat.ModifiedTime; }
            catch (ArgumentOutOfRangeException) { }
        }

        return new AndroidFileEntry(
            name,
            AndroidSharedStoragePaths.ToDisplayText(name),
            path,
            kind,
            error == UnixErrorCode.Default && kind != AndroidFileKind.Directory ? stat.Size : null,
            modified,
            error);
    }

    static AndroidFileKind GetKind(UnixFileStatus mode)
        => (mode & UnixFileStatus.TypeMask) switch
        {
            UnixFileStatus.Directory => AndroidFileKind.Directory,
            UnixFileStatus.Regular => AndroidFileKind.File,
            UnixFileStatus.SymbolicLink => AndroidFileKind.SymbolicLink,
            _ => AndroidFileKind.Other
        };
}
