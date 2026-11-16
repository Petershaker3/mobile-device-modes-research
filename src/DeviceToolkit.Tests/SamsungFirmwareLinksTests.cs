using DeviceToolkit.Core.Services;
using Xunit;

namespace logcat.Tests;

public sealed class SamsungFirmwareLinksTests
{
    [Fact]
    public void UsesExactReportedModelAndValidCsc()
    {
        var uri = SamsungFirmwareLinks.ForReportedDevice("samsung", "SM-G991B", "BTU");

        Assert.Equal("https://samfw.com/firmware/SM-G991B/BTU", uri?.AbsoluteUri);
    }

    [Fact]
    public void FallsBackToModelListingWhenCscIsMissingOrInvalid()
    {
        var uri = SamsungFirmwareLinks.ForReportedDevice("SAMSUNG", "SM-X820", "Unavailable");

        Assert.Equal("https://samfw.com/firmware/SM-X820", uri?.AbsoluteUri);
    }

    [Theory]
    [InlineData("Google", "SM-G991B", "BTU")]
    [InlineData("Samsung", "Pixel 9", "BTU")]
    [InlineData("Samsung", "SM-G991B/../../evil", "BTU")]
    public void DoesNotBuildUrlForNonSamsungOrInvalidModel(string manufacturer, string model, string csc)
    {
        Assert.Null(SamsungFirmwareLinks.ForReportedDevice(manufacturer, model, csc));
    }
}
