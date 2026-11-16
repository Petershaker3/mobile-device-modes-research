using System.Text.Json;
using MediaDevices;

namespace DeviceToolkit.Wpd.Worker;

internal sealed record Request(string Operation, string? PnpDeviceId, string? FriendlyName, string? Manufacturer,
    string? Description, string? Path, string? Destination);
internal sealed record DeviceRow(string? PnpDeviceId, string FriendlyName, string Manufacturer, string Description);
internal sealed record StorageDetails(string Name, long? TotalBytes, long? FreeBytes, string? FileSystem);
internal sealed record DeviceDetails(string? Model, string? Name, string? FirmwareVersion, string? PowerLevel,
    string? PowerSource, string? Protocol, string? DeviceDateTime, IReadOnlyList<StorageDetails>? Storage);
internal sealed record EntryRow(string Name, bool IsDirectory, ulong? SizeBytes);
internal sealed record Result(bool Success, string? Error, IReadOnlyList<DeviceRow>? Devices = null,
    IReadOnlyList<EntryRow>? Entries = null, ulong? SizeBytes = null, DeviceDetails? Details = null);

internal static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [STAThread]
    private static int Main()
    {
        if (!OperatingSystem.IsWindows()) return 2;

        try
        {
            var firstLine = Console.In.ReadLine();
            if (string.IsNullOrWhiteSpace(firstLine)) throw new InvalidDataException("Worker request is empty.");
            var firstRequest = Deserialize(firstLine);
            if (firstRequest.Operation == "enumerate")
            {
                Write(EnumerateDevices());
                return 0;
            }

            var selected = SelectDevice(firstRequest);
            try
            {
                selected.Connect(MediaDeviceAccess.GenericRead, MediaDeviceShare.Read);
                Write(new Result(true, null));

                string? line;
                while ((line = Console.In.ReadLine()) is not null)
                {
                    var request = Deserialize(line);
                    if (request.Operation == "shutdown")
                    {
                        Write(new Result(true, null));
                        return 0;
                    }

                    try
                    {
                        Write(ExecuteConnected(selected, request));
                    }
                    catch (Exception ex)
                    {
                        Write(new Result(false, ex.GetType().Name));
                    }
                }

                return 0;
            }
            finally
            {
                selected.Dispose();
            }
        }
        catch (Exception ex)
        {
            Write(new Result(false, ex.GetType().Name));
            return 1;
        }
    }

    private static Result EnumerateDevices()
    {
        var devices = MediaDeviceManager.Instance.GetDevices()?.ToArray() ?? [];
        try
        {
            return new Result(true, null, Devices: devices.Select(device => new DeviceRow(
                device.PnPDeviceID,
                device.FriendlyName ?? string.Empty,
                device.Manufacturer ?? string.Empty,
                device.Description ?? string.Empty)).ToArray());
        }
        finally
        {
            foreach (var device in devices) device.Dispose();
        }
    }

    private static MediaDevice SelectDevice(Request request)
    {
        var targets = MediaDeviceManager.Instance.GetDevices()?.ToArray() ?? [];
        var matches = targets.Where(device => MatchesSelection(device, request)).ToArray();
        var selected = matches.Length == 1 ? matches[0] : null;
        foreach (var device in targets)
            if (!ReferenceEquals(device, selected)) device.Dispose();

        if (selected is not null) return selected;
        if (matches.Length > 1) throw new InvalidOperationException("The selected WPD identity is ambiguous; refusing to connect.");
        throw new InvalidOperationException("The selected WPD device is no longer available.");
    }

    private static bool MatchesSelection(MediaDevice device, Request request)
    {
        if (!string.IsNullOrWhiteSpace(request.PnpDeviceId))
            return string.Equals(device.PnPDeviceID, request.PnpDeviceId, StringComparison.Ordinal);

        return !string.IsNullOrWhiteSpace(request.FriendlyName)
            && string.Equals(device.FriendlyName, request.FriendlyName, StringComparison.Ordinal)
            && string.Equals(device.Manufacturer ?? string.Empty, request.Manufacturer ?? string.Empty, StringComparison.Ordinal)
            && string.Equals(device.Description ?? string.Empty, request.Description ?? string.Empty, StringComparison.Ordinal);
    }

    private static Result ExecuteConnected(MediaDevice device, Request request)
    {
        if (request.Operation == "details")
            return new Result(true, null, Details: new DeviceDetails(
                ReadOptional(() => device.Model),
                ReadOptional(() => device.Name),
                ReadOptional(() => device.FirmwareVersion),
                ReadOptional(() => device.PowerLevel?.ToString()),
                ReadOptional(() => device.PowerSource?.ToString()),
                ReadOptional(() => device.Protocol),
                ReadOptional(() => device.DateTime?.ToString("O", System.Globalization.CultureInfo.InvariantCulture)),
                ReadStorage(device)));

        var path = ValidateDevicePath(request.Path);
        if (request.Operation is "directories" or "files")
        {
            var directory = string.IsNullOrEmpty(path)
                ? device.GetRootDirectory()
                : device.GetDirectoryInfoAsync(path, CancellationToken.None).GetAwaiter().GetResult();

            if (request.Operation == "directories")
                return new Result(true, null, Entries: directory.EnumerateDirectories()
                    .Select(entry => new EntryRow(entry.Name, true, null)).ToArray());

            return new Result(true, null, Entries: directory.EnumerateFiles()
                .Select(entry => new EntryRow(entry.Name, false, entry.Length)).ToArray());
        }

        if (request.Operation == "download")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Destination);
            var file = device.GetFileInfoAsync(path, CancellationToken.None).GetAwaiter().GetResult();
            file.CopyToAsync(request.Destination, overwrite: false, CancellationToken.None).GetAwaiter().GetResult();
            return new Result(true, null, SizeBytes: file.Length);
        }

        throw new NotSupportedException("The requested worker operation is not supported.");
    }

    private static IReadOnlyList<StorageDetails> ReadStorage(MediaDevice device)
    {
        try
        {
            return device.GetDrives().Select((drive, index) => new StorageDetails(
                $"Storage {index + 1}",
                ReadOptionalInt64(() => drive.TotalSize),
                ReadOptionalInt64(() => drive.AvailableFreeSpace),
                ReadOptional(() => drive.DriveFormat)))
                .ToArray();
        }
        catch
        {
            return [];
        }
    }

    private static long? ReadOptionalInt64(Func<long> read)
    {
        try { return read(); }
        catch { return null; }
    }

    private static string? ReadOptional(Func<object?> read)
    {
        try
        {
            var value = read();
            var text = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)?.Trim();
            return string.IsNullOrEmpty(text) ? null : text;
        }
        catch
        {
            return null;
        }
    }

    private static string ValidateDevicePath(string? requestedPath)
    {
        var path = requestedPath?.Trim('/') ?? string.Empty;
        if (path.Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Any(segment => segment is "." or ".." || segment.IndexOfAny(['\\', '\0']) >= 0))
            throw new InvalidDataException("The device path is invalid.");
        return path;
    }

    private static Request Deserialize(string line) =>
        JsonSerializer.Deserialize<Request>(line, JsonOptions) ?? throw new InvalidDataException("Worker request is invalid.");

    private static void Write(Result result)
    {
        Console.Out.WriteLine(JsonSerializer.Serialize(result, JsonOptions));
        Console.Out.Flush();
    }
}
