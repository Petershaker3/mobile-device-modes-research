namespace DeviceToolkit.Wpd;

/// <summary>A process-local selection token. It is not a PnP ID or persistent device_id.</summary>
public sealed record WpdDeviceInfo(Guid SelectionId, string FriendlyName, string Manufacturer, string Description)
{
    public string DisplayName => string.IsNullOrWhiteSpace(Manufacturer)
        ? FriendlyName
        : $"{FriendlyName} ({Manufacturer})";
}

/// <summary>A directory or regular file discovered through a read-only WPD session.</summary>
public sealed record WpdFileEntry(Guid EntryId, string Name, bool IsDirectory, ulong? SizeBytes);
public sealed record WpdStorageDetails(string Name, long? TotalBytes, long? FreeBytes, string FileSystem);
public sealed record WpdDeviceDetails(string Model, string Name, string FirmwareVersion, string PowerLevel,
    string PowerSource, string Protocol, string DeviceDateTime, IReadOnlyList<WpdStorageDetails> Storage);

public sealed record WpdDownloadResult(ulong SizeBytes, string LocalPath);

/// <summary>
/// Read-only Windows Portable Device access. Native WPD work runs in a helper process so
/// a blocked COM call can be stopped by the operation timeout or caller cancellation.
/// </summary>
public sealed class WpdDeviceFileProvider : IDisposable
{
    readonly object _gate = new();
    readonly WpdWorkerClient _worker;
    readonly Dictionary<Guid, WpdWorkerDevice> _available = [];
    bool _disposed;

    public WpdDeviceFileProvider(
        string? workerAssemblyPath = null,
        TimeSpan? operationTimeout = null,
        TimeSpan? connectionTimeout = null)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Windows Portable Devices are available only on Windows.");

        var defaultWorkerPath = Path.Combine(AppContext.BaseDirectory, "WpdWorker", "DeviceToolkit.Wpd.Worker.dll");
        _worker = new WpdWorkerClient(
            workerAssemblyPath ?? defaultWorkerPath,
            operationTimeout ?? TimeSpan.FromSeconds(30),
            connectionTimeout ?? TimeSpan.FromSeconds(30));
    }

    public async Task<IReadOnlyList<WpdDeviceInfo>> EnumerateDevicesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var devices = await _worker.EnumerateDevicesAsync(cancellationToken).ConfigureAwait(false);
        lock (_gate)
        {
            ThrowIfDisposed();
            _available.Clear();
            var result = new List<WpdDeviceInfo>(devices.Count);
            foreach (var device in devices)
            {
                var selectionId = Guid.NewGuid();
                _available.Add(selectionId, device);
                result.Add(new WpdDeviceInfo(selectionId, device.FriendlyName, device.Manufacturer, device.Description));
            }

            return result;
        }
    }

    public async Task<WpdReadOnlySession> OpenReadOnlyAsync(Guid selectionId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        WpdWorkerDevice selected;
        lock (_gate)
        {
            if (!_available.Remove(selectionId, out selected!))
                throw new InvalidOperationException("The selected WPD device is stale or was not returned by this provider.");
        }

        var worker = await _worker.OpenSessionAsync(selected, cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            return new WpdReadOnlySession(worker, selected);
        }
        catch
        {
            worker.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _available.Clear();
        }
    }

    void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(WpdDeviceFileProvider));
    }

}

public sealed class WpdReadOnlySession : IDisposable
{
    readonly WpdWorkerClient.WpdWorkerProcess _worker;
    readonly WpdWorkerDevice _device;
    readonly object _gate = new();
    readonly SemaphoreSlim _operationGate = new(1, 1);
    readonly CancellationTokenSource _disposeCts = new();
    readonly Dictionary<Guid, string> _directories = [];
    readonly Dictionary<Guid, string> _files = [];
    bool _disposed;
    bool _faulted;

    internal WpdReadOnlySession(WpdWorkerClient.WpdWorkerProcess worker, WpdWorkerDevice device)
    {
        _worker = worker;
        _device = device;
        RootDirectoryId = Guid.NewGuid();
        _directories.Add(RootDirectoryId, string.Empty);
    }

    public Guid RootDirectoryId { get; }

    public async Task<WpdDeviceDetails> ReadDeviceDetailsAsync(CancellationToken cancellationToken = default)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
        await _operationGate.WaitAsync(linked.Token).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            var response = await _worker.ExecuteAsync(
                WpdWorkerClient.CreateRequest("details", _device), linked.Token).ConfigureAwait(false);
            var details = response.Details ?? new WpdWorkerDeviceDetails(null, null, null, null, null, null, null, null);
            return new WpdDeviceDetails(
                MetadataValue(details.Model), MetadataValue(details.Name),
                MetadataValue(details.FirmwareVersion), MetadataValue(details.PowerLevel),
                MetadataValue(details.PowerSource), MetadataValue(details.Protocol),
                MetadataValue(details.DeviceDateTime), (details.Storage ?? [])
                    .Select(storage => new WpdStorageDetails(storage.Name, storage.TotalBytes, storage.FreeBytes,
                        MetadataValue(storage.FileSystem)))
                    .ToArray());
        }
        catch (TimeoutException) { MarkFaulted(); throw; }
        catch (OperationCanceledException) { MarkFaulted(); throw; }
        finally { _operationGate.Release(); }
    }

    public Task<IReadOnlyList<WpdFileEntry>> ListDirectoriesAsync(Guid directoryId, CancellationToken cancellationToken = default) =>
        ListEntriesAsync(directoryId, "directories", cancellationToken);

    public Task<IReadOnlyList<WpdFileEntry>> ListFilesAsync(Guid directoryId, CancellationToken cancellationToken = default) =>
        ListEntriesAsync(directoryId, "files", cancellationToken);

    async Task<IReadOnlyList<WpdFileEntry>> ListEntriesAsync(Guid directoryId, string operation, CancellationToken cancellationToken)
    {
        var path = GetDirectoryPath(directoryId);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
        await _operationGate.WaitAsync(linked.Token).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            var response = await _worker.ExecuteAsync(
                WpdWorkerClient.CreateRequest(operation, _device, path), linked.Token).ConfigureAwait(false);
            var entries = response.Entries ?? [];
            var projected = new List<WpdFileEntry>(entries.Count);
            lock (_gate)
            {
                ThrowIfDisposed();
                foreach (var entry in entries)
                {
                    var entryId = Guid.NewGuid();
                    var childPath = JoinDevicePath(path, entry.Name);
                    if (entry.IsDirectory) _directories.Add(entryId, childPath);
                    else _files.Add(entryId, childPath);
                    projected.Add(new WpdFileEntry(entryId, entry.Name, entry.IsDirectory, entry.SizeBytes));
                }
            }

            return projected;
        }
        catch (TimeoutException)
        {
            MarkFaulted();
            throw;
        }
        catch (OperationCanceledException)
        {
            MarkFaulted();
            throw;
        }
        finally
        {
            _operationGate.Release();
        }
    }

    public async Task<WpdDownloadResult> DownloadFileAsync(
        Guid entryId,
        string localDestination,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(localDestination);
        var devicePath = GetFilePath(entryId);

        var fullDestination = Path.GetFullPath(localDestination);
        if (File.Exists(fullDestination) || Directory.Exists(fullDestination))
            throw new IOException("The local destination already exists. Choose a new destination before downloading.");

        var parent = Path.GetDirectoryName(fullDestination)
            ?? throw new ArgumentException("The local destination must include a parent directory.", nameof(localDestination));
        if (!Directory.Exists(parent)) throw new DirectoryNotFoundException("The selected local destination folder no longer exists.");

        var stagingPath = Path.Combine(parent, $".device-toolkit-wpd-{Guid.NewGuid():N}.partial");
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
        await _operationGate.WaitAsync(linked.Token).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            var response = await _worker.ExecuteAsync(
                WpdWorkerClient.CreateRequest("download", _device, devicePath, stagingPath), linked.Token).ConfigureAwait(false);
            linked.Token.ThrowIfCancellationRequested();

            var size = new FileInfo(stagingPath).Length;
            if (response.SizeBytes != checked((ulong)size))
                throw new IOException("The downloaded byte count does not match the WPD file size.");

            File.Move(stagingPath, fullDestination, overwrite: false);
            return new WpdDownloadResult(checked((ulong)size), fullDestination);
        }
        catch (TimeoutException)
        {
            MarkFaulted();
            throw;
        }
        catch (OperationCanceledException)
        {
            MarkFaulted();
            throw;
        }
        finally
        {
            _operationGate.Release();
            try { File.Delete(stagingPath); } catch { }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _directories.Clear();
            _files.Clear();
        }

        _disposeCts.Cancel();
        _worker.Dispose();
    }

    string GetDirectoryPath(Guid directoryId)
    {
        ThrowIfDisposed();
        lock (_gate)
        {
            return _directories.TryGetValue(directoryId, out var path)
                ? path
                : throw new InvalidOperationException("The directory selection is stale or belongs to another WPD session.");
        }
    }

    string GetFilePath(Guid entryId)
    {
        ThrowIfDisposed();
        lock (_gate)
        {
            return _files.TryGetValue(entryId, out var path)
                ? path
                : throw new InvalidOperationException("The file selection is stale or belongs to another WPD session.");
        }
    }

    static string JoinDevicePath(string parent, string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name is "." or ".." || name.IndexOfAny(['/', '\\']) >= 0)
            throw new InvalidDataException("WPD returned an invalid path component.");
        return string.IsNullOrEmpty(parent) ? name : $"{parent}/{name}";
    }

    void MarkFaulted()
    {
        lock (_gate)
        {
            _faulted = true;
            _directories.Clear();
            _files.Clear();
        }
    }

    void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(WpdReadOnlySession));
        if (_faulted) throw new InvalidOperationException("The WPD session was stopped; select and open the device again.");
    }

    static string MetadataValue(string? value) => string.IsNullOrWhiteSpace(value) ? "Not reported" : value;
}
