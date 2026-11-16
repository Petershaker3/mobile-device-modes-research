using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DeviceToolkit.Core.Operations;
using DeviceToolkit.Core.Services;
using DeviceToolkit.Desktop.Services;
using DeviceToolkit.Wpd;

namespace DeviceToolkit.Desktop;

public sealed record WpdBrowserEntry(WpdFileEntry File, string DevicePath)
{
    public string Name => File.Name;
    public bool IsDirectory => File.IsDirectory;
    public string TypeText => IsDirectory ? "Folder" : "File";
    public string SizeText => IsDirectory ? string.Empty : File.SizeBytes is { } size
        ? size.ToString("N0", CultureInfo.CurrentCulture) + " bytes"
        : "Unknown";
}

public sealed record WpdDownloadPreview(
    WpdDeviceInfo Device,
    WpdBrowserEntry Entry,
    string LocalDestination,
    bool DestinationExists,
    OperationPlan Plan);

public sealed class WpdBrowserViewModel : INotifyPropertyChanged, IDisposable
{
    readonly DeviceWorkflow _workflow;
    readonly ApprovalService _approvalService;
    readonly Stack<(Guid DirectoryId, string Path)> _parents = new();
    CancellationTokenSource? _activeOperation;
    WpdReadOnlySession? _session;
    WpdDeviceInfo? _selectedDevice;
    WpdBrowserEntry? _selectedEntry;
    WpdDownloadPreview? _downloadPreview;
    Guid _currentDirectoryId;
    string _currentPath = "Storage not open";
    string _status = "Refresh MTP devices to begin. WPD browsing does not write to the phone.";
    string _listingSummary = "No MTP folder has been opened.";
    string _mtpModel = "Not read";
    string _mtpDeviceName = "Not read";
    string _mtpFirmwareVersion = "Not read";
    string _mtpPowerLevel = "Not read";
    string _mtpPowerSource = "Not read";
    string _mtpProtocol = "Not read";
    string _mtpDeviceDateTime = "Not read";
    string _mtpStorageSummary = "Not read";
    bool _isBusy;
    bool _listingLoaded;
    bool _disposed;

    public WpdBrowserViewModel(DeviceWorkflow workflow, ApprovalService approvalService)
    {
        _workflow = workflow;
        _approvalService = approvalService;
        RefreshDevicesCommand = new AsyncRelayCommand(RefreshDevicesAsync, () => !IsBusy);
        OpenStorageCommand = new AsyncRelayCommand(OpenStorageAsync, () => !IsBusy && SelectedDevice is not null && _session is null);
        ParentFolderCommand = new AsyncRelayCommand(GoToParentAsync, () => !IsBusy && _session is not null && _parents.Count > 0);
        RefreshFolderCommand = new AsyncRelayCommand(RefreshFolderAsync, () => !IsBusy && _session is not null && ListingLoaded);
        ConfirmDownloadCommand = new AsyncRelayCommand(ConfirmDownloadAsync, () => !IsBusy && DownloadPreview is { DestinationExists: false });
        CancelCommand = new RelayCommand(() => _activeOperation?.Cancel(), () => IsBusy && _activeOperation is not null);
    }

    public ObservableCollection<WpdDeviceInfo> Devices { get; } = new();
    public ObservableCollection<WpdBrowserEntry> Entries { get; } = new();
    public event PropertyChangedEventHandler? PropertyChanged;
    public ICommand RefreshDevicesCommand { get; }
    public ICommand OpenStorageCommand { get; }
    public ICommand ParentFolderCommand { get; }
    public ICommand RefreshFolderCommand { get; }
    public ICommand ConfirmDownloadCommand { get; }
    public ICommand CancelCommand { get; }

    public WpdDeviceInfo? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            if (!Set(ref _selectedDevice, value)) return;
            OnPropertyChanged(nameof(HasSelectedDevice));
            OnPropertyChanged(nameof(SelectedMetadataName));
            OnPropertyChanged(nameof(SelectedMetadataManufacturer));
            OnPropertyChanged(nameof(SelectedMetadataDescription));
            OnPropertyChanged(nameof(MtpSamFwFirmwareUri));
            OnPropertyChanged(nameof(HasMtpSamFwFirmwareUri));
            MtpModel = "Not read";
            MtpDeviceName = "Not read";
            MtpFirmwareVersion = "Not read";
            MtpPowerLevel = "Not read";
            MtpPowerSource = "Not read";
            MtpProtocol = "Not read";
            MtpDeviceDateTime = "Not read";
            MtpStorageSummary = "Not read";
            CloseSession();
            RaiseCommands();
        }
    }

    public bool HasSelectedDevice => SelectedDevice is not null;
    public string SelectedMetadataName => MetadataValue(SelectedDevice?.FriendlyName);
    public string SelectedMetadataManufacturer => MetadataValue(SelectedDevice?.Manufacturer);
    public string SelectedMetadataDescription => MetadataValue(SelectedDevice?.Description);
    public string MtpModel
    {
        get => _mtpModel;
        private set
        {
            if (!Set(ref _mtpModel, value)) return;
            OnPropertyChanged(nameof(MtpSamFwFirmwareUri));
            OnPropertyChanged(nameof(HasMtpSamFwFirmwareUri));
        }
    }
    public string MtpDeviceName { get => _mtpDeviceName; private set => Set(ref _mtpDeviceName, value); }
    public string MtpFirmwareVersion { get => _mtpFirmwareVersion; private set => Set(ref _mtpFirmwareVersion, value); }
    public string MtpPowerLevel { get => _mtpPowerLevel; private set => Set(ref _mtpPowerLevel, value); }
    public string MtpPowerSource { get => _mtpPowerSource; private set => Set(ref _mtpPowerSource, value); }
    public string MtpProtocol { get => _mtpProtocol; private set => Set(ref _mtpProtocol, value); }
    public string MtpDeviceDateTime { get => _mtpDeviceDateTime; private set => Set(ref _mtpDeviceDateTime, value); }
    public string MtpStorageSummary { get => _mtpStorageSummary; private set => Set(ref _mtpStorageSummary, value); }
    public Uri? MtpSamFwFirmwareUri => SelectedDevice is { } device
        ? SamsungFirmwareLinks.ForReportedDevice(device.Manufacturer, MtpModel, string.Empty)
        : null;
    public bool HasMtpSamFwFirmwareUri => MtpSamFwFirmwareUri is not null;

    public WpdBrowserEntry? SelectedEntry
    {
        get => _selectedEntry;
        set
        {
            if (!Set(ref _selectedEntry, value)) return;
            DownloadPreview = null;
            OnPropertyChanged(nameof(CanPrepareDownload));
        }
    }

    public WpdDownloadPreview? DownloadPreview
    {
        get => _downloadPreview;
        private set
        {
            if (!Set(ref _downloadPreview, value)) return;
            OnPropertyChanged(nameof(HasDownloadPreview));
            OnPropertyChanged(nameof(DownloadPreviewSummary));
            RaiseCommands();
        }
    }

    public bool HasDownloadPreview => DownloadPreview is not null;
    public string DownloadPreviewSummary => DownloadPreview is not { } preview
        ? string.Empty
        : $"Phone: {preview.Device.FriendlyName}\nSource: {preview.Entry.DevicePath}\nDestination: {preview.LocalDestination}\nSize: {preview.Entry.File.SizeBytes?.ToString("N0", CultureInfo.CurrentCulture) ?? "Unknown"} bytes\nThe phone will only be read. The destination must not already exist.";
    public bool CanPrepareDownload => !IsBusy && _session is not null && SelectedEntry is { IsDirectory: false, File.SizeBytes: not null };
    public string CurrentPath { get => _currentPath; private set => Set(ref _currentPath, value); }
    public string Status { get => _status; private set => Set(ref _status, value); }
    public string ListingSummary { get => _listingSummary; private set => Set(ref _listingSummary, value); }
    public bool ListingLoaded
    {
        get => _listingLoaded;
        private set
        {
            if (!Set(ref _listingLoaded, value)) return;
            (RefreshFolderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }
    }
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!Set(ref _isBusy, value)) return;
            RaiseCommands();
        }
    }

    public async Task RefreshDevicesAsync()
    {
        CloseSession();
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        Status = "Reading connected Windows Portable Devices. No phone data is changed.";
        try
        {
            var found = await _workflow.EnumerateWpdDevicesAsync(_activeOperation.Token);
            Devices.Clear();
            foreach (var device in found) Devices.Add(device);
            SelectedDevice = null;
            Status = Devices.Count switch
            {
                0 => "No WPD device found. Connect the phone with USB and choose File transfer or MTP.",
                1 => "One WPD device found. Select it explicitly, then open its storage.",
                _ => $"{Devices.Count} WPD devices found. Select the intended phone explicitly."
            };
        }
        catch (OperationCanceledException)
        {
            Devices.Clear();
            SelectedDevice = null;
            Status = "WPD discovery was cancelled.";
        }
        catch
        {
            Devices.Clear();
            SelectedDevice = null;
            Status = "WPD discovery failed. Check the USB data connection and phone unlock state.";
        }
        finally { EndOperation(); }
    }

    async Task OpenStorageAsync()
    {
        if (SelectedDevice is not { } device) return;
        CloseSession();
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        Status = "Opening a read-only MTP session for the selected phone.";
        try
        {
            var session = await _workflow.OpenWpdReadOnlySessionAsync(device, _activeOperation.Token);
            _session = session;
            var details = await _workflow.ReadWpdDeviceDetailsAsync(device, session, _activeOperation.Token);
            MtpModel = details.Model;
            MtpDeviceName = details.Name;
            MtpFirmwareVersion = details.FirmwareVersion;
            MtpPowerLevel = details.PowerLevel;
            MtpPowerSource = details.PowerSource;
            MtpProtocol = details.Protocol;
            MtpDeviceDateTime = details.DeviceDateTime;
            MtpStorageSummary = details.Storage.Count == 0
                ? "Not reported by Windows"
                : string.Join(Environment.NewLine, details.Storage.Select(storage =>
                    $"{storage.Name}: {FormatBytes(storage.TotalBytes)} total, {FormatBytes(storage.FreeBytes)} available" +
                    (storage.FileSystem == "Not reported" ? string.Empty : $", {storage.FileSystem}")));
            var root = await _workflow.ListWpdDirectoryAsync(device, session, session.RootDirectoryId, "/", _activeOperation.Token);
            var storageRoots = root.Where(entry => entry.IsDirectory && (entry.Name is "Internal storage" or "Phone")).ToArray();
            if (storageRoots.Length != 1)
                throw new InvalidOperationException("Could not identify exactly one known phone storage volume. No volume was guessed.");
            _parents.Clear();
            CurrentPath = storageRoots[0].Name;
            _currentDirectoryId = storageRoots[0].EntryId;
            await LoadDirectoryAsync(_currentDirectoryId, CurrentPath, _activeOperation.Token);
        }
        catch (OperationCanceledException)
        {
            RequireDeviceRefresh();
            Status = "Opening the MTP session was cancelled. Refresh devices before retrying.";
        }
        catch (Exception ex)
        {
            RequireDeviceRefresh();
            Status = $"Could not open MTP storage: {ex.Message} Refresh devices and retry.";
        }
        finally
        {
            EndOperation();
            RaiseCommands();
        }
    }

    async Task GoToParentAsync()
    {
        if (_session is null || _parents.Count == 0) return;
        var parent = _parents.Pop();
        await BrowseAsync(parent.DirectoryId, parent.Path);
    }

    Task RefreshFolderAsync() => BrowseAsync(_currentDirectoryId, CurrentPath);

    public async Task OpenDirectoryAsync(WpdBrowserEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (!entry.IsDirectory || _session is null) return;
        _parents.Push((_currentDirectoryId, CurrentPath));
        await BrowseAsync(entry.File.EntryId, entry.DevicePath);
    }

    async Task BrowseAsync(Guid directoryId, string path)
    {
        if (_session is null || SelectedDevice is not { } device) return;
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        Status = $"Reading names and metadata in {path}. File contents are not opened.";
        try
        {
            await LoadDirectoryAsync(directoryId, path, _activeOperation.Token);
        }
        catch (OperationCanceledException)
        {
            RequireDeviceRefresh();
            Status = "The MTP request was cancelled and its worker stopped. Refresh devices and reopen storage.";
        }
        catch (TimeoutException)
        {
            RequireDeviceRefresh();
            Status = "The MTP request exceeded its deadline and its worker stopped. Refresh devices and reopen storage.";
        }
        catch (Exception ex)
        {
            RequireDeviceRefresh();
            Status = $"Could not read the MTP folder: {ex.Message} Refresh devices and reopen storage.";
        }
        finally
        {
            EndOperation();
            RaiseCommands();
        }
    }

    async Task LoadDirectoryAsync(Guid directoryId, string path, CancellationToken ct)
    {
        if (_session is null || SelectedDevice is not { } device) return;
        var entries = await _workflow.ListWpdDirectoryAsync(device, _session, directoryId, path, ct);
        var projected = entries
            .OrderByDescending(entry => entry.IsDirectory)
            .ThenBy(entry => entry.Name, StringComparer.CurrentCultureIgnoreCase)
            .Select(entry => new WpdBrowserEntry(entry, JoinDevicePath(path, entry.Name)))
            .ToArray();
        Entries.Clear();
        foreach (var entry in projected) Entries.Add(entry);
        SelectedEntry = null;
        _currentDirectoryId = directoryId;
        CurrentPath = path;
        ListingLoaded = true;
        ListingSummary = $"{Entries.Count} folders and files loaded. Names and sizes only; contents are not opened.";
        Status = $"Loaded {Entries.Count} items from {path}.";
    }

    public void PrepareDownload(string localDestination)
    {
        if (!CanPrepareDownload || SelectedDevice is not { } device || SelectedEntry is not { } entry) return;
        var fullDestination = Path.GetFullPath(localDestination);
        var exists = File.Exists(fullDestination) || Directory.Exists(fullDestination);
        if (exists)
        {
            DownloadPreview = null;
            Status = "That local destination already exists. Choose a new destination.";
            return;
        }

        var plan = _workflow.CreateWpdDownloadPlan(device, entry.File, entry.DevicePath, fullDestination);
        DownloadPreview = new WpdDownloadPreview(device, entry, fullDestination, false, plan);
        Status = "Review the selected phone, MTP file, local destination, and size before confirming the download.";
    }

    async Task ConfirmDownloadAsync()
    {
        if (DownloadPreview is not { DestinationExists: false } preview || _session is not { } session ||
            SelectedDevice is not { } device || device.SelectionId != preview.Device.SelectionId)
        {
            DownloadPreview = null;
            Status = "The phone or MTP session changed. Select the phone and prepare the download again.";
            return;
        }

        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        Status = "Downloading one selected MTP file to the reviewed local path.";
        try
        {
            var approval = _approvalService.ApproveAfterUserConfirmation(preview.Plan);
            await _workflow.RecordApprovalGrantedAsync(preview.Plan);
            var result = await _workflow.DownloadWpdFileAsync(device, session, preview.Entry.File,
                preview.Entry.DevicePath, preview.LocalDestination, preview.Plan, approval, _activeOperation.Token);
            DownloadPreview = null;
            Status = $"Downloaded {result.SizeBytes:N0} bytes. The phone was only read.";
        }
        catch (OperationCanceledException)
        {
            RequireDeviceRefresh();
            DownloadPreview = null;
            Status = "Download cancelled. The worker stopped; refresh devices and reopen storage before retrying.";
        }
        catch (TimeoutException)
        {
            RequireDeviceRefresh();
            DownloadPreview = null;
            Status = "The MTP download exceeded its deadline and the worker stopped. Refresh devices and reopen storage.";
        }
        catch (Exception ex)
        {
            DownloadPreview = null;
            Status = $"Download failed: {ex.Message}";
        }
        finally
        {
            EndOperation();
            RaiseCommands();
        }
    }

    static string JoinDevicePath(string parent, string name)
        => string.IsNullOrEmpty(parent) ? name : $"{parent.TrimEnd('/')}/{name}";

    void CloseSession()
    {
        _session?.Dispose();
        _session = null;
        Entries.Clear();
        SelectedEntry = null;
        _parents.Clear();
        _currentDirectoryId = Guid.Empty;
        CurrentPath = "Storage not open";
        ListingLoaded = false;
        ListingSummary = "No MTP folder has been opened.";
        DownloadPreview = null;
    }

    void RequireDeviceRefresh()
    {
        CloseSession();
        Devices.Clear();
        if (_selectedDevice is not null) SelectedDevice = null;
        else RaiseCommands();
    }

    void EndOperation()
    {
        _activeOperation?.Dispose();
        _activeOperation = null;
        IsBusy = false;
    }

    void RaiseCommands()
    {
        (RefreshDevicesCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (OpenStorageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ParentFolderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (RefreshFolderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ConfirmDownloadCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(CanPrepareDownload));
    }

    bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    static string MetadataValue(string? value)
        => string.IsNullOrWhiteSpace(value) ? "Not reported by Windows" : value.Trim();

    static string FormatBytes(long? bytes) => bytes is { } value
        ? string.Format(CultureInfo.CurrentCulture, "{0:N1} GB", value / 1_000_000_000d)
        : "not reported";

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _activeOperation?.Cancel();
        _activeOperation?.Dispose();
        CloseSession();
    }
}
