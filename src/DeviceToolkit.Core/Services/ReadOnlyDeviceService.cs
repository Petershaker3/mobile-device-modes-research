using System.Text.RegularExpressions;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.Models;
using logcat.Services;

namespace DeviceToolkit.Core.Services;

public sealed record DeviceFacts(
    string Model,
    string Manufacturer,
    string Brand,
    string Product,
    string Device,
    string Android,
    string Api,
    string BuildId,
    string BuildDisplay,
    string SecurityPatch,
    string Abi,
    string SupportedAbis,
    string BootMode,
    string Bootloader,
    string Baseband,
    string SamsungCsc,
    string SamsungBootloaderLocked,
    string SamsungVerifiedBootState,
    string SamsungWarrantyBit,
    string SamsungVerityMode,
    string SelinuxMode,
    PrivateDnsSettings PrivateDns);

public sealed record AppVersion(string VersionName, string VersionCode);

/// <summary>
/// Shared, read-only Android queries for desktop and local MCP hosts.
/// It never chooses a device and never performs a device or host write.
/// The caller supplies an explicit device and owns ADB runtime startup.
/// </summary>
public sealed class ReadOnlyDeviceService
{
    static readonly (string Key, string Property)[] PropertyCommands =
    [
        ("Model", "ro.product.model"),
        ("Manufacturer", "ro.product.manufacturer"),
        ("Brand", "ro.product.brand"),
        ("Product", "ro.product.name"),
        ("Device", "ro.product.device"),
        ("Android", "ro.build.version.release"),
        ("Api", "ro.build.version.sdk"),
        ("BuildId", "ro.build.id"),
        ("BuildDisplay", "ro.build.display.id"),
        ("SecurityPatch", "ro.build.version.security_patch"),
        ("Abi", "ro.product.cpu.abi"),
        ("SupportedAbis", "ro.product.cpu.abilist"),
        ("BootMode", "ro.bootmode"),
        ("Bootloader", "ro.bootloader"),
        ("Baseband", "gsm.version.baseband"),
        ("SamsungCsc", "ro.csc.sales_code"),
        ("SamsungBootSalesCode", "ro.boot.sales_code"),
        ("SamsungBootloaderLocked", "ro.boot.flash.locked"),
        ("SamsungVerifiedBootState", "ro.boot.verifiedbootstate"),
        ("SamsungWarrantyBit", "ro.boot.warranty_bit"),
        ("SamsungVerityMode", "ro.boot.veritymode")
    ];

    static readonly (string Key, string Command)[] PrivateDnsCommands =
    [
        ("PrivateDnsMode", "settings get global private_dns_mode"),
        ("PrivateDnsDefaultMode", "settings get global private_dns_default_mode"),
        ("PrivateDnsSpecifier", "settings get global private_dns_specifier")
    ];

    readonly AdbShell _shell;

    public ReadOnlyDeviceService() : this(new AdbShell())
    {
    }

    internal ReadOnlyDeviceService(AdbShell shell)
        => _shell = shell ?? throw new ArgumentNullException(nameof(shell));

    public async Task<DeviceFacts> ReadFactsAsync(ConnectedDevice device, CancellationToken ct)
    {
        ValidateDevice(device);
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var propertyOutput = await _shell.RunAsync(
            device.Serial,
            BuildAllowlistedPropertyCommand(),
            TimeSpan.FromSeconds(12),
            ct).ConfigureAwait(false);
        values = ParsePropertyOutput(propertyOutput);
        foreach (var (key, command) in PrivateDnsCommands)
        {
            ct.ThrowIfCancellationRequested();
            values[key] = (await _shell.RunAsync(device.Serial, command, TimeSpan.FromSeconds(8), ct).ConfigureAwait(false)).Trim();
        }
        var selinuxMode = (await _shell.RunAsync(device.Serial, "getenforce", TimeSpan.FromSeconds(8), ct).ConfigureAwait(false)).Trim();

        return new DeviceFacts(
            Value("Model"), Value("Manufacturer"), Value("Brand"), Value("Product"), Value("Device"),
            Value("Android"), Value("Api"), Value("BuildId"), Value("BuildDisplay"), Value("SecurityPatch"),
            Value("Abi"), Value("SupportedAbis"), Value("BootMode"), Value("Bootloader"), Value("Baseband"),
            FirstValue("SamsungCsc", "SamsungBootSalesCode"), Value("SamsungBootloaderLocked"), Value("SamsungVerifiedBootState"),
            Value("SamsungWarrantyBit"), Value("SamsungVerityMode"),
            string.IsNullOrWhiteSpace(selinuxMode) ? "Unavailable" : selinuxMode,
            PrivateDnsSettings.Parse(Optional("PrivateDnsMode"), Optional("PrivateDnsDefaultMode"), Optional("PrivateDnsSpecifier")));

        string Value(string key) => values.TryGetValue(key, out var value) && value.Length > 0 ? value : "Unavailable";
        string FirstValue(params string[] keys) => keys.Select(Value).FirstOrDefault(value => value != "Unavailable") ?? "Unavailable";
        string? Optional(string key) => values.TryGetValue(key, out var value) ? value : null;
    }

    internal static string BuildAllowlistedPropertyCommand()
        => string.Join("; ", PropertyCommands.Select(entry =>
            $"printf '%s=' '{entry.Property}'; getprop '{entry.Property}'"));

    internal static Dictionary<string, string> ParsePropertyOutput(string output)
    {
        var byProperty = PropertyCommands.ToDictionary(entry => entry.Property, entry => entry.Key, StringComparer.Ordinal);
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var line in output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = line.IndexOf('=');
            if (separator <= 0) continue;
            var property = line[..separator].Trim();
            if (byProperty.TryGetValue(property, out var key))
                values[key] = line[(separator + 1)..].Trim();
        }
        return values;
    }

    public async Task<IReadOnlyList<InstalledApp>> ReadInstalledAppsAsync(ConnectedDevice device, CancellationToken ct)
    {
        ValidateDevice(device);
        var client = new AdbClient();
        var target = GetOnlineTarget(client, device);

        // skipInit avoids the synchronous, uncancellable refresh in the constructor.
        var packageManager = new PackageManager(client, target, skipInit: true);
        await packageManager.RefreshPackagesAsync(ct).ConfigureAwait(false);
        var allPackages = new Dictionary<string, string>(packageManager.Packages, StringComparer.Ordinal);
        var userPackageManager = new PackageManager(client, target, arguments: ["-3"], skipInit: true);
        await userPackageManager.RefreshPackagesAsync(ct).ConfigureAwait(false);
        return InstalledAppProjection.FromPackagePaths(
            allPackages,
            userPackageManager.Packages.Keys.ToHashSet(StringComparer.Ordinal));
    }

    public async Task<AppVersion> ReadAppVersionAsync(ConnectedDevice device, InstalledApp app, CancellationToken ct)
    {
        ValidateDevice(device);
        ArgumentNullException.ThrowIfNull(app);
        if (!Regex.IsMatch(app.PackageId, "^[A-Za-z0-9_]+(\\.[A-Za-z0-9_]+)+$", RegexOptions.CultureInvariant))
            throw new InvalidOperationException("The selected package ID is not valid.");

        var client = new AdbClient();
        var target = GetOnlineTarget(client, device);
        var packageManager = new PackageManager(client, target, skipInit: true);
        var info = await packageManager.GetVersionInfoAsync(app.PackageId, ct).ConfigureAwait(false);
        return new AppVersion(info.VersionName, info.VersionCode.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    static DeviceData GetOnlineTarget(AdbClient client, ConnectedDevice device)
        => client.GetDevices().SingleOrDefault(candidate =>
            candidate.Serial == device.Serial && candidate.State == DeviceState.Online)
            ?? throw new InvalidOperationException("The selected device is no longer online.");

    static void ValidateDevice(ConnectedDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (string.IsNullOrWhiteSpace(device.Serial))
            throw new ArgumentException("A specific device serial is required.", nameof(device));
    }
}
