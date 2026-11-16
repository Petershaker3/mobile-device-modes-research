using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.Models;

namespace logcat.Services;

/// <summary>
/// Read-only device discovery for the product UI. The transplanted
/// AdbManager.ListDevicesAsync also probes root with `su -c id`, which is
/// explicitly outside this product's policy. This adapter only enumerates ADB
/// state and basic model metadata, and never sends a command to the device.
/// </summary>
public sealed class SafeDeviceDiscovery
{
    readonly Func<IEnumerable<DeviceData>> _getDevices;

    public SafeDeviceDiscovery() : this(() => new AdbClient().GetDevices()) { }

    internal SafeDeviceDiscovery(Func<IEnumerable<DeviceData>> getDevices)
        => _getDevices = getDevices ?? throw new ArgumentNullException(nameof(getDevices));

    public IReadOnlyList<ConnectedDevice> ListDevices()
        => _getDevices()
            .Select(device => new ConnectedDevice(
                device.Serial ?? string.Empty,
                string.IsNullOrWhiteSpace(device.Model)
                    ? "Android device"
                    : device.Model.Replace('_', ' '),
                device.State.ToString()))
            .Where(device => !string.IsNullOrWhiteSpace(device.Serial))
            .ToArray();
}

public sealed record ConnectedDevice(string Serial, string Model, string State)
{
    public string DisplayName => $"{Model} ({Serial}) [{State}]";
}
