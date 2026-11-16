using logcat.Services;
using Xunit;

namespace logcat.Tests;

public sealed class AndroidFileTransferPathsTests
{
    [Theory]
    [InlineData("/sdcard/Download/backup.zip", "/sdcard/Download/backup.zip")]
    [InlineData("/sdcard/Download/a'b.zip", "/sdcard/Download/a'b.zip")]
    public void AcceptsFilesInsideSharedStorage(string input, string expected)
        => Assert.Equal(expected, AndroidFileTransferPaths.NormalizeSharedFile(input));

    [Theory]
    [InlineData("/sdcard")]
    [InlineData("/sdcard/Download/file.zip/")]
    [InlineData("/data/local/tmp/file")]
    [InlineData("/sdcard/../data/file")]
    public void RejectsFilesOutsideTheSharedStorageScope(string input)
        => Assert.Throws<ArgumentException>(() => AndroidFileTransferPaths.NormalizeSharedFile(input));

    [Fact]
    public void CreatesHiddenUniqueStageBesideDestination()
    {
        var operationId = Guid.Parse("f48bb25e-d70a-4c67-9f12-61a4f64043f5");

        Assert.Equal(
            "/sdcard/Download/.device-toolkit-f48bb25ed70a4c679f1261a4f64043f5.partial",
            AndroidFileTransferPaths.CreateStagingPath("/sdcard/Download/a.zip", operationId));
    }

    [Fact]
    public void RequiresAnOperationIdForTheTemporaryPath()
        => Assert.Throws<ArgumentException>(() => AndroidFileTransferPaths.CreateStagingPath("/sdcard/Download/file.zip", Guid.Empty));

    [Fact]
    public void QuotesNamesContainingShellSyntaxAsOneArgument()
    {
        Assert.Equal("'a'\\''b $(id).zip'", AndroidFileTransferPaths.QuotePosixShellArgument("a'b $(id).zip"));
    }
}
