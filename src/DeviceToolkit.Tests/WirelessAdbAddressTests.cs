using logcat.Services;
using Xunit;

namespace logcat.Tests;

public sealed class WirelessAdbAddressTests
{
    [Theory]
    [InlineData("192.168.1.24", "37123", "192.168.1.24:37123")]
    [InlineData(" 10.0.0.8 ", "5555", "10.0.0.8:5555")]
    public void ParsesIpv4Endpoint(string address, string port, string expected)
        => Assert.Equal(expected, WirelessAdbAddress.ParseIpv4Endpoint(address, port).ToString());

    [Theory]
    [InlineData("localhost", "37123")]
    [InlineData("192.168.1.1:37123", "37123")]
    [InlineData("256.1.1.1", "37123")]
    [InlineData("2001:db8::1", "37123")]
    [InlineData("192.168.1.1;adb disconnect", "37123")]
    public void RejectsNonLiteralOrNonIpv4Addresses(string address, string port)
        => Assert.Throws<ArgumentException>(() => WirelessAdbAddress.ParseIpv4Endpoint(address, port));

    [Theory]
    [InlineData("0")]
    [InlineData("65536")]
    [InlineData("port")]
    [InlineData("+5555")]
    public void RejectsInvalidPorts(string port)
        => Assert.Throws<ArgumentOutOfRangeException>(() => WirelessAdbAddress.ParseIpv4Endpoint("192.168.1.1", port));

    [Theory]
    [InlineData("123456")]
    [InlineData("000001")]
    public void AcceptsSixDigitPairingCodes(string code)
        => WirelessAdbAddress.ValidatePairingCode(code);

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("12ab56")]
    public void RejectsMalformedPairingCodes(string code)
        => Assert.Throws<ArgumentException>(() => WirelessAdbAddress.ValidatePairingCode(code));

    [Fact]
    public void RecognizesOnlySuccessfulAdbResponses()
    {
        Assert.True(WirelessAdbAddress.IsPairingSuccess("Successfully paired to 192.168.1.24:37123"));
        Assert.False(WirelessAdbAddress.IsPairingSuccess("Failed to pair: incorrect code"));
        Assert.True(WirelessAdbAddress.IsConnectSuccess("connected to 192.168.1.24:5555"));
        Assert.True(WirelessAdbAddress.IsConnectSuccess("already connected to 192.168.1.24:5555"));
        Assert.False(WirelessAdbAddress.IsConnectSuccess("failed to connect to 192.168.1.24:5555"));
    }
}
