using logcat.Services;
using Xunit;

namespace logcat.Tests;

public sealed class PrivateDnsSettingsTests
{
    [Theory]
    [InlineData("off", PrivateDnsMode.Off)]
    [InlineData("opportunistic", PrivateDnsMode.Opportunistic)]
    [InlineData("hostname", PrivateDnsMode.Hostname)]
    [InlineData("future-mode", PrivateDnsMode.Unknown)]
    public void Parse_RecognizesKnownModesAndPreservesUnknownAsUnknown(string mode, PrivateDnsMode expected)
    {
        var result = PrivateDnsSettings.Parse(mode, null, "dns.example");
        Assert.Equal(expected, result.Mode);
        Assert.Equal(expected == PrivateDnsMode.Hostname ? "dns.example" : null, result.ProviderHostname);
    }

    [Theory]
    [InlineData(null, "opportunistic", PrivateDnsMode.Opportunistic)]
    [InlineData("null", "off", PrivateDnsMode.Off)]
    [InlineData("", "hostname", PrivateDnsMode.Hostname)]
    public void Parse_UsesDefaultModeWhenCurrentModeIsUnset(string? mode, string defaultMode, PrivateDnsMode expected)
    {
        var result = PrivateDnsSettings.Parse(mode, defaultMode, "resolver.example");
        Assert.Equal(expected, result.Mode);
        Assert.Equal(expected == PrivateDnsMode.Hostname ? "resolver.example" : null, result.ProviderHostname);
    }
}
