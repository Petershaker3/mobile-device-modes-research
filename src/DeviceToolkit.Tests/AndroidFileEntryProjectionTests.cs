using AdvancedSharpAdbClient.Models;
using logcat.Services;
using Xunit;

namespace logcat.Tests;

public sealed class AndroidFileEntryProjectionTests
{
    [Theory]
    [InlineData(".")]
    [InlineData("..")]
    public void HidesSyncProtocolCurrentAndParentRecords(string name)
        => Assert.Null(AndroidFileEntryProjection.FromSyncEntry("/sdcard", Stat(name, UnixFileStatus.Directory)));

    [Fact]
    public void ProjectsAUnicodeDirectoryAsNavigableButWithoutAReportedSize()
    {
        var entry = AndroidFileEntryProjection.FromSyncEntry(
            "/sdcard",
            Stat("صور الهاتف", UnixFileStatus.Directory, size: 0));

        Assert.NotNull(entry);
        Assert.Equal("صور الهاتف", entry.Name);
        Assert.Equal("/sdcard/صور الهاتف", entry.RemotePath);
        Assert.Equal(AndroidFileKind.Directory, entry.Kind);
        Assert.Null(entry.SizeBytes);
        Assert.True(entry.CanNavigate);
    }

    [Fact]
    public void PreservesLargeFileSizesAndEscapesControlCharactersOnlyInDisplayName()
    {
        const string name = "report\nfinal.bin";
        var entry = AndroidFileEntryProjection.FromSyncEntry(
            "/sdcard/Download",
            Stat(name, UnixFileStatus.Regular, size: 5_000_000_000));

        Assert.NotNull(entry);
        Assert.Equal(name, entry.Name);
        Assert.Equal("report\\u000Afinal.bin", entry.DisplayName);
        Assert.Equal("/sdcard/Download/report\nfinal.bin", entry.RemotePath);
        Assert.Equal(5_000_000_000UL, entry.SizeBytes);
        Assert.False(entry.CanNavigate);
    }

    [Fact]
    public void DoesNotNavigateThroughSymbolicLinks()
    {
        var entry = AndroidFileEntryProjection.FromSyncEntry(
            "/sdcard",
            Stat("linked-folder", UnixFileStatus.SymbolicLink));

        Assert.NotNull(entry);
        Assert.Equal(AndroidFileKind.SymbolicLink, entry.Kind);
        Assert.False(entry.CanNavigate);
    }

    static FileStatisticsEx Stat(string path, UnixFileStatus kind, ulong size = 0, uint error = 0)
        => new(new FileStatisticsDataEx(
            error,
            Device: 0,
            IndexNode: 0,
            Mode: (uint)kind,
            LinkCount: 1,
            UserId: 0,
            GroupId: 0,
            Size: size,
            AccessTime: 1_700_000_000,
            ModifiedTime: 1_700_000_000,
            ChangedTime: 1_700_000_000))
        {
            Path = path
        };
}
