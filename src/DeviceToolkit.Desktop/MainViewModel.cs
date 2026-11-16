using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Data;
using AdvancedSharpAdbClient.Models;
using DeviceToolkit.Core.Operations;
using DeviceToolkit.Core.Services;
using DeviceToolkit.Core.Windows;
using DeviceToolkit.Desktop.Services;
using logcat.Services;

namespace DeviceToolkit.Desktop;

public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
{
    readonly ApprovalService _approvalService = new();
    readonly DeviceWorkflow _workflow;
    CancellationTokenSource? _activeOperation;
    ConnectedDevice? _selectedDevice;
    FastbootTarget? _selectedFastbootDevice;
    FastbootProfileOption _selectedFastbootProfile = FastbootOutputParser.Profiles[0];
    string _fastbootMode = "No Fastboot information has been read.";
    DeviceFacts? _facts;
    string _status = "Connect an Android phone with USB debugging enabled, then refresh devices.";
    string _result = "No diagnostic has been run.";
    bool _isBusy;
    string _appFilter = string.Empty;
    bool _appsLoaded;
    InstalledApp? _selectedApp;
    AppVersion? _selectedAppVersion;
    string? _pendingApkPath;
    ApkInstallPreview? _installPreview;
    AppRemovalPreview? _removalPreview;
    string _remoteFileDirectory = AndroidSharedStoragePaths.Root;
    bool _fileListingLoaded;
    AndroidFileEntry? _selectedFileEntry;
    FileTransferPreview? _transferPreview;
    string _transferProgressText = "No transfer is running.";
    double _transferProgressPercent;
    bool _windowsInventoryLoaded;
    string _windowsInventorySummary = "Windows device inventory has not been loaded.";
    string? _windowsInventoryError;
    string? _windowsInventoryEmptyMessage = "Select refresh to read present Windows devices and reported driver problems.";
    DateTimeOffset? _windowsInventoryUpdatedAt;

    public ObservableCollection<ConnectedDevice> Devices { get; } = new();
    public ObservableCollection<FastbootTarget> FastbootDevices { get; } = new();
    public IReadOnlyList<FastbootProfileOption> FastbootProfiles => FastbootOutputParser.Profiles;
    public ObservableCollection<FastbootFact> FastbootFacts { get; } = new();
    public WpdBrowserViewModel WpdBrowser { get; }
    public ObservableCollection<InstalledApp> InstalledApps { get; } = new();
    public ObservableCollection<AndroidFileEntry> FileEntries { get; } = new();
    public ObservableCollection<PnpDeviceSnapshot> WindowsDevices { get; } = new();
    public ICollectionView AppsView { get; }
    public event PropertyChangedEventHandler? PropertyChanged;
    public ICommand RefreshCommand { get; }
    public ICommand ReadInfoCommand { get; }
    public ICommand CaptureCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand RefreshAppsCommand { get; }
    public ICommand ReadAppVersionCommand { get; }
    public ICommand InstallApkCommand { get; }
    public ICommand PreviewRemovalCommand { get; }
    public ICommand RemoveAppCommand { get; }
    public ICommand LoadSharedStorageCommand { get; }
    public ICommand ParentFolderCommand { get; }
    public ICommand RefreshFolderCommand { get; }
    public ICommand ConfirmTransferCommand { get; }
    public ICommand RefreshWindowsDevicesCommand { get; }
    public ICommand RefreshFastbootDevicesCommand { get; }
    public ICommand ReadFastbootInfoCommand { get; }

    public MainViewModel()
    {
        _workflow = new DeviceWorkflow(_approvalService);
        WpdBrowser = new WpdBrowserViewModel(_workflow, _approvalService);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => !IsBusy);
        ReadInfoCommand = new AsyncRelayCommand(ReadInfoAsync, () => !IsBusy && SelectedDevice?.State == "Online");
        CaptureCommand = new AsyncRelayCommand(CaptureAsync, () => !IsBusy && SelectedDevice?.State == "Online");
        CancelCommand = new RelayCommand(() => _activeOperation?.Cancel(), () => IsBusy && _activeOperation is not null);
        RefreshAppsCommand = new AsyncRelayCommand(RefreshAppsAsync, () => !IsBusy && SelectedDevice?.State == "Online");
        ReadAppVersionCommand = new AsyncRelayCommand(ReadAppVersionAsync, () => !IsBusy && SelectedDevice?.State == "Online" && SelectedApp is not null);
        InstallApkCommand = new AsyncRelayCommand(InstallApkAsync, () => !IsBusy && SelectedDevice?.State == "Online" && InstallPreview is not null && _pendingApkPath is not null && !InstallPreview.AlreadyInstalled);
        PreviewRemovalCommand = new RelayCommand(PreviewRemoval, () => !IsBusy && SelectedDevice?.State == "Online" && SelectedApp?.IsThirdParty == true);
        RemoveAppCommand = new AsyncRelayCommand(RemoveAppAsync, () => !IsBusy && SelectedDevice?.State == "Online" && RemovalPreview is not null);
        LoadSharedStorageCommand = new AsyncRelayCommand(LoadSharedStorageAsync, () => !IsBusy && SelectedDevice?.State == "Online");
        ParentFolderCommand = new AsyncRelayCommand(GoToParentFolderAsync, () => !IsBusy && SelectedDevice?.State == "Online" && AndroidSharedStoragePaths.GetParent(RemoteFileDirectory) is not null);
        RefreshFolderCommand = new AsyncRelayCommand(RefreshFolderAsync, () => !IsBusy && SelectedDevice?.State == "Online" && FileListingLoaded);
        ConfirmTransferCommand = new AsyncRelayCommand(ConfirmTransferAsync, CanConfirmTransfer);
        RefreshWindowsDevicesCommand = new AsyncRelayCommand(RefreshWindowsDevicesAsync, () => !IsBusy);
        RefreshFastbootDevicesCommand = new AsyncRelayCommand(RefreshFastbootDevicesAsync, () => !IsBusy);
        ReadFastbootInfoCommand = new AsyncRelayCommand(ReadFastbootInfoAsync, () => !IsBusy && SelectedFastbootDevice is not null);
        AppsView = CollectionViewSource.GetDefaultView(InstalledApps);
        AppsView.Filter = MatchesAppFilter;
        FileEntries.CollectionChanged += (_, _) => OnPropertyChanged(nameof(RemoteFileListingSummary));
        FastbootFacts.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasFastbootFacts));
    }

    public ConnectedDevice? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            if (!Set(ref _selectedDevice, value)) return;
            _activeOperation?.Cancel();
            Facts = null;
            InstallPreview = null;
            _pendingApkPath = null;
            RemovalPreview = null;
            InstalledApps.Clear();
            FileEntries.Clear();
            SelectedFileEntry = null;
            TransferPreview = null;
            RemoteFileDirectory = AndroidSharedStoragePaths.Root;
            FileListingLoaded = false;
            SelectedApp = null;
            AppsLoaded = false;
            AppsView.Refresh();
            OnPropertyChanged(nameof(AppCount));
            RaiseCommands();
        }
    }

    public FastbootTarget? SelectedFastbootDevice
    {
        get => _selectedFastbootDevice;
        set
        {
            if (!Set(ref _selectedFastbootDevice, value)) return;
            _activeOperation?.Cancel();
            FastbootFacts.Clear();
            FastbootMode = "Select a Fastboot device, then read its allowlisted information.";
            (ReadFastbootInfoCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public FastbootProfileOption SelectedFastbootProfile
    {
        get => _selectedFastbootProfile;
        set
        {
            if (!Set(ref _selectedFastbootProfile, value)) return;
            _activeOperation?.Cancel();
            FastbootFacts.Clear();
            FastbootMode = value.Profile == FastbootDeviceProfile.Huawei
                ? "Huawei Fastboot profile selected. Hardware reads are confirmed on PPA-LX2; other models remain unverified."
                : "Android Fastboot profile selected.";
        }
    }

    public string FastbootMode { get => _fastbootMode; private set => Set(ref _fastbootMode, value); }
    public bool HasFastbootFacts => FastbootFacts.Count > 0;

    public DeviceFacts? Facts
    {
        get => _facts;
        private set
        {
            if (!Set(ref _facts, value)) return;
            OnPropertyChanged(nameof(SamFwFirmwareUri));
            OnPropertyChanged(nameof(SamFwFirmwareAvailable));
        }
    }
    public Uri? SamFwFirmwareUri => Facts is { } facts
        ? SamsungFirmwareLinks.ForReportedDevice(facts.Manufacturer, facts.Model, facts.SamsungCsc)
        : null;
    public bool SamFwFirmwareAvailable => SamFwFirmwareUri is not null;
    public string Status { get => _status; private set => Set(ref _status, value); }
    public string Result { get => _result; private set => Set(ref _result, value); }

    public string WindowsInventorySummary
    {
        get => _windowsInventorySummary;
        private set => Set(ref _windowsInventorySummary, value);
    }
    public string? WindowsInventoryError
    {
        get => _windowsInventoryError;
        private set => Set(ref _windowsInventoryError, value);
    }
    public string? WindowsInventoryEmptyMessage
    {
        get => _windowsInventoryEmptyMessage;
        private set => Set(ref _windowsInventoryEmptyMessage, value);
    }
    public DateTimeOffset? WindowsInventoryUpdatedAt
    {
        get => _windowsInventoryUpdatedAt;
        private set
        {
            if (Set(ref _windowsInventoryUpdatedAt, value))
                OnPropertyChanged(nameof(WindowsInventoryFreshness));
        }
    }
    public string WindowsInventoryFreshness => WindowsInventoryUpdatedAt is { } updated
        ? $"Last read {updated.ToLocalTime():g}"
        : "Not read yet";
    public bool WindowsInventoryLoaded
    {
        get => _windowsInventoryLoaded;
        private set => Set(ref _windowsInventoryLoaded, value);
    }
    public int AppCount => InstalledApps.Count;
    public bool AppsLoaded
    {
        get => _appsLoaded;
        private set { if (Set(ref _appsLoaded, value)) OnPropertyChanged(nameof(AppInventorySummary)); }
    }
    public string AppInventorySummary => AppsLoaded ? $"{AppCount} packages loaded" : "Inventory not loaded";
    public string RemoteFileDirectoryDisplay => AndroidSharedStoragePaths.ToDisplayText(RemoteFileDirectory);
    public string RemoteFileListingSummary => FileListingLoaded
        ? $"{FileEntries.Count} folders and files loaded"
        : FileEntries.Count > 0
            ? $"Listing incomplete. {FileEntries.Count} entries received before the operation stopped."
            : "Shared storage has not been opened.";
    public string RemoteFileDirectory
    {
        get => _remoteFileDirectory;
        private set
        {
            if (Set(ref _remoteFileDirectory, value))
            {
                OnPropertyChanged(nameof(RemoteFileDirectoryDisplay));
                (ParentFolderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }
    public bool FileListingLoaded
    {
        get => _fileListingLoaded;
        private set
        {
            if (Set(ref _fileListingLoaded, value))
            {
                OnPropertyChanged(nameof(RemoteFileListingSummary));
                (RefreshFolderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }
    public AndroidFileEntry? SelectedFileEntry
    {
        get => _selectedFileEntry;
        set
        {
            if (!Set(ref _selectedFileEntry, value)) return;
            TransferPreview = null;
            RaiseCommands();
        }
    }
    public FileTransferPreview? TransferPreview
    {
        get => _transferPreview;
        private set
        {
            if (!Set(ref _transferPreview, value)) return;
            OnPropertyChanged(nameof(TransferPreviewSummary));
            OnPropertyChanged(nameof(TransferActionText));
            OnPropertyChanged(nameof(HasTransferPreview));
            RaiseCommands();
        }
    }
    public bool HasTransferPreview => TransferPreview is not null;
    public string TransferActionText => TransferPreview?.Direction == FileTransferDirection.Upload ? "Upload file" : "Download file";
    public string TransferPreviewSummary => TransferPreview is not { } preview
        ? string.Empty
        : $"Phone: {preview.DeviceModel} ({preview.DeviceSerial})\nSource: {preview.Source}\nDestination: {preview.Destination}\nSize: {preview.SizeBytes:N0} bytes\n" +
          (preview.DestinationExists ? "Destination already exists. Choose a different name before continuing." : "The destination is clear. Review it, then start this single-file transfer.");
    public string TransferProgressText { get => _transferProgressText; private set => Set(ref _transferProgressText, value); }
    public double TransferProgressPercent { get => _transferProgressPercent; private set => Set(ref _transferProgressPercent, value); }
    public bool CanChooseDownload => !IsBusy && SelectedDevice?.State == "Online" && SelectedFileEntry?.Kind == AndroidFileKind.File && SelectedFileEntry.Error == UnixErrorCode.Default;
    public bool CanChooseUpload => !IsBusy && SelectedDevice?.State == "Online" && FileListingLoaded;
    public InstalledApp? SelectedApp
    {
        get => _selectedApp;
        set
        {
            if (!Set(ref _selectedApp, value)) return;
            SelectedAppVersion = null;
            RemovalPreview = null;
            RaiseCommands();
        }
    }
    public AppVersion? SelectedAppVersion { get => _selectedAppVersion; private set => Set(ref _selectedAppVersion, value); }
    public ApkInstallPreview? InstallPreview { get => _installPreview; private set { if (Set(ref _installPreview, value)) RaiseCommands(); } }
    public AppRemovalPreview? RemovalPreview { get => _removalPreview; private set { if (Set(ref _removalPreview, value)) RaiseCommands(); } }
    public string AppFilter
    {
        get => _appFilter;
        set { if (Set(ref _appFilter, value ?? string.Empty)) AppsView.Refresh(); }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set { if (Set(ref _isBusy, value)) RaiseCommands(); }
    }

    async Task RefreshAsync()
    {
        IsBusy = true;
        Status = "Checking ADB devices. No device shell command is sent during discovery.";
        try
        {
            var selectedSerial = SelectedDevice?.Serial;
            var found = await _workflow.ListDevicesAsync();
            Devices.Clear();
            foreach (var device in found) Devices.Add(device);
            SelectedDevice = Devices.FirstOrDefault(device => device.Serial == selectedSerial);
            Status = Devices.Count switch
            {
                0 => "No ADB devices found. Check the data cable, USB debugging, and the phone's authorization prompt.",
                1 => "One device found. Select it explicitly before reading information or capturing logs.",
                _ => $"{Devices.Count} devices found. Select the intended phone explicitly."
            };
        }
        catch
        {
            Status = "Device discovery failed. Check that Android Platform Tools are installed and the ADB server is available.";
        }
        finally { IsBusy = false; }
    }

    async Task RefreshWindowsDevicesAsync()
    {
        IsBusy = true;
        WindowsInventoryError = null;
        WindowsInventoryEmptyMessage = WindowsInventoryLoaded ? null : "Reading present Windows devices...";
        WindowsInventorySummary = WindowsInventoryLoaded
            ? $"Refreshing {WindowsDevices.Count} devices. Previous results remain visible until the read completes."
            : "Reading present Windows devices...";
        Status = "Reading Windows Plug and Play inventory. No device or driver state will be changed.";
        try
        {
            var found = await _workflow.EnumerateWindowsDevicesAsync();
            WindowsDevices.Clear();
            foreach (var device in found
                         .OrderByDescending(item => item.ProblemCode is > 0)
                         .ThenBy(item => item.ProblemCode is null ? 0 : 1)
                         .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase))
                WindowsDevices.Add(device);

            int problemCount = found.Count(item => item.ProblemCode is > 0);
            int unknownCount = found.Count(item => item.ProblemCode is null);
            WindowsInventoryLoaded = true;
            WindowsInventoryUpdatedAt = DateTimeOffset.Now;
            WindowsInventorySummary = $"{found.Count} present; {problemCount} with problem codes; {unknownCount} status unavailable.";
            WindowsInventoryEmptyMessage = found.Count == 0 ? "No present Windows devices were returned." : null;
            Status = "Windows device inventory refreshed. No settings were changed.";
        }
        catch (Exception ex)
        {
            WindowsInventoryError = $"Windows device inventory could not be read: {ex.Message}";
            WindowsInventoryEmptyMessage = null;
            WindowsInventorySummary = WindowsInventoryLoaded
                ? $"Refresh failed. Previous results remain visible. {WindowsInventoryFreshness}."
                : "Inventory unavailable.";
            Status = "Windows device inventory read failed. Retry from the Windows drivers tab.";
        }
        finally { IsBusy = false; }
    }

    public async Task ConnectWirelessAsync(string pairingCode)
    {
        if (IsBusy) return;
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        RaiseCommands();
        Status = "Finding the phone's open pairing screen, then pairing and connecting.";
        try
        {
            var serial = await _workflow.PairAndConnectWirelessAsync(pairingCode, _activeOperation.Token);
            var found = await _workflow.ListDevicesAsync(_activeOperation.Token);
            Devices.Clear();
            foreach (var device in found) Devices.Add(device);
            SelectedDevice = Devices.FirstOrDefault(device => string.Equals(device.Serial, serial, StringComparison.Ordinal));
            Status = SelectedDevice is null
                ? "ADB connected, but the phone was not present in the refreshed list. Refresh devices to retry discovery."
                : "Phone paired and connected over Wi-Fi. It is selected in the device list.";
        }
        catch (OperationCanceledException) { Status = "Wireless connection cancelled."; }
        catch (ArgumentException ex) { Status = ex.Message; }
        catch (InvalidOperationException ex) { Status = ex.Message; }
        catch { Status = "Wireless connection failed. Confirm the phone is showing a current pairing code and try again."; }
        finally
        {
            _activeOperation.Dispose();
            _activeOperation = null;
            IsBusy = false;
        }
    }

    async Task ReadInfoAsync()
    {
        if (SelectedDevice is null) return;
        IsBusy = true;
        Status = "Reading a short allowlist of public Android properties from the selected device.";
        try
        {
            Facts = await _workflow.ReadFactsAsync(SelectedDevice, CancellationToken.None);
            Status = "Device information read successfully. Values remain in this window only.";
        }
        catch
        {
            Facts = null;
            Status = "Could not read device properties. Confirm the selected phone is authorized and still connected.";
        }
        finally { IsBusy = false; }
    }

    async Task RefreshFastbootDevicesAsync()
    {
        var selectedSerial = SelectedFastbootDevice?.Serial;
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        Status = "Searching for devices already in Fastboot mode. The phone will not be rebooted or changed.";
        try
        {
            var found = await _workflow.ListFastbootDevicesAsync(_activeOperation.Token);
            FastbootDevices.Clear();
            foreach (var target in found) FastbootDevices.Add(target);
            SelectedFastbootDevice = found.FirstOrDefault(target => target.Serial == selectedSerial);
            Status = found.Count switch
            {
                0 => "No Fastboot devices found. If needed, switch the phone to Fastboot manually, then refresh.",
                1 => "One Fastboot device found. Select it explicitly before reading information.",
                _ => $"{found.Count} Fastboot devices found. Select the intended device explicitly."
            };
        }
        catch (OperationCanceledException) { Status = "Fastboot discovery cancelled."; }
        catch (Exception ex) { Status = $"Fastboot discovery failed: {ex.Message}"; }
        finally
        {
            _activeOperation.Dispose();
            _activeOperation = null;
            IsBusy = false;
        }
    }

    async Task ReadFastbootInfoAsync()
    {
        if (SelectedFastbootDevice is null) return;
        var target = SelectedFastbootDevice;
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        FastbootFacts.Clear();
        var profile = SelectedFastbootProfile.Profile;
        FastbootMode = profile == FastbootDeviceProfile.Huawei
            ? "Reading the allowlisted Fastboot variables and fixed Huawei version, model, and lock-state queries."
            : "Reading four allowlisted Fastboot variables from the selected device.";
        Status = profile == FastbootDeviceProfile.Huawei
            ? "Huawei profile uses fixed read-only model, version, and lock-state queries. Unsupported commands are reported individually; no identifiers, transitions, or writes are requested."
            : "Reading Fastboot information. No getvar all, OEM, transition, or write command is used.";
        try
        {
            var facts = await _workflow.ReadFastbootFactsAsync(target, profile, _activeOperation.Token);
            foreach (var fact in facts.Facts) FastbootFacts.Add(fact);
            FastbootMode = facts.Mode;
            Status = profile == FastbootDeviceProfile.Huawei
                ? "Huawei read-only queries completed. PPA-LX2 was verified on hardware; other models remain unverified. Unsupported values are shown with their status."
                : "Fastboot information read. Unsupported values are shown as NotReported.";
        }
        catch (OperationCanceledException) { FastbootMode = "Read cancelled."; Status = "Fastboot information read cancelled."; }
        catch (Exception ex) { FastbootMode = "Information unavailable."; Status = $"Fastboot read failed: {ex.Message}"; }
        finally
        {
            _activeOperation.Dispose();
            _activeOperation = null;
            IsBusy = false;
        }
    }

    async Task RefreshAppsAsync()
    {
        if (SelectedDevice is null) return;
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        RaiseCommands();
        Status = "Reading installed package IDs and APK paths from the selected phone.";
        try
        {
            var found = await _workflow.ReadInstalledAppsAsync(SelectedDevice, _activeOperation.Token);
            InstalledApps.Clear();
            foreach (var app in found) InstalledApps.Add(app);
            SelectedApp = null;
            AppsLoaded = true;
            AppsView.Refresh();
            OnPropertyChanged(nameof(AppCount));
            Status = $"Loaded {AppCount} package records. No app was installed, disabled, or removed.";
        }
        catch (OperationCanceledException)
        {
            InstalledApps.Clear();
            SelectedApp = null;
            AppsLoaded = false;
            AppsView.Refresh();
            OnPropertyChanged(nameof(AppCount));
            Status = "App inventory read cancelled.";
        }
        catch
        {
            InstalledApps.Clear();
            AppsLoaded = false;
            AppsView.Refresh();
            OnPropertyChanged(nameof(AppCount));
            Status = "Could not read the app inventory. Confirm the selected phone is authorized and still connected.";
        }
        finally
        {
            _activeOperation.Dispose();
            _activeOperation = null;
            IsBusy = false;
        }
    }

    bool MatchesAppFilter(object item)
    {
        if (item is not InstalledApp app) return false;
        return string.IsNullOrWhiteSpace(AppFilter)
            || app.PackageId.Contains(AppFilter, StringComparison.OrdinalIgnoreCase)
            || app.ApkPath.Contains(AppFilter, StringComparison.OrdinalIgnoreCase);
    }

    async Task ReadAppVersionAsync()
    {
        if (SelectedDevice is null || SelectedApp is null) return;
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        RaiseCommands();
        Status = "Reading version metadata for the selected package.";
        try
        {
            SelectedAppVersion = await _workflow.ReadAppVersionAsync(SelectedDevice, SelectedApp, _activeOperation.Token);
            Status = "Package version metadata loaded.";
        }
        catch (OperationCanceledException)
        {
            SelectedAppVersion = null;
            Status = "Package detail read cancelled.";
        }
        catch
        {
            SelectedAppVersion = null;
            Status = "Could not read version information for the selected package.";
        }
        finally
        {
            _activeOperation.Dispose();
            _activeOperation = null;
            IsBusy = false;
        }
    }

    async Task CaptureAsync()
    {
        if (SelectedDevice is null) return;
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        RaiseCommands();
        Status = "Capturing the selected device's recent logcat for 30 seconds. The local temporary file is deleted after analysis.";
        Result = "Capture in progress.";
        try
        {
            var capture = await _workflow.CaptureAndAnalyzeAsync(
                SelectedDevice, TimeSpan.FromSeconds(30), _activeOperation.Token);
            Result = $"Captured {capture.Lines} log lines. Found {capture.CrashEvents} crash events in {capture.CrashGroups} groups.";
            Status = "Analysis complete. The log was processed locally and the temporary file was removed.";
        }
        catch (OperationCanceledException)
        {
            Result = "Capture cancelled. Temporary data was removed.";
            Status = "Diagnostic capture cancelled.";
        }
        catch
        {
            Result = "No complete diagnostic result was produced.";
            Status = "Capture or analysis failed. No logs were uploaded or saved as a report.";
        }
        finally
        {
            _activeOperation.Dispose();
            _activeOperation = null;
            IsBusy = false;
        }
    }

    Task LoadSharedStorageAsync() => BrowseSharedStorageAsync(AndroidSharedStoragePaths.Root);

    async Task GoToParentFolderAsync()
    {
        var parent = AndroidSharedStoragePaths.GetParent(RemoteFileDirectory);
        if (parent is not null) await BrowseSharedStorageAsync(parent);
    }

    Task RefreshFolderAsync() => BrowseSharedStorageAsync(RemoteFileDirectory);

    public void PrepareDownload(string localDestination)
    {
        if (!CanChooseDownload || SelectedDevice is null || SelectedFileEntry is null) return;
        var entry = SelectedFileEntry;
        var destination = Path.GetFullPath(localDestination);
        TransferPreview = new FileTransferPreview(
            FileTransferDirection.Download,
            SelectedDevice.Model,
            SelectedDevice.Serial,
            AndroidSharedStoragePaths.ToDisplayText(entry.RemotePath),
            AndroidSharedStoragePaths.ToDisplayText(destination),
            destination,
            entry.RemotePath,
            entry.SizeBytes ?? 0,
            File.Exists(destination) || Directory.Exists(destination));
        Status = TransferPreview.DestinationExists
            ? "The selected local path already exists. Choose a new path before downloading."
            : "Review the selected phone, remote file, local destination, and size before downloading.";
    }

    public void PrepareUpload(string localSource)
    {
        if (!CanChooseUpload || SelectedDevice is null) return;
        var source = new FileInfo(localSource);
        if (!source.Exists)
        {
            TransferPreview = null;
            Status = "The selected local file no longer exists.";
            return;
        }
        var fileName = source.Name;
        var existingNames = FileEntries.Select(entry => entry.Name).ToHashSet(StringComparer.Ordinal);
        var candidateName = fileName;
        for (var copyIndex = 2; existingNames.Contains(candidateName) && copyIndex < 10000; copyIndex++)
            candidateName = $"{Path.GetFileNameWithoutExtension(fileName)} ({copyIndex}){Path.GetExtension(fileName)}";
        var remotePath = AndroidSharedStoragePaths.CombineChild(RemoteFileDirectory, candidateName);
        TransferPreview = new FileTransferPreview(
            FileTransferDirection.Upload,
            SelectedDevice.Model,
            SelectedDevice.Serial,
            AndroidSharedStoragePaths.ToDisplayText(source.FullName),
            AndroidSharedStoragePaths.ToDisplayText(remotePath),
            source.FullName,
            remotePath,
            checked((ulong)source.Length),
            existingNames.Contains(candidateName));
        Status = TransferPreview.DestinationExists
            ? "Could not find an unused destination name. Refresh the folder and choose a different source name."
            : "Review the selected phone, local file, remote destination, and size before uploading.";
    }

    bool CanConfirmTransfer()
        => !IsBusy && SelectedDevice?.State == "Online" && TransferPreview is { DestinationExists: false } preview
           && string.Equals(SelectedDevice.Serial, preview.DeviceSerial, StringComparison.Ordinal);

    async Task ConfirmTransferAsync()
    {
        if (SelectedDevice is null || TransferPreview is not { DestinationExists: false } preview) return;
        var device = SelectedDevice;
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        TransferProgressPercent = 0;
        TransferProgressText = "Starting transfer.";
        Status = preview.Direction == FileTransferDirection.Upload
            ? "Uploading one file to the previewed shared-storage path."
            : "Downloading one file to the previewed local path.";
        var progress = new Progress<SyncProgressChangedEventArgs>(args =>
        {
            TransferProgressPercent = args.ProgressPercentage;
            TransferProgressText = $"{args.ProgressPercentage:N0}%  {args.ReceivedBytesSize:N0} of {args.TotalBytesToReceive:N0} bytes";
        });
        var succeeded = false;
        try
        {
            var plan = _workflow.CreateFileTransferPlan(device, preview);
            var approval = _approvalService.ApproveAfterUserConfirmation(plan);
            await _workflow.RecordApprovalGrantedAsync(plan);
            var result = preview.Direction == FileTransferDirection.Upload
                ? await _workflow.UploadSharedFileAsync(device, preview, progress, plan, approval, _activeOperation.Token)
                : await _workflow.DownloadSharedFileAsync(device, SelectedFileEntry!, preview, progress, plan, approval, _activeOperation.Token);
            TransferPreview = null;
            TransferProgressPercent = 100;
            TransferProgressText = $"Complete: {result.SizeBytes:N0} bytes.";
            Status = preview.Direction == FileTransferDirection.Upload
                ? "Upload completed and Android confirmed the final path and size."
                : "Download completed and the local file size matched the remote file.";
            succeeded = true;
        }
        catch (OperationCanceledException)
        {
            TransferProgressText = "Transfer cancelled. Newly created partial data was removed where possible.";
            Status = "Transfer cancelled. The original source and any pre-existing destination were left unchanged.";
        }
        catch (Exception ex)
        {
            TransferProgressText = "Transfer did not complete.";
            Status = $"Transfer failed: {ex.Message}";
        }
        finally
        {
            _activeOperation.Dispose();
            _activeOperation = null;
            IsBusy = false;
        }
        if (succeeded && preview.Direction == FileTransferDirection.Upload)
            await BrowseSharedStorageAsync(RemoteFileDirectory);
    }

    public Task OpenDirectoryAsync(AndroidFileEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return entry.CanNavigate ? BrowseSharedStorageAsync(entry.RemotePath) : Task.CompletedTask;
    }

    async Task BrowseSharedStorageAsync(string path)
    {
        if (SelectedDevice is null) return;
        string normalizedDirectory;
        try { normalizedDirectory = AndroidSharedStoragePaths.NormalizeDirectory(path); }
        catch (ArgumentException ex)
        {
            Status = $"Cannot open this shared-storage path: {ex.Message}";
            return;
        }

        var device = SelectedDevice;
        IsBusy = true;
        _activeOperation = new CancellationTokenSource();
        RaiseCommands();
        FileEntries.Clear();
        SelectedFileEntry = null;
        TransferPreview = null;
        RemoteFileDirectory = normalizedDirectory;
        FileListingLoaded = false;
        Status = $"Reading names and file metadata from {RemoteFileDirectoryDisplay}. File contents are not opened.";
        try
        {
            await foreach (var entry in _workflow.ListSharedStorageAsync(device, normalizedDirectory, _activeOperation.Token))
                FileEntries.Add(entry);

            FileListingLoaded = true;
            Status = $"Loaded {FileEntries.Count} items from {RemoteFileDirectoryDisplay}. No file contents were opened or changed.";
        }
        catch (OperationCanceledException)
        {
            FileListingLoaded = false;
            Status = $"Directory listing cancelled after {FileEntries.Count} items.";
        }
        catch
        {
            FileListingLoaded = false;
            Status = $"Could not list {RemoteFileDirectoryDisplay}. The folder may be unavailable or outside shared-storage access.";
        }
        finally
        {
            _activeOperation.Dispose();
            _activeOperation = null;
            IsBusy = false;
        }
    }

    public async Task PrepareInstallAsync(string apkPath)
    {
        if (SelectedDevice is null) return;
        IsBusy = true;
        InstallPreview = null;
        RemovalPreview = null;
        Status = "Inspecting APK metadata before showing the install preview.";
        try
        {
            InstallPreview = await _workflow.InspectApkAsync(SelectedDevice, apkPath, CancellationToken.None);
            _pendingApkPath = apkPath;
            Status = InstallPreview.AlreadyInstalled
                ? "Preview ready. This package is already installed, so installation is disabled."
                : "Review the APK, target phone, package ID, version, size, and SHA-256 before installing.";
        }
        catch (Exception ex)
        {
            _pendingApkPath = null;
            Status = $"APK inspection failed: {ex.Message}";
        }
        finally { IsBusy = false; }
    }

    async Task InstallApkAsync()
    {
        if (SelectedDevice is null || InstallPreview is null || _pendingApkPath is null) return;
        IsBusy = true;
        Status = $"Installing {InstallPreview.PackageId} on the selected phone.";
        try
        {
            var plan = _workflow.CreateInstallPlan(SelectedDevice, InstallPreview, _pendingApkPath);
            var approval = _approvalService.ApproveAfterUserConfirmation(plan);
            await _workflow.RecordApprovalGrantedAsync(plan);
            await _workflow.InstallApkAsync(SelectedDevice, InstallPreview, _pendingApkPath, plan, approval, CancellationToken.None);
            Status = $"Installed {InstallPreview.PackageId}. Android confirmed the previewed version.";
            InstallPreview = null;
            _pendingApkPath = null;
            AppsLoaded = false;
        }
        catch (Exception ex) { Status = $"Installation did not complete: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    void PreviewRemoval()
    {
        if (SelectedDevice is null || SelectedApp is null || !SelectedApp.IsThirdParty) return;
        RemovalPreview = new AppRemovalPreview(SelectedApp.PackageId, SelectedDevice.Model, SelectedDevice.Serial);
        InstallPreview = null;
        _pendingApkPath = null;
        Status = "Review the selected phone and user app, then choose Remove package to confirm.";
    }

    async Task RemoveAppAsync()
    {
        if (SelectedDevice is null || RemovalPreview is null) return;
        var device = SelectedDevice;
        var preview = RemovalPreview;
        if (!string.Equals(device.Serial, preview.DeviceSerial, StringComparison.Ordinal))
        {
            RemovalPreview = null;
            Status = "The selected phone changed. Prepare the removal preview again.";
            return;
        }
        IsBusy = true;
        Status = $"Removing {preview.PackageId} from the selected phone.";
        try
        {
            var plan = _workflow.CreateRemovalPlan(device, preview);
            var approval = _approvalService.ApproveAfterUserConfirmation(plan);
            await _workflow.RecordApprovalGrantedAsync(plan);
            await _workflow.RemoveThirdPartyAppAsync(device, preview.PackageId, plan, approval, CancellationToken.None);
            RemovalPreview = null;
            AppsLoaded = false;
            Status = $"Removed {preview.PackageId}. Android confirmed that it is no longer installed.";
        }
        catch (Exception ex) { Status = $"Removal did not complete: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    void RaiseCommands()
    {
        (RefreshCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ReadInfoCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (CaptureCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RefreshAppsCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ReadAppVersionCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (InstallApkCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (PreviewRemovalCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RemoveAppCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (LoadSharedStorageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ParentFolderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (RefreshFolderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ConfirmTransferCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (RefreshWindowsDevicesCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (RefreshFastbootDevicesCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ReadFastbootInfoCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(CanChooseDownload));
        OnPropertyChanged(nameof(CanChooseUpload));
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

    public void Dispose()
    {
        WpdBrowser.Dispose();
        _activeOperation?.Cancel();
        _activeOperation?.Dispose();
        _workflow.Dispose();
        _approvalService.Dispose();
    }
}
