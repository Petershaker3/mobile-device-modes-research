using DeviceToolkit.Core.Services;
using logcat.Services;
using Xunit;

namespace logcat.Tests;

public sealed class DevicePropertyOutputParserTests
{
    [Fact]
    public void ReadCommandContainsOnlyTheFixedPropertyAllowlist()
    {
        var command = ReadOnlyDeviceService.BuildAllowlistedPropertyCommand();

        Assert.Contains("getprop 'ro.product.model'", command);
        Assert.Contains("getprop 'ro.build.version.security_patch'", command);
        Assert.Contains("getprop 'ro.boot.warranty_bit'", command);
        Assert.Contains("getprop 'ro.boot.verifiedbootstate'", command);
        Assert.Contains("getprop 'ro.boot.flash.locked'", command);
        Assert.Contains("getprop 'ro.csc.sales_code'", command);
        Assert.DoesNotContain("ro.serialno", command);
        Assert.DoesNotContain("imei", command, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("getprop'", command);
    }

    [Fact]
    public void ParsesOnlyAllowlistedPropertiesAndKeepsEqualsInsideValues()
    {
        var parsed = ReadOnlyDeviceService.ParsePropertyOutput(
            "ro.product.model=Pixel Example\n" +
            "ro.build.display.id=release=stable\n" +
            "ro.serialno=private-value\n" +
            "not a property line\n");

        Assert.Equal("Pixel Example", parsed["Model"]);
        Assert.Equal("release=stable", parsed["BuildDisplay"]);
        Assert.False(parsed.ContainsKey("ro.serialno"));
        Assert.Equal(2, parsed.Count);
    }

    [Fact]
    public void EmptyAndMissingPropertiesAreNotInvented()
    {
        var parsed = ReadOnlyDeviceService.ParsePropertyOutput(
            "ro.product.model=\n" +
            "ro.build.version.sdk=35\n");

        Assert.Equal(string.Empty, parsed["Model"]);
        Assert.Equal("35", parsed["Api"]);
        Assert.False(parsed.ContainsKey("SecurityPatch"));
    }

    [Fact]
    public async Task ReadFactsUsesExplicitTargetAndMapsAllowlistedFields()
    {
        var commands = new List<string>();
        var shell = new AdbShell((arguments, onLine, _) =>
        {
            commands.Add(arguments);
            if (arguments.Contains("printf '%s='", StringComparison.Ordinal))
            {
                onLine?.Report("ro.product.model=Model X");
                onLine?.Report("ro.product.manufacturer=Maker");
                onLine?.Report("ro.product.brand=Brand");
                onLine?.Report("ro.product.name=product_x");
                onLine?.Report("ro.product.device=device_x");
                onLine?.Report("ro.build.version.release=15");
                onLine?.Report("ro.build.version.sdk=35");
                onLine?.Report("ro.build.id=BUILD123");
                onLine?.Report("ro.build.display.id=BUILD123 release");
                onLine?.Report("ro.build.version.security_patch=2026-10-01");
                onLine?.Report("ro.product.cpu.abi=arm64-v8a");
                onLine?.Report("ro.product.cpu.abilist=arm64-v8a,armeabi-v7a");
                onLine?.Report("ro.bootmode=unknown");
                onLine?.Report("ro.bootloader=bootloader_x");
                onLine?.Report("gsm.version.baseband=baseband_x");
                onLine?.Report("ro.csc.sales_code=BTU");
                onLine?.Report("ro.boot.flash.locked=1");
                onLine?.Report("ro.boot.verifiedbootstate=green");
                onLine?.Report("ro.boot.warranty_bit=0");
                onLine?.Report("ro.boot.veritymode=enforcing");
            }
            else if (arguments.EndsWith("settings get global private_dns_mode", StringComparison.Ordinal))
                onLine?.Report("hostname");
            else if (arguments.EndsWith("settings get global private_dns_specifier", StringComparison.Ordinal))
                onLine?.Report("resolver.example");
            else if (arguments.EndsWith("settings get global private_dns_default_mode", StringComparison.Ordinal))
                onLine?.Report("opportunistic");
            return Task.FromResult(0);
        });

        var facts = await new ReadOnlyDeviceService(shell).ReadFactsAsync(
            new ConnectedDevice("TEST_DEVICE", "Model X", "Online"), CancellationToken.None);

        Assert.Equal("Model X", facts.Model);
        Assert.Equal("Brand", facts.Brand);
        Assert.Equal("35", facts.Api);
        Assert.Equal("2026-10-01", facts.SecurityPatch);
        Assert.Equal("arm64-v8a,armeabi-v7a", facts.SupportedAbis);
        Assert.Equal("unknown", facts.BootMode);
        Assert.Equal("BTU", facts.SamsungCsc);
        Assert.Equal("1", facts.SamsungBootloaderLocked);
        Assert.Equal("green", facts.SamsungVerifiedBootState);
        Assert.Equal("0", facts.SamsungWarrantyBit);
        Assert.Equal("enforcing", facts.SamsungVerityMode);
        Assert.Equal("Unavailable", facts.SelinuxMode);
        Assert.Equal(PrivateDnsMode.Hostname, facts.PrivateDns.Mode);
        Assert.Equal(5, commands.Count);
        Assert.All(commands, command => Assert.StartsWith("-s TEST_DEVICE shell ", command, StringComparison.Ordinal));
    }
}
