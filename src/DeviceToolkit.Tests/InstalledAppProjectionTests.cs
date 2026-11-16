using logcat.Services;
using Xunit;

namespace logcat.Tests;

public sealed class InstalledAppProjectionTests
{
    [Fact]
    public void ProjectsAndSortsPackageIdsWithTheirInstallPaths()
    {
        var packages = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["com.vendor.zeta"] = "/system/app/Zeta/Zeta.apk",
            ["com.vendor.alpha"] = "/data/app/Alpha/base.apk"
        };

        var result = InstalledAppProjection.FromPackagePaths(packages);

        Assert.Collection(result,
            item => Assert.Equal(new InstalledApp("com.vendor.alpha", "/data/app/Alpha/base.apk"), item),
            item => Assert.Equal(new InstalledApp("com.vendor.zeta", "/system/app/Zeta/Zeta.apk"), item));
    }

    [Fact]
    public void EmptyPackageInventoryReturnsNoRows()
        => Assert.Empty(InstalledAppProjection.FromPackagePaths(new Dictionary<string, string>()));

    [Fact]
    public void MarksOnlyPackagesReturnedByTheThirdPartyInventoryAsRemovable()
    {
        var packages = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["com.vendor.user"] = "/data/app/User/base.apk",
            ["com.android.system"] = "/system/app/System/System.apk"
        };

        var result = InstalledAppProjection.FromPackagePaths(packages, new HashSet<string>(["com.vendor.user"], StringComparer.Ordinal));

        Assert.True(result.Single(app => app.PackageId == "com.vendor.user").IsThirdParty);
        Assert.False(result.Single(app => app.PackageId == "com.android.system").IsThirdParty);
    }
}
