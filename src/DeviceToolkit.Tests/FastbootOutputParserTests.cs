using DeviceToolkit.Core.Services;
using Xunit;

namespace logcat.Tests;

public sealed class FastbootOutputParserTests
{
    [Fact]
    public void GetReadVariables_KeepsHuaweiCommandsSeparateAndAllowlisted()
    {
        Assert.Equal(
            new[] { "is-userspace", "product", "current-slot", "unlocked" },
            FastbootOutputParser.GetReadVariables(FastbootDeviceProfile.Android));
        Assert.Equal(
            new[] { "is-userspace", "product", "current-slot", "unlocked", "rescue_version", "vendorcountry" },
            FastbootOutputParser.GetReadVariables(FastbootDeviceProfile.Huawei));
        Assert.Equal(
            new[] { "Product model", "Build version", "Base version", "Custom version", "Preload version", "Android version", "HWNFF version", "Key version", "Boot lock state" },
            FastbootOutputParser.GetHuaweiOemReadCommands().Select(command => command.Name));
        Assert.DoesNotContain(FastbootOutputParser.GetHuaweiOemReadCommands(), command => command.Arguments.Contains("get-psid"));
        Assert.DoesNotContain(FastbootOutputParser.GetHuaweiOemReadCommands(), command => command.Arguments.Contains("oeminforead-hotainfo"));
        Assert.Throws<ArgumentOutOfRangeException>(() => FastbootOutputParser.GetReadVariables((FastbootDeviceProfile)99));
    }

    [Fact]
    public void ParseDevices_ReturnsOnlyFastbootTargetsAndDeduplicatesSerials()
    {
        var targets = FastbootOutputParser.ParseDevices("ABC123\tfastboot\r\n\r\nABC123 fastboot\nother\tunauthorized");

        Assert.Single(targets);
        Assert.Equal("ABC123", targets[0].Serial);
    }

    [Theory]
    [InlineData("(bootloader) is-userspace: yes", "Fastbootd")]
    [InlineData("is-userspace: no", "Bootloader Fastboot")]
    [InlineData("", "Fastboot mode (unclassified)")]
    public void ClassifyMode_RequiresAnExplicitUserspaceValue(string response, string expected)
    {
        var fact = FastbootOutputParser.ParseVariable("is-userspace", response, 0);
        Assert.Equal(expected, FastbootOutputParser.ClassifyMode([fact]));
        if (response.Length == 0) Assert.Equal("NotReported", fact.Status);
    }

    [Fact]
    public void ParseVariable_RecognizesFastbootStderrFormat()
    {
        var fact = FastbootOutputParser.ParseVariable("product", "(bootloader) product: example-product", 0);

        Assert.Equal("example-product", fact.Value);
        Assert.Equal("Success", fact.Status);
    }

    [Fact]
    public void ParseVariable_RepresentsUnsupportedVariableWithoutGuessing()
    {
        var fact = FastbootOutputParser.ParseVariable("current-slot", "FAILED (remote: 'Variable not found')", 1);

        Assert.Equal(string.Empty, fact.Value);
        Assert.Equal("NotReported", fact.Status);
    }

    [Fact]
    public void ParseVariable_RejectsVariablesOutsideTheAllowlist()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FastbootOutputParser.ParseVariable("serialno", "serialno: secret", 0));
    }

    [Theory]
    [InlineData("rescue_version", "(bootloader) rescue_version: 1.2.3", "1.2.3")]
    [InlineData("vendorcountry", "vendorcountry: hw/eu", "hw/eu")]
    public void ParseHuaweiVariable_ParsesOnlyTheSeparateHuaweiReadAllowlist(string name, string output, string expected)
    {
        var fact = FastbootOutputParser.ParseHuaweiVariable(name, output, 0);

        Assert.Equal(name, fact.Name);
        Assert.Equal(expected, fact.Value);
        Assert.Equal("Success", fact.Status);
        Assert.Throws<ArgumentOutOfRangeException>(() => FastbootOutputParser.ParseVariable(name, output, 0));
    }

    [Fact]
    public void ParseHuaweiVariable_ReportsUnsupportedVendorValueWithoutGuessing()
    {
        var fact = FastbootOutputParser.ParseHuaweiVariable("rescue_version", "FAILED (remote: 'Variable not found')", 1);

        Assert.Equal(string.Empty, fact.Value);
        Assert.Equal("NotReported", fact.Status);
    }

    [Theory]
    [InlineData("(bootloader) ANE-LX1", "ANE-LX1")]
    [InlineData("(bootloader) product model: BLA-L29\nOKAY", "BLA-L29")]
    public void ParseHuaweiProductModel_ExtractsOnlyAConstrainedModelToken(string output, string expected)
    {
        var fact = FastbootOutputParser.ParseHuaweiProductModel(output, 0);

        Assert.Equal("Product model", fact.Name);
        Assert.Equal(expected, fact.Value);
        Assert.Equal("Success", fact.Status);
    }

    [Fact]
    public void ParseHuaweiProductModel_RepresentsDeniedAndMissingResponses()
    {
        Assert.Equal("PermissionDenied", FastbootOutputParser.ParseHuaweiProductModel("FAILED (remote: 'Command not allowed')", 1).Status);
        Assert.Equal("NotReported", FastbootOutputParser.ParseHuaweiProductModel("", 0).Status);
        Assert.Throws<InvalidOperationException>(() => FastbootOutputParser.ParseHuaweiProductModel("FAILED (device disconnected)", 1));
    }

    [Theory]
    [InlineData("Build version", "(bootloader) :PPAH-L22 10.1.1.280(C185E8R2P1)\nOKAY [ 0.008s]", "PPAH-L22 10.1.1.280(C185E8R2P1)")]
    [InlineData("Base version", "(bootloader) PPAH-LGRP2-OVS 10.1.1.280\nOKAY", "PPAH-LGRP2-OVS 10.1.1.280")]
    [InlineData("Custom version", "(bootloader) PPAH-L22-CUST 10.1.1.8(C185)\nOKAY", "PPAH-L22-CUST 10.1.1.8(C185)")]
    [InlineData("Preload version", "(bootloader) PPAH-L22-PRELOAD 10.1.1.1(C185R2)\nOKAY", "PPAH-L22-PRELOAD 10.1.1.1(C185R2)")]
    [InlineData("Android version", "(bootloader) 10\nOKAY", "10")]
    [InlineData("HWNFF version", "(bootloader) Ver3.0\nOKAY", "Ver3.0")]
    [InlineData("Key version", "(bootloader) huawei_key_v1\nOKAY", "huawei_key_v1")]
    public void ParseHuaweiOemRead_ExtractsOnlyTheRequestedSingleValue(string name, string output, string expected)
    {
        var fact = FastbootOutputParser.ParseHuaweiOemRead(name, output, 0);

        Assert.Equal(name, fact.Name);
        Assert.Equal(expected, fact.Value);
        Assert.Equal("Success", fact.Status);
    }

    [Theory]
    [InlineData("PPAH-L22 10.1.1.280(C185E8R2P1)", "PPAH-L22")]
    [InlineData("MODEL_X2 12.0.0.1(C432)", "MODEL_X2")]
    public void ParseHuaweiEquipmentModelFromBuildVersion_ExtractsOnlyTheLeadingModelCode(string build, string expected)
    {
        var fact = FastbootOutputParser.ParseHuaweiEquipmentModelFromBuildVersion(build);

        Assert.Equal("Equipment model", fact.Name);
        Assert.Equal(expected, fact.Value);
        Assert.Equal("Derived", fact.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not a version")]
    [InlineData("123 10.0.0")]
    public void ParseHuaweiEquipmentModelFromBuildVersion_DoesNotGuessFromUnrecognizedValues(string build)
    {
        var fact = FastbootOutputParser.ParseHuaweiEquipmentModelFromBuildVersion(build);

        Assert.Equal(string.Empty, fact.Value);
        Assert.Equal("NotReported", fact.Status);
    }

    [Fact]
    public void ParseHuaweiOemRead_ExtractsOnlyLockStateFields()
    {
        var fact = FastbootOutputParser.ParseHuaweiOemRead(
            "Boot lock state", "(bootloader) FB LockState: LOCKED\n(bootloader) USER LockState: UNLOCKED\nOKAY", 0);

        Assert.Equal("FB: LOCKED; USER: UNLOCKED", fact.Value);
        Assert.Equal("Success", fact.Status);
    }

    [Fact]
    public void ParseHuaweiOemRead_ContinuesWhenTheDeviceDoesNotImplementAnAllowlistedQuery()
    {
        var fact = FastbootOutputParser.ParseHuaweiOemRead("Base version", "FAILED (unknown command)", 1);

        Assert.Equal(string.Empty, fact.Value);
        Assert.Equal("NotReported", fact.Status);
    }

    [Fact]
    public void ParseHuaweiOemRead_RejectsCommandsOutsideTheFixedReadAllowlist()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            FastbootOutputParser.ParseHuaweiOemRead("Dongle Info", "RSA-2048", 0));
    }

    [Fact]
    public void HuaweiOemReadCommands_ContainOnlyTheFixedReadQueries()
    {
        var commands = FastbootOutputParser.GetHuaweiOemReadCommands()
            .Select(command => string.Join(' ', command.Arguments))
            .ToArray();

        Assert.Contains("oem oeminforead-BASE_VERSION", commands);
        Assert.Contains("oem oeminforead-CUSTOM_VERSION", commands);
        Assert.Contains("oem oeminforead-PRELOAD_VERSION", commands);
        Assert.DoesNotContain(commands, command => command.Contains("PSID", StringComparison.OrdinalIgnoreCase)
            || command.Contains("hotainfo", StringComparison.OrdinalIgnoreCase)
            || command.Contains("flash", StringComparison.OrdinalIgnoreCase)
            || command.Contains("unlock", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("oem get-product-model")]
    [InlineData("rescue_phoneinfo")]
    [InlineData("serialno")]
    public void ParseHuaweiVariable_RejectsOemAndIdentifyingValues(string name)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FastbootOutputParser.ParseHuaweiVariable(name, "", 0));
    }

    [Fact]
    public void ParseVariable_ReportsTransportFailuresInsteadOfCallingThemUnsupported()
    {
        Assert.Throws<InvalidOperationException>(() => FastbootOutputParser.ParseVariable("product", "FAILED (device disconnected)", 1));
    }

    [Fact]
    public void ParseVariable_DoesNotTreatSuccessfulProcessExitAsProofThatFastbootReadSucceeded()
    {
        Assert.Throws<InvalidOperationException>(() => FastbootOutputParser.ParseVariable("product", "FAILED (device disconnected)", 0));
    }

    [Fact]
    public void ParseVariable_ReportsVendorPermissionDenialWithoutAbortingOtherVariableReads()
    {
        var fact = FastbootOutputParser.ParseVariable("current-slot", "FAILED (remote: 'Command not allowed')", 1);

        Assert.Equal("PermissionDenied", fact.Status);
        Assert.Equal(string.Empty, fact.Value);
    }
}
