using System.Diagnostics;
using DeviceToolkit.Wpd;

var fakeWorker = Path.Combine(AppContext.BaseDirectory, "FakeWorker", "DeviceToolkit.Wpd.FakeWorker.dll");
if (!File.Exists(fakeWorker)) throw new FileNotFoundException("The fake worker fixture was not copied.", fakeWorker);

var testRoot = Path.Combine(Path.GetTempPath(), $"DeviceToolkit-WpdSessionSmoke-{Guid.NewGuid():N}");
Directory.CreateDirectory(testRoot);
var smokeSucceeded = false;
try
{
    await VerifyLongLivedSessionAndTimeoutAsync(fakeWorker, Path.Combine(testRoot, "timeout.log"));
    await VerifyCancellationStopsWorkerAsync(fakeWorker, Path.Combine(testRoot, "cancel.log"));
    await VerifySessionDisposalStopsWorkerAsync(fakeWorker, Path.Combine(testRoot, "dispose.log"));
    Console.WriteLine("WPD fake-worker smoke passed: one process per session; timeout, cancellation, and session disposal stopped the worker.");
    smokeSucceeded = true;
}
finally
{
    Environment.SetEnvironmentVariable("DEVICE_TOOLKIT_WPD_FAKE_LOG", null);
    if (smokeSucceeded)
    {
        try { Directory.Delete(testRoot, recursive: true); } catch { }
    }
    else
    {
        Console.Error.WriteLine($"WPD fake-worker diagnostics retained at {testRoot}");
    }
}

static async Task VerifyLongLivedSessionAndTimeoutAsync(string fakeWorker, string logPath)
{
    Environment.SetEnvironmentVariable("DEVICE_TOOLKIT_WPD_FAKE_LOG", logPath);
    using var provider = new WpdDeviceFileProvider(fakeWorker, TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(15));
    var device = (await provider.EnumerateDevicesAsync()).Single();
    using var session = await provider.OpenReadOnlyAsync(device.SelectionId);
    var directories = await session.ListDirectoriesAsync(session.RootDirectoryId);
    _ = await session.ListFilesAsync(session.RootDirectoryId);
    var unicodeDirectory = directories.Single(entry => entry.Name == "مجلد تجربة");
    var largeFile = (await session.ListFilesAsync(unicodeDirectory.EntryId)).Single();
    var downloadPath = Path.Combine(Path.GetDirectoryName(logPath)!, "fixture-large.bin");
    var download = await session.DownloadFileAsync(largeFile.EntryId, downloadPath);
    Ensure(download.SizeBytes == 16UL * 1024 * 1024 && new FileInfo(downloadPath).Length == (long)download.SizeBytes,
        "The Unicode path or large download size did not survive the worker protocol.");
    File.Delete(downloadPath);
    var slowDirectory = directories.Single(entry => entry.Name == "slow");

    await ExpectAsync<TimeoutException>(() => session.ListFilesAsync(slowDirectory.EntryId));
    await ExpectAsync<InvalidOperationException>(() => session.ListFilesAsync(session.RootDirectoryId));

    var rows = ReadWorkerRows(logPath);
    var sessionRows = rows.Where(row => row.Operation is "connect" or "directories" or "files" or "download").ToArray();
    Ensure(sessionRows.Select(row => row.ProcessId).Distinct().Count() == 1,
        "The WPD session did not reuse one worker process for its requests.");
    Ensure(sessionRows.Any(row => row.Operation == "files" && row.Path == "slow"),
        "The worker did not receive the delayed files request.");
    Ensure(sessionRows.Any(row => row.Operation == "download" && row.Path == "مجلد تجربة/fixture-large.bin"),
        "The worker did not preserve the Unicode path during the download request.");
    Ensure(sessionRows.All(row => HasExited(row.ProcessId)),
        "The timed-out WPD worker process is still running.");
}

static async Task VerifyCancellationStopsWorkerAsync(string fakeWorker, string logPath)
{
    Environment.SetEnvironmentVariable("DEVICE_TOOLKIT_WPD_FAKE_LOG", logPath);
    using var provider = new WpdDeviceFileProvider(fakeWorker, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15));
    var device = (await provider.EnumerateDevicesAsync()).Single();
    using var session = await provider.OpenReadOnlyAsync(device.SelectionId);
    var slowDirectory = (await session.ListDirectoriesAsync(session.RootDirectoryId))
        .Single(entry => entry.Name == "slow");

    using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));
    await ExpectAsync<OperationCanceledException>(() =>
        session.ListFilesAsync(slowDirectory.EntryId, cancellation.Token));

    var rows = ReadWorkerRows(logPath);
    var sessionRows = rows.Where(row => row.Operation is "connect" or "directories" or "files").ToArray();
    Ensure(sessionRows.Select(row => row.ProcessId).Distinct().Count() == 1,
        "The cancellation scenario did not keep a single worker for its session.");
    Ensure(sessionRows.All(row => HasExited(row.ProcessId)),
        "The cancelled WPD worker process is still running.");
}

static async Task VerifySessionDisposalStopsWorkerAsync(string fakeWorker, string logPath)
{
    Environment.SetEnvironmentVariable("DEVICE_TOOLKIT_WPD_FAKE_LOG", logPath);
    using var provider = new WpdDeviceFileProvider(fakeWorker, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15));
    var device = (await provider.EnumerateDevicesAsync()).Single();
    var session = await provider.OpenReadOnlyAsync(device.SelectionId);
    var slowDirectory = (await session.ListDirectoriesAsync(session.RootDirectoryId))
        .Single(entry => entry.Name == "slow");
    _ = await session.ListFilesAsync(session.RootDirectoryId);
    var pendingOperation = session.ListFilesAsync(slowDirectory.EntryId);
    await WaitUntilAsync(
        () => ReadWorkerRows(logPath).Any(row => row.Operation == "files" && row.Path == "slow"),
        TimeSpan.FromSeconds(5));

    session.Dispose();
    await ExpectAsync<OperationCanceledException>(() => pendingOperation);

    var rows = ReadWorkerRows(logPath);
    var sessionRows = rows.Where(row => row.Operation is "connect" or "directories" or "files").ToArray();
    Ensure(sessionRows.Select(row => row.ProcessId).Distinct().Count() == 1,
        "The session-disposal scenario did not keep a single worker for its session.");
    Ensure(sessionRows.All(row => HasExited(row.ProcessId)),
        "The WPD worker process remained active after session disposal.");
}

static (int ProcessId, string Operation, string Path)[] ReadWorkerRows(string path) =>
    File.ReadLines(path).Select(line =>
    {
        var columns = line.Split('|', 3);
        return columns.Length == 3
            ? (int.Parse(columns[0]), columns[1], columns[2])
            : throw new InvalidDataException("The fake worker log row was malformed.");
    }).ToArray();

static bool HasExited(int processId)
{
    try
    {
        using var process = Process.GetProcessById(processId);
        return process.HasExited;
    }
    catch (ArgumentException) { return true; }
}

static async Task ExpectAsync<TException>(Func<Task> action) where TException : Exception
{
    try { await action(); }
    catch (TException) { return; }
    throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
}

static async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
{
    var stopwatch = Stopwatch.StartNew();
    while (!condition())
    {
        if (stopwatch.Elapsed >= timeout)
            throw new TimeoutException("The fake worker did not reach the expected operation.");
        await Task.Delay(TimeSpan.FromMilliseconds(20));
    }
}

static void Ensure(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
