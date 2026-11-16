using System.Text;

namespace logcat.Services;

/// <summary>Writes a bounded logcat stream as UTF-8 and flushes accepted lines on every exit path.</summary>
public static class BoundedLogcatFileWriter
{
    public static async Task WriteAsync(
        IAsyncEnumerable<string> lines,
        string outputPath,
        long maximumBytes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        if (maximumBytes <= 0) throw new ArgumentOutOfRangeException(nameof(maximumBytes));

        var directory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

        await using var file = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        await using var writer = new StreamWriter(file, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        long bytesWritten = 0;

        try
        {
            await foreach (var line in lines.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                if (string.IsNullOrWhiteSpace(line) || LogParser.IsBanner(line)) continue;

                var lineBytes = Encoding.UTF8.GetByteCount(line) + Encoding.UTF8.GetByteCount(Environment.NewLine);
                if (lineBytes > maximumBytes - bytesWritten)
                    throw new InvalidDataException("The bounded log capture exceeded its local size limit.");

                await writer.WriteLineAsync(line).ConfigureAwait(false);
                bytesWritten += lineBytes;
            }
        }
        finally
        {
            // Flush accepted data even when the device stream is cancelled or the size limit is hit.
            await writer.FlushAsync(CancellationToken.None).ConfigureAwait(false);
            await file.FlushAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
