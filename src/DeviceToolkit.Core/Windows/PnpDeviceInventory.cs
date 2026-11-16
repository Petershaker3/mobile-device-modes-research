using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.Devices.DeviceAndDriverInstallation;
using Windows.Win32.Foundation;

namespace DeviceToolkit.Core.Windows;

/// <summary>Read-only snapshot of a currently present Windows Plug and Play device.</summary>
public sealed record PnpDeviceSnapshot(
    string Name,
    string InstanceId,
    IReadOnlyList<string> HardwareIds,
    uint? StatusFlags,
    uint? ProblemCode)
{
    public string HardwareIdsText => HardwareIds.Count == 0 ? "-" : string.Join(", ", HardwareIds);
    public string StatusFlagsText => StatusFlags is { } flags ? $"0x{flags:X8}" : "Unavailable";
    public bool HasProblem => ProblemCode is > 0;
    public string ProblemCodeText => ProblemCode switch
    {
        > 0 => ProblemCode.Value.ToString(System.Globalization.CultureInfo.InvariantCulture),
        0 => "0",
        _ => "Unavailable"
    };
}

/// <summary>Enumerates present PnP devices using SetupAPI and Configuration Manager.</summary>
[SupportedOSPlatform("windows5.0")]
public sealed class PnpDeviceInventory
{
    private const uint DigcfPresent = 0x00000002;
    private const uint DigcfAllClasses = 0x00000004;
    private const uint ErrorInsufficientBuffer = 122;
    private const uint CrSuccess = 0;

    public IReadOnlyList<PnpDeviceSnapshot> EnumeratePresentDevices()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(5))
            throw new PlatformNotSupportedException("PnP inventory is available only on Windows.");

        using SafeHandle deviceSet = PInvoke.SetupDiGetClassDevs(
            ClassGuid: null,
            Enumerator: null,
            hwndParent: default,
            Flags: (SETUP_DI_GET_CLASS_DEVS_FLAGS)(DigcfPresent | DigcfAllClasses));

        if (deviceSet.IsInvalid)
            throw new Win32Exception(Marshal.GetLastWin32Error(), "SetupDiGetClassDevs failed.");

        var devices = new List<PnpDeviceSnapshot>();
        uint index = 0;
        while (true)
        {
            var info = new SP_DEVINFO_DATA { cbSize = (uint)Marshal.SizeOf<SP_DEVINFO_DATA>() };
            if (!PInvoke.SetupDiEnumDeviceInfo(deviceSet, index++, ref info))
            {
                int error = Marshal.GetLastWin32Error();
                if (error == 259) // ERROR_NO_MORE_ITEMS
                    break;
                throw new Win32Exception(error, "SetupDiEnumDeviceInfo failed.");
            }

            string instanceId = ReadInstanceId(deviceSet, in info);
            string name = ReadStringProperty(deviceSet, in info, SETUP_DI_REGISTRY_PROPERTY.SPDRP_FRIENDLYNAME)
                ?? ReadStringProperty(deviceSet, in info, SETUP_DI_REGISTRY_PROPERTY.SPDRP_DEVICEDESC)
                ?? instanceId;
            IReadOnlyList<string> hardwareIds = ReadHardwareIds(deviceSet, in info);
            uint? statusFlags = null;
            uint? problemCode = null;
            CONFIGRET statusResult = PInvoke.CM_Get_DevNode_Status(
                out CM_DEVNODE_STATUS_FLAGS flags,
                out CM_PROB problem,
                info.DevInst,
                0);
            if (statusResult == (CONFIGRET)CrSuccess)
            {
                statusFlags = (uint)flags;
                problemCode = (uint)problem;
            }

            devices.Add(new PnpDeviceSnapshot(name, instanceId, hardwareIds, statusFlags, problemCode));
        }

        return devices;
    }

    [SupportedOSPlatform("windows5.0")]
    private static string ReadInstanceId(SafeHandle deviceSet, in SP_DEVINFO_DATA info)
    {
        PInvoke.SetupDiGetDeviceInstanceId(deviceSet, in info, RequiredSize: out uint requiredSize);
        int sizeError = Marshal.GetLastWin32Error();
        if (requiredSize == 0)
            throw new Win32Exception(sizeError, "Could not determine the device instance ID size.");

        Span<char> buffer = new char[checked((int)requiredSize)];
        if (!PInvoke.SetupDiGetDeviceInstanceId(deviceSet, in info, buffer))
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not read the device instance ID.");
        int end = buffer.IndexOf('\0');
        return new string(end < 0 ? buffer : buffer[..end]);
    }

    [SupportedOSPlatform("windows5.0")]
    private static IReadOnlyList<string> ReadHardwareIds(SafeHandle deviceSet, in SP_DEVINFO_DATA info)
    {
        PInvoke.SetupDiGetDeviceRegistryProperty(
            deviceSet, in info, SETUP_DI_REGISTRY_PROPERTY.SPDRP_HARDWAREID,
            out _, Span<byte>.Empty, out uint requiredSize);
        int error = Marshal.GetLastWin32Error();
        if (requiredSize == 0)
        {
            // Some devices do not expose a hardware ID through this property.
            if (error == 13) // ERROR_INVALID_DATA
                return Array.Empty<string>();
            if (error != 0)
                throw new Win32Exception(error, "Could not determine the device hardware ID size.");
            return Array.Empty<string>();
        }
        if (error != ErrorInsufficientBuffer && error != 0)
            throw new Win32Exception(error, "Could not determine the device hardware ID size.");

        Span<byte> bytes = new byte[checked((int)requiredSize)];
        if (!PInvoke.SetupDiGetDeviceRegistryProperty(
                deviceSet, in info, SETUP_DI_REGISTRY_PROPERTY.SPDRP_HARDWAREID, out _, bytes, out _))
        {
            error = Marshal.GetLastWin32Error();
            if (error is 13 or 87)
                return Array.Empty<string>();
            throw new Win32Exception(error, "Could not read device hardware IDs.");
        }

        string multiString = System.Text.Encoding.Unicode.GetString(bytes);
        return multiString.Split('\0', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    [SupportedOSPlatform("windows5.0")]
    private static string? ReadStringProperty(
        SafeHandle deviceSet,
        in SP_DEVINFO_DATA info,
        SETUP_DI_REGISTRY_PROPERTY property)
    {
        PInvoke.SetupDiGetDeviceRegistryProperty(deviceSet, in info, property, out _, Span<byte>.Empty, out uint size);
        int error = Marshal.GetLastWin32Error();
        if (size == 0)
        {
            if (error is 0 or 13) // ERROR_INVALID_DATA means the optional property is absent.
                return null;
            if (error != ErrorInsufficientBuffer)
                throw new Win32Exception(error, "Could not determine the device display name size.");
        }

        Span<byte> bytes = new byte[checked((int)size)];
        if (!PInvoke.SetupDiGetDeviceRegistryProperty(deviceSet, in info, property, out _, bytes, out _))
        {
            error = Marshal.GetLastWin32Error();
            if (error == 13)
                return null;
            throw new Win32Exception(error, "Could not read the device display name.");
        }

        string text = System.Text.Encoding.Unicode.GetString(bytes).TrimEnd('\0');
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
