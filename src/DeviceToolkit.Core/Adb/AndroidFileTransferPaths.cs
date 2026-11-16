using System.Text;

namespace logcat.Services;

/// <summary>Path rules shared by the single-file Android transfer workflows.</summary>
public static class AndroidFileTransferPaths
{
    const int MaximumUtf8PathBytes = 4096;

    public static string NormalizeSharedFile(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (path.Length == 0 || path.Contains('\0') || Encoding.UTF8.GetByteCount(path) > MaximumUtf8PathBytes)
            throw new ArgumentException("The Android file path is invalid.", nameof(path));
        if (path.EndsWith("/", StringComparison.Ordinal))
            throw new ArgumentException("The Android file path cannot end with a directory separator.", nameof(path));

        var normalized = AndroidSharedStoragePaths.NormalizeDirectory(path);
        if (normalized == AndroidSharedStoragePaths.Root)
            throw new ArgumentException("The shared-storage root is not a file.", nameof(path));
        return normalized;
    }

    public static string CreateStagingPath(string destinationPath, Guid operationId)
    {
        if (operationId == Guid.Empty) throw new ArgumentException("A unique operation ID is required.", nameof(operationId));
        var destination = NormalizeSharedFile(destinationPath);
        var parent = AndroidSharedStoragePaths.GetParent(destination)
            ?? throw new ArgumentException("The destination has no parent directory.", nameof(destinationPath));
        return AndroidSharedStoragePaths.CombineChild(parent, $".device-toolkit-{operationId:N}.partial");
    }

    public static string QuotePosixShellArgument(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Contains('\0')) throw new ArgumentException("NUL is not valid in a shell argument.", nameof(value));
        return "'" + value.Replace("'", "'\\''", StringComparison.Ordinal) + "'";
    }
}
