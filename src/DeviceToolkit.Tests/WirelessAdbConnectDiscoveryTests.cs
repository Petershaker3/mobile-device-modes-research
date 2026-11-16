using System.Net;
using logcat.Services;
using Xunit;

namespace logcat.Tests;

public sealed class WirelessAdbConnectDiscoveryTests
{
    static readonly IPAddress PhoneAddress = IPAddress.Parse("192.168.1.24");

    [Fact]
    public async Task WaitsUntilMatchingConnectServiceIsAdvertised()
    {
        var reads = 0;
        Task<IReadOnlyList<WirelessAdbService>> Read(CancellationToken _)
        {
            reads++;
            IReadOnlyList<WirelessAdbService> result = reads < 3
                ? []
                : [new WirelessAdbService(WirelessAdbServiceKind.Connect,
                    new IPEndPoint(PhoneAddress, 37123))];
            return Task.FromResult(result);
        }

        var endpoint = await WirelessAdbConnectDiscovery.WaitForMatchingConnectServiceAsync(
            Read, PhoneAddress, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(1), CancellationToken.None);

        Assert.Equal("192.168.1.24:37123", endpoint.ToString());
        Assert.Equal(3, reads);
    }

    [Fact]
    public async Task IgnoresConnectServiceAdvertisedForAnotherAddress()
    {
        var otherAddress = IPAddress.Parse("192.168.1.25");
        Task<IReadOnlyList<WirelessAdbService>> Read(CancellationToken _) =>
            Task.FromResult<IReadOnlyList<WirelessAdbService>>(
                [new WirelessAdbService(WirelessAdbServiceKind.Connect,
                    new IPEndPoint(otherAddress, 37123))]);

        await Assert.ThrowsAsync<TimeoutException>(() =>
            WirelessAdbConnectDiscovery.WaitForMatchingConnectServiceAsync(
                Read, PhoneAddress, TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(1), CancellationToken.None));
    }

    [Fact]
    public async Task RejectsAmbiguousConnectionPortsForThePairedPhone()
    {
        Task<IReadOnlyList<WirelessAdbService>> Read(CancellationToken _) =>
            Task.FromResult<IReadOnlyList<WirelessAdbService>>(
            [
                new WirelessAdbService(WirelessAdbServiceKind.Connect, new IPEndPoint(PhoneAddress, 37123)),
                new WirelessAdbService(WirelessAdbServiceKind.Connect, new IPEndPoint(PhoneAddress, 37124))
            ]);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            WirelessAdbConnectDiscovery.WaitForMatchingConnectServiceAsync(
                Read, PhoneAddress, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(1), CancellationToken.None));

        Assert.Contains("more than one connection port", error.Message, StringComparison.Ordinal);
    }
}
