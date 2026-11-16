using System.Text.Json;

namespace DeviceToolkit.Wpd.FakeWorker;

internal sealed record Request(string Operation, string? PnpDeviceId = null, string? FriendlyName = null,
    string? Manufacturer = null, string? Description = null, string? Path = null, string? Destination = null);
internal sealed record DeviceRow(string? PnpDeviceId, string FriendlyName, string Manufacturer, string Description);
internal sealed record EntryRow(string Name, bool IsDirectory, ulong? SizeBytes);
internal sealed record Result(bool Success, string? Error, IReadOnlyList<DeviceRow>? Devices = null,
    IReadOnlyList<EntryRow>? Entries = null, ulong? SizeBytes = null);

internal static class Program
{
    static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    static int Main()
    {
        var logPath = Environment.GetEnvironmentVariable("DEVICE_TOOLKIT_WPD_FAKE_LOG");
        if (string.IsNullOrWhiteSpace(logPath)) return 2;

        var first = Console.In.ReadLine();
        if (string.IsNullOrWhiteSpace(first)) return 3;
        var request = Deserialize(first);
        Record(logPath, request);

        if (request.Operation == "enumerate")
        {
            Write(new Result(true, null,
            [new DeviceRow("fixture-pnp-id", "WPD worker lifecycle fixture", "DeviceToolkit", "test device")]));
            return 0;
        }

        if (request.Operation != "connect") return 4;
        Write(new Result(true, null));

        string? line;
        while ((line = Console.In.ReadLine()) is not null)
        {
            request = Deserialize(line);
            Record(logPath, request);
            if (request.Operation == "shutdown")
            {
                Write(new Result(true, null));
                return 0;
            }

            if (request.Operation == "directories")
            {
                Write(new Result(true, null, Entries:
                [
                    new EntryRow("slow", true, null),
                    new EntryRow("normal", true, null),
                    new EntryRow("مجلد تجربة", true, null)
                ]));
                continue;
            }

            if (request.Operation == "files" && request.Path == "slow")
                Thread.Sleep(Timeout.Infinite);

            if (request.Operation == "files")
            {
                var entries = request.Path == "مجلد تجربة"
                    ? new[] { new EntryRow("fixture-large.bin", false, 16UL * 1024 * 1024) }
                    : new[] { new EntryRow("sample.txt", false, 5) };
                Write(new Result(true, null, Entries: entries));
                continue;
            }

            if (request.Operation == "download" && !string.IsNullOrWhiteSpace(request.Destination))
            {
                const ulong size = 16UL * 1024 * 1024;
                using (var output = new FileStream(request.Destination, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    output.SetLength(checked((long)size));
                Write(new Result(true, null, SizeBytes: size));
                continue;
            }

            Write(new Result(false, "unsupported operation"));
        }

        return 0;
    }

    static Request Deserialize(string line) =>
        JsonSerializer.Deserialize<Request>(line, JsonOptions) ?? throw new InvalidDataException("Empty request.");

    static void Record(string path, Request request) =>
        File.AppendAllText(path, $"{Environment.ProcessId}|{request.Operation}|{request.Path ?? string.Empty}{Environment.NewLine}");

    static void Write(Result result)
    {
        Console.Out.WriteLine(JsonSerializer.Serialize(result, JsonOptions));
        Console.Out.Flush();
    }
}
