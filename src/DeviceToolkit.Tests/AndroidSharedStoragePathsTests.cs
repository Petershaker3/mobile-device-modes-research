using logcat.Services;
using Xunit;

namespace logcat.Tests;

public sealed class AndroidSharedStoragePathsTests
{
    [Theory]
    [InlineData("/sdcard", "/sdcard")]
    [InlineData("//sdcard//DCIM/Camera/", "/sdcard/DCIM/Camera")]
    public void NormalizesDirectoriesWithinSharedStorage(string input, string expected)
        => Assert.Equal(expected, AndroidSharedStoragePaths.NormalizeDirectory(input));

    [Theory]
    [InlineData("relative/Download")]
    [InlineData("/data")]
    [InlineData("/sdcardary/Download")]
    [InlineData("/sdcard/../data")]
    [InlineData("/sdcard/./Download")]
    public void RejectsPathsOutsideOrTraversingSharedStorage(string input)
        => Assert.Throws<ArgumentException>(() => AndroidSharedStoragePaths.NormalizeDirectory(input));

    [Fact]
    public void CombinesUnicodeChildNamesWithoutChangingTheirIdentity()
    {
        const string childName = "مجلد 1";

        Assert.Equal("/sdcard/Download/مجلد 1", AndroidSharedStoragePaths.CombineChild("/sdcard/Download", childName));
    }

    [Fact]
    public void RefusesTraversalAsAChildName()
    {
        Assert.Throws<ArgumentException>(() => AndroidSharedStoragePaths.CombineChild("/sdcard", ".."));
        Assert.Throws<ArgumentException>(() => AndroidSharedStoragePaths.CombineChild("/sdcard", "a/b"));
    }

    [Fact]
    public void ParentStopsAtSharedStorageRoot()
    {
        Assert.Null(AndroidSharedStoragePaths.GetParent("/sdcard"));
        Assert.Equal("/sdcard", AndroidSharedStoragePaths.GetParent("/sdcard/Download"));
    }

    [Fact]
    public void EscapesControlCharactersForDisplayWithoutChangingPathNames()
    {
        const string name = "note\nfinal.txt";

        Assert.Equal("note\\u000Afinal.txt", AndroidSharedStoragePaths.ToDisplayText(name));
        Assert.Equal("/sdcard/note\nfinal.txt", AndroidSharedStoragePaths.CombineChild("/sdcard", name));
    }
}
