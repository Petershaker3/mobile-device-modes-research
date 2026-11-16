using System.Runtime.CompilerServices;
using System.Text;
using logcat.Services;
using Xunit;

namespace logcat.Tests;

public sealed class BoundedLogcatFileWriterTests
{
    [Fact]
    public async Task CallerCancellationFlushesEveryAcceptedLineBeforePropagating()
    {
        var path = NewPath();
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
        try
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                BoundedLogcatFileWriter.WriteAsync(OneLineThenWait("last buffered line", cancellation.Token),
                    path, 1024, cancellation.Token));

            Assert.Equal("last buffered line" + Environment.NewLine, await File.ReadAllTextAsync(path));
        }
        finally
        {
            TryDelete(path);
        }
    }

    [Fact]
    public async Task SizeLimitFlushesAcceptedLinesAndRejectsOverflowLine()
    {
        var path = NewPath();
        const string accepted = "ok";
        var limit = Encoding.UTF8.GetByteCount(accepted + Environment.NewLine);
        try
        {
            await Assert.ThrowsAsync<InvalidDataException>(() =>
                BoundedLogcatFileWriter.WriteAsync(Lines(accepted, "overflow"), path, limit));

            Assert.Equal(accepted + Environment.NewLine, await File.ReadAllTextAsync(path));
        }
        finally
        {
            TryDelete(path);
        }
    }

    [Fact]
    public async Task IgnoresBlankAndLogcatBannerLines()
    {
        var path = NewPath();
        try
        {
            await BoundedLogcatFileWriter.WriteAsync(
                Lines("", "--------- beginning of main", "I/Tag: message"), path, 1024);

            Assert.Equal("I/Tag: message" + Environment.NewLine, await File.ReadAllTextAsync(path));
        }
        finally
        {
            TryDelete(path);
        }
    }

    static string NewPath() => Path.Combine(Path.GetTempPath(), $"bounded-logcat-{Guid.NewGuid():N}.log");

    static async IAsyncEnumerable<string> OneLineThenWait(
        string line,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return line;
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    static async IAsyncEnumerable<string> Lines(params string[] lines)
    {
        foreach (var line in lines)
        {
            yield return line;
            await Task.Yield();
        }
    }

    static void TryDelete(string path)
    {
        try { File.Delete(path); } catch { }
    }
}
