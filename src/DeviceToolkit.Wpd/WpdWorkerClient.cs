using System.Diagnostics;
using System.Text.Json;

namespace DeviceToolkit.Wpd;

internal sealed record WpdWorkerRequest(string Operation, string? PnpDeviceId = null, string? FriendlyName = null,
    string? Manufacturer = null, string? Description = null, string? Path = null, string? Destination = null);
internal sealed record WpdWorkerDevice(string? PnpDeviceId, string FriendlyName, string Manufacturer, string Description);
internal sealed record WpdWorkerStorageDetails(string Name, long? TotalBytes, long? FreeBytes, string? FileSystem);
internal sealed record WpdWorkerDeviceDetails(string? Model, string? Name, string? FirmwareVersion, string? PowerLevel,
    string? PowerSource, string? Protocol, string? DeviceDateTime, IReadOnlyList<WpdWorkerStorageDetails>? Storage);
internal sealed record WpdWorkerEntry(string Name, bool IsDirectory, ulong? SizeBytes);
internal sealed record WpdWorkerResponse(bool Success, string? Error, IReadOnlyList<WpdWorkerDevice>? Devices = null,
    IReadOnlyList<WpdWorkerEntry>? Entries = null, ulong? SizeBytes = null, WpdWorkerDeviceDetails? Details = null);

internal sealed class WpdWorkerClient
{
    static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    readonly string _workerAssemblyPath;
    readonly TimeSpan _operationTimeout;
    readonly TimeSpan _connectionTimeout;

    public WpdWorkerClient(string workerAssemblyPath, TimeSpan operationTimeout, TimeSpan connectionTimeout)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workerAssemblyPath);
        if (!File.Exists(workerAssemblyPath))
            throw new FileNotFoundException("The isolated WPD worker assembly was not found.", workerAssemblyPath);
        if (operationTimeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(operationTimeout));
        if (connectionTimeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(connectionTimeout));
        _workerAssemblyPath = Path.GetFullPath(workerAssemblyPath);
        _operationTimeout = operationTimeout;
        _connectionTimeout = connectionTimeout;
    }

    public async Task<IReadOnlyList<WpdWorkerDevice>> EnumerateDevicesAsync(CancellationToken cancellationToken)
    {
        using var process = StartProcess();
        var outputTask = process.StandardOutput.ReadLineAsync();
        var errorTask = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(_connectionTimeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
        try
        {
            await process.StandardInput.WriteLineAsync(Serialize(new WpdWorkerRequest("enumerate")))
                .WaitAsync(linked.Token).ConfigureAwait(false);
            process.StandardInput.Close();
            await process.WaitForExitAsync(linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            KillAndWait(process);
            if (cancellationToken.IsCancellationRequested) throw;
            throw CreateTimeoutException(_connectionTimeout);
        }

        var line = await outputTask.ConfigureAwait(false);
        _ = await errorTask.ConfigureAwait(false);
        var response = Deserialize(line);
        EnsureSuccess(response);
        return response.Devices ?? [];
    }

    public async Task<WpdWorkerProcess> OpenSessionAsync(WpdWorkerDevice device, CancellationToken cancellationToken)
    {
        var process = StartProcess();
        var worker = new WpdWorkerProcess(process, _operationTimeout);
        try
        {
            await worker.ExecuteAsync(CreateRequest("connect", device), cancellationToken, _connectionTimeout).ConfigureAwait(false);
            return worker;
        }
        catch
        {
            worker.Dispose();
            throw;
        }
    }

    internal static WpdWorkerRequest CreateRequest(
        string operation,
        WpdWorkerDevice device,
        string? path = null,
        string? destination = null) =>
        new(operation, device.PnpDeviceId, device.FriendlyName, device.Manufacturer, device.Description, path, destination);

    Process StartProcess()
    {
        var startInfo = new ProcessStartInfo("dotnet")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.GetDirectoryName(_workerAssemblyPath)!
        };
        startInfo.ArgumentList.Add(_workerAssemblyPath);
        var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        if (!process.Start())
        {
            process.Dispose();
            throw new InvalidOperationException("The WPD worker process did not start.");
        }
        return process;
    }

    static TimeoutException CreateTimeoutException(TimeSpan timeout) =>
        new($"The isolated WPD operation exceeded {timeout.TotalSeconds:0} seconds.");

    static string Serialize(WpdWorkerRequest request) => JsonSerializer.Serialize(request, JsonOptions);

    static WpdWorkerResponse Deserialize(string? line) =>
        string.IsNullOrWhiteSpace(line)
            ? throw new InvalidDataException("The WPD worker returned no response.")
            : JsonSerializer.Deserialize<WpdWorkerResponse>(line, JsonOptions)
                ?? throw new InvalidDataException("The WPD worker response was invalid.");

    static void EnsureSuccess(WpdWorkerResponse response)
    {
        if (!response.Success)
            throw new InvalidOperationException($"The isolated WPD operation failed: {response.Error ?? "unknown error"}.");
    }

    static void KillAndWait(Process process)
    {
        try
        {
            if (!process.HasExited) process.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException) { }
        catch (System.ComponentModel.Win32Exception) { }
        try { process.WaitForExit(5000); } catch (InvalidOperationException) { }
    }

    internal sealed class WpdWorkerProcess : IDisposable
    {
        readonly Process _process;
        readonly SemaphoreSlim _requestGate = new(1, 1);
        readonly TimeSpan _operationTimeout;
        int _stopped;

        internal WpdWorkerProcess(Process process, TimeSpan operationTimeout)
        {
            _process = process;
            _operationTimeout = operationTimeout;
        }

        internal async Task<WpdWorkerResponse> ExecuteAsync(
            WpdWorkerRequest request,
            CancellationToken cancellationToken,
            TimeSpan? timeoutOverride = null)
        {
            var timeoutDuration = timeoutOverride ?? _operationTimeout;
            if (timeoutDuration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeoutOverride));
            await _requestGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (Volatile.Read(ref _stopped) != 0 || _process.HasExited)
                    throw new InvalidOperationException("The isolated WPD worker has stopped.");

                using var timeout = new CancellationTokenSource(timeoutDuration);
                using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
                try
                {
                    await _process.StandardInput.WriteLineAsync(Serialize(request)).WaitAsync(linked.Token).ConfigureAwait(false);
                    var line = await _process.StandardOutput.ReadLineAsync().WaitAsync(linked.Token).ConfigureAwait(false);
                    var response = Deserialize(line);
                    EnsureSuccess(response);
                    return response;
                }
                catch (OperationCanceledException)
                {
                    StopProcess();
                    if (cancellationToken.IsCancellationRequested) throw;
                    throw new TimeoutException($"The isolated WPD operation exceeded {timeoutDuration.TotalSeconds:0} seconds.");
                }
                catch
                {
                    if (_process.HasExited) StopProcess();
                    throw;
                }
            }
            finally
            {
                _requestGate.Release();
            }
        }

        public void Dispose() => StopProcess();

        void StopProcess()
        {
            if (Interlocked.Exchange(ref _stopped, 1) != 0) return;
            KillAndWait(_process);
            _process.Dispose();
        }
    }
}
