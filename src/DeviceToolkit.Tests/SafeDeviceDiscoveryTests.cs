using AdvancedSharpAdbClient.Models;
using logcat.Services;
using Xunit;

namespace logcat.Tests;

public sealed class SafeDeviceDiscoveryTests
{
    [Fact]
    public void EnumeratesMetadataAndShowsSerialForExplicitTargetSelection()
    {
        var source = new[]
        {
            new DeviceData
            {
                Serial = "SERIAL_PRIVATE_1234",
                Model = "SM_N976N",
                State = DeviceState.Online
            }
        };
        var discovery = new SafeDeviceDiscovery(() => source);

        var device = Assert.Single(discovery.ListDevices());

        Assert.Equal("SERIAL_PRIVATE_1234", device.Serial);
        Assert.Equal("SM N976N", device.Model);
        Assert.Equal("SM N976N (SERIAL_PRIVATE_1234) [Online]", device.DisplayName);
    }
}
