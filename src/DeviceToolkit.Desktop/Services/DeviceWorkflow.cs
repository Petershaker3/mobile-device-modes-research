using System.Text;
using System.IO;
using System.Net;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.Models;
using logcat.Services;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using DeviceToolkit.Core.Services;
using DeviceToolkit.Core.Operations;
using DeviceToolkit.Core.Windows;
using DeviceToolkit.Wpd;

namespace DeviceToolkit.Desktop.Services;

public sealed record ApkInstallPreview(string FileName, string PackageId, string VersionName, string VersionCode, long SizeBytes, string Sha256, bool AlreadyInstalled);
public sealed record AppRemovalPreview(string PackageId, string DeviceModel, string DeviceSerial);
public sealed record DiagnosticResult(int Lines, int CrashEvents, int CrashGroups, DateTimeOffset Started, DateTimeOffset Finished);
public sealed record FileTransferResult(ulong SizeBytes, string DestinationPath);

/// <summary>Safe, local-only read path for the desktop app.</summary>
public sealed class DeviceWorkflow : IDisposable
{
    readonly SafeDeviceDiscovery _discovery = new();
    readonly ReadOnlyDeviceService _readService = new();
    readonly FastbootReadOnlyClient _fastbootClient = new();
    readonly PnpDeviceInventory _pnpInventory = new();
    readonly WpdDeviceFileProvider _wpdProvider = new();
    readonly ApprovalService _approvalService;
    readonly bool _ownsApprovalService;
    readonly SemaphoreSlim _auditStoreLock = new(1, 1);
    AuditStore? _auditStore;
    bool _disposed;

    public DeviceWorkflow(ApprovalService? approvalService = null, AuditStore? auditStore = null)
    {
        _ownsApprovalService = approvalService is null;
        _approvalService = approvalService ?? new ApprovalService();
        _auditStore = auditStore;
        CleanupStaleCaptures();
    }

    public Task RecordApprovalGrantedAsync(OperationPlan plan, CancellationToken ct = default)
        => RecordApprovalGrantedAsync(plan, "The desktop user approved the reviewed operation plan.", ct);

    async Task RecordApprovalGrantedAsync(OperationPlan plan, string summary, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(plan);
        var auditStore = await GetAuditStoreAsync(ct).ConfigureAwait(false);
        try
        {
            await auditStore.AppendAsync(new AuditEventDraft(
                "ApprovalGranted",
                new AuditActor("DesktopUser"),
                summary,
                plan.PlanId,
                plan.ImpactLevel.ToString()), ct).ConfigureAwait(false);
        }
        catch (Exception auditError)
        {
            throw new AuditOperationRecordException(operationStarted: false, operationCompleted: false,
                operationError: null, auditError: auditError);
        }
    }

    async Task<T> RunAuditedOperationAsync<T>(OperationPlan plan,
        Func<CancellationToken, Task<T>> operation, CancellationToken ct)
    {
        var auditStore = await GetAuditStoreAsync(ct).ConfigureAwait(false);
        return await auditStore.ExecuteOperationAsync(plan, operation, ct).ConfigureAwait(false);
    }

    async Task RunAuditedOperationAsync(OperationPlan plan,
        Func<CancellationToken, Task> operation, CancellationToken ct)
    {
        var auditStore = await GetAuditStoreAsync(ct).ConfigureAwait(false);
        await auditStore.ExecuteOperationAsync(plan, operation, ct).ConfigureAwait(false);
    }

    OperationPlan CreateReadPlan(string operationType, ConnectedDevice device)
        => _approvalService.CreatePlan(operationType, device.Serial, [], []);

    async Task<AuditStore> GetAuditStoreAsync(CancellationToken ct)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_auditStore is not null) return _auditStore;

        await _auditStoreLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_auditStore is not null) return _auditStore;
            var stateDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DeviceToolkit", "state");
            var databasePath = Path.Combine(stateDirectory, "audit.db");
            _auditStore = await AuditStore.OpenAsync(databasePath, cancellationToken: ct).ConfigureAwait(false);
            return _auditStore;
        }
        finally
        {
            _auditStoreLock.Release();
        }
    }

    public Task<IReadOnlyList<ConnectedDevice>> ListDevicesAsync(CancellationToken ct = default)
    {
        var plan = _approvalService.CreatePlan("device.discover", "local:adb-server", [], []);
        return RunAuditedOperationAsync(plan, async token =>
        {
            if (!AdbExecutableLocator.EnsureAvailable())
                throw new InvalidOperationException("Android Platform Tools (adb.exe) were not found.");
            AdbServer.Instance.StartServer("adb", false);
            return await Task.Run(_discovery.ListDevices, token).ConfigureAwait(false);
        }, ct);
    }

    public Task<IReadOnlyList<FastbootTarget>> ListFastbootDevicesAsync(CancellationToken ct = default)
    {
        var plan = _approvalService.CreatePlan("device.discover", "local:fastboot-server", [], []);
        return RunAuditedOperationAsync(plan, token => _fastbootClient.ListDevicesAsync(token), ct);
    }

    public Task<FastbootDeviceFacts> ReadFastbootFactsAsync(
        FastbootTarget target,
        FastbootDeviceProfile profile = FastbootDeviceProfile.Android,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(target);
        if (string.IsNullOrWhiteSpace(target.Serial))
            throw new ArgumentException("Select one Fastboot device before reading its information.", nameof(target));
        var plan = _approvalService.CreatePlan("device.read_info", target.Serial, [], []);
        return RunAuditedOperationAsync(plan, token => _fastbootClient.ReadFactsAsync(target, profile, token), ct);
    }

    public Task<IReadOnlyList<PnpDeviceSnapshot>> EnumerateWindowsDevicesAsync(CancellationToken ct = default)
    {
        var plan = _approvalService.CreatePlan("drivers.diagnose", "local:windows-pnp", [], []);
        return RunAuditedOperationAsync(plan,
            token => Task.Run(_pnpInventory.EnumeratePresentDevices, token), ct);
    }

    public Task<IReadOnlyList<WpdDeviceInfo>> EnumerateWpdDevicesAsync(CancellationToken ct = default)
    {
        var plan = _approvalService.CreatePlan("device.discover", "local:windows-wpd", [], []);
        return RunAuditedOperationAsync(plan, token => _wpdProvider.EnumerateDevicesAsync(token), ct);
    }

    public Task<WpdReadOnlySession> OpenWpdReadOnlySessionAsync(WpdDeviceInfo device, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(device);
        var plan = CreateWpdReadPlan(device, "session.open", string.Empty);
        return RunAuditedOperationAsync(plan, token => _wpdProvider.OpenReadOnlyAsync(device.SelectionId, token), ct);
    }

    public Task<WpdDeviceDetails> ReadWpdDeviceDetailsAsync(WpdDeviceInfo device, WpdReadOnlySession session, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(session);
        var plan = CreateWpdReadPlan(device, "device.details", "metadata");
        return RunAuditedOperationAsync(plan, token => session.ReadDeviceDetailsAsync(token), ct);
    }

    public Task<IReadOnlyList<WpdFileEntry>> ListWpdDirectoryAsync(
        WpdDeviceInfo device,
        WpdReadOnlySession session,
        Guid directoryId,
        string directoryPath,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(session);
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);
        var plan = CreateWpdReadPlan(device, "directory.list", directoryPath);
        return RunAuditedOperationAsync<IReadOnlyList<WpdFileEntry>>(plan, async token =>
        {
            var directories = await session.ListDirectoriesAsync(directoryId, token).ConfigureAwait(false);
            var files = await session.ListFilesAsync(directoryId, token).ConfigureAwait(false);
            return directories.Concat(files).ToArray();
        }, ct);
    }

    public OperationPlan CreateWpdDownloadPlan(
        WpdDeviceInfo device,
        WpdFileEntry file,
        string devicePath,
        string localDestination)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(file);
        ArgumentException.ThrowIfNullOrWhiteSpace(devicePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(localDestination);
        if (file.IsDirectory || file.SizeBytes is null)
            throw new InvalidOperationException("Only a regular WPD file with a known size can be previewed for download.");
        var fullDestination = Path.GetFullPath(localDestination);
        var target = $"wpd:{device.SelectionId:N}";
        return _approvalService.CreatePlan("file.pull", target,
        [
            KeyValuePair.Create("direction", "WpdDownload"),
            KeyValuePair.Create("devicePath", devicePath),
            KeyValuePair.Create("localPath", fullDestination),
            KeyValuePair.Create("sizeBytes", file.SizeBytes.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            KeyValuePair.Create("destinationExists", (File.Exists(fullDestination) || Directory.Exists(fullDestination)).ToString().ToLowerInvariant())
        ], ["VerifyLocalSize"]);
    }

    public Task<WpdDownloadResult> DownloadWpdFileAsync(
        WpdDeviceInfo device,
        WpdReadOnlySession session,
        WpdFileEntry file,
        string devicePath,
        string localDestination,
        OperationPlan plan,
        OperationApproval approval,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(approval);
        var fullDestination = Path.GetFullPath(localDestination);
        var expectedInputs = new[]
        {
            KeyValuePair.Create("direction", "WpdDownload"),
            KeyValuePair.Create("devicePath", devicePath),
            KeyValuePair.Create("localPath", fullDestination),
            KeyValuePair.Create("sizeBytes", (file.SizeBytes ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture)),
            KeyValuePair.Create("destinationExists", (File.Exists(fullDestination) || Directory.Exists(fullDestination)).ToString().ToLowerInvariant())
        };
        var target = $"wpd:{device.SelectionId:N}";
        _approvalService.ValidateAndConsumeForOperation(plan, approval, "file.pull", target, expectedInputs, ["VerifyLocalSize"]);
        return RunAuditedOperationAsync(plan, async token =>
        {
            if (file.IsDirectory || file.SizeBytes is null)
                throw new InvalidOperationException("Only a regular WPD file with a known size can be downloaded.");
            if (File.Exists(fullDestination) || Directory.Exists(fullDestination))
                throw new IOException("The local destination already exists. Choose a new destination before downloading.");
            var result = await session.DownloadFileAsync(file.EntryId, fullDestination, token).ConfigureAwait(false);
            if (result.SizeBytes != file.SizeBytes.Value)
            {
                try { File.Delete(fullDestination); } catch { }
                throw new IOException("The downloaded byte count did not match the previewed WPD file size.");
            }
            return result;
        }, ct);
    }

    OperationPlan CreateWpdReadPlan(WpdDeviceInfo device, string action, string path)
        => _approvalService.CreatePlan("file.list", $"wpd:{device.SelectionId:N}",
        [KeyValuePair.Create("provider", "WPD"), KeyValuePair.Create("action", action), KeyValuePair.Create("path", path)], []);

    public async Task<string> PairAndConnectWirelessAsync(string pairingCode, CancellationToken ct)
    {
        WirelessAdbAddress.ValidatePairingCode(pairingCode);
        var discoveryPlan = _approvalService.CreatePlan("device.wireless_discover", "local:adb-server", [], []);
        var services = await RunAuditedOperationAsync(discoveryPlan, async token =>
        {
            if (!AdbExecutableLocator.EnsureAvailable())
                throw new InvalidOperationException("Android Platform Tools (adb.exe) were not found.");
            AdbServer.Instance.StartServer("adb", false);
            return await ReadWirelessAdbServicesAsync(token).ConfigureAwait(false);
        }, ct).ConfigureAwait(false);

        var pairingServices = services.Where(service => service.Kind == WirelessAdbServiceKind.Pairing).ToArray();
        if (pairingServices.Length == 0)
            throw new InvalidOperationException("On the phone, open Wireless debugging and tap Pair device with pairing code, then enter the current code here.");
        if (pairingServices.Length > 1)
            throw new InvalidOperationException("More than one phone is advertising a pairing screen. Close the other pairing screens and retry.");

        var pairingEndpoint = pairingServices[0].Endpoint;
        var plan = _approvalService.CreatePlan("device.wireless_pair", pairingEndpoint.ToString(),
            [KeyValuePair.Create("action", "pair-and-connect")],
            ["ConfirmPairingResponse", "ConfirmConnectionResponse"]);
        var approval = _approvalService.ApproveAfterUserConfirmation(plan);
        await RecordApprovalGrantedAsync(plan,
            "The desktop user initiated wireless pairing with a one-time code from the phone.", ct).ConfigureAwait(false);

        return await RunAuditedOperationAsync(plan,
            token => PairAndConnectWirelessCoreAsync(pairingEndpoint, pairingCode, plan, approval, token), ct)
            .ConfigureAwait(false);
    }

    async Task<string> PairAndConnectWirelessCoreAsync(IPEndPoint pairingEndpoint, string pairingCode, OperationPlan plan,
        OperationApproval approval, CancellationToken ct)
    {
        _approvalService.ValidateAndConsumeForOperation(plan, approval, "device.wireless_pair", pairingEndpoint.ToString(),
            [KeyValuePair.Create("action", "pair-and-connect")],
            ["ConfirmPairingResponse", "ConfirmConnectionResponse"]);
        var client = new AdbClient();
        var response = await client.PairAsync(
            pairingEndpoint.Address.ToString(), pairingEndpoint.Port, pairingCode, ct).ConfigureAwait(false);
        if (!WirelessAdbAddress.IsPairingSuccess(response))
            throw new InvalidOperationException("ADB did not confirm pairing. Check the current code on the phone and retry.");

        IPEndPoint connectEndpoint;
        try
        {
            connectEndpoint = await WirelessAdbConnectDiscovery.WaitForMatchingConnectServiceAsync(
                ReadWirelessAdbServicesAsync,
                pairingEndpoint.Address,
                TimeSpan.FromSeconds(20),
                TimeSpan.FromMilliseconds(500),
                ct).ConfigureAwait(false);
        }
        catch (TimeoutException ex)
        {
            throw new InvalidOperationException(
                "Pairing succeeded, but the phone did not advertise its connection service within 20 seconds. Keep Wireless debugging enabled, leave the Wireless debugging screen open, and retry.", ex);
        }

        var connectResponse = await client.ConnectAsync(
            connectEndpoint.Address.ToString(), connectEndpoint.Port, ct).ConfigureAwait(false);
        if (!WirelessAdbAddress.IsConnectSuccess(connectResponse))
            throw new InvalidOperationException("Pairing succeeded, but ADB did not confirm the connection. Keep Wireless debugging open and retry.");
        return connectEndpoint.ToString();
    }

    static async Task<IReadOnlyList<WirelessAdbService>> ReadWirelessAdbServicesAsync(CancellationToken ct)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "adb",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.GetTempPath()
        };
        process.StartInfo.ArgumentList.Add("mdns");
        process.StartInfo.ArgumentList.Add("services");
        if (!process.Start()) throw new InvalidOperationException("ADB mDNS discovery could not start.");

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(8));
        var outputTask = process.StandardOutput.ReadToEndAsync(timeout.Token);
        var errorTask = process.StandardError.ReadToEndAsync(timeout.Token);
        try { await process.WaitForExitAsync(timeout.Token).ConfigureAwait(false); }
        catch
        {
            try { process.Kill(entireProcessTree: true); } catch { }
            throw;
        }

        var output = await outputTask.ConfigureAwait(false);
        _ = await errorTask.ConfigureAwait(false);
        if (process.ExitCode != 0 || output.Length > 64 * 1024)
            throw new InvalidOperationException("ADB could not discover Wireless debugging services. Check Android Platform Tools and retry.");
        return WirelessAdbAddress.ParseMdnsServicesOutput(output);
    }

    public Task<DeviceFacts> ReadFactsAsync(ConnectedDevice device, CancellationToken ct)
    {
        ValidateDevice(device);
        var plan = CreateReadPlan("device.read_info", device);
        return RunAuditedOperationAsync(plan, token => ReadFactsCoreAsync(device, token), ct);
    }

    async Task<DeviceFacts> ReadFactsCoreAsync(ConnectedDevice device, CancellationToken ct)
    {
        if (!AdbExecutableLocator.EnsureAvailable())
            throw new InvalidOperationException("Android Platform Tools (adb.exe) were not found.");
        return await _readService.ReadFactsAsync(device, ct).ConfigureAwait(false);
    }

    public Task<IReadOnlyList<InstalledApp>> ReadInstalledAppsAsync(ConnectedDevice device, CancellationToken ct)
    {
        ValidateDevice(device);
        var plan = CreateReadPlan("app.list", device);
        return RunAuditedOperationAsync(plan, token => _readService.ReadInstalledAppsAsync(device, token), ct);
    }

    public Task<AppVersion> ReadAppVersionAsync(ConnectedDevice device, InstalledApp app, CancellationToken ct)
    {
        ValidateDevice(device);
        var plan = CreateReadPlan("app.details", device);
        return RunAuditedOperationAsync(plan, token => _readService.ReadAppVersionAsync(device, app, token), ct);
    }

    public Task<ApkInstallPreview> InspectApkAsync(ConnectedDevice device, string apkPath, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(device);
        ValidateDevice(device);
        var plan = CreateReadPlan("package.inspect", device);
        return RunAuditedOperationAsync(plan, token => InspectApkCoreAsync(device, apkPath, token), ct);
    }

    async Task<ApkInstallPreview> InspectApkCoreAsync(ConnectedDevice device, string apkPath, CancellationToken ct)
    {
        var file = new FileInfo(apkPath);
        if (!file.Exists || !string.Equals(file.Extension, ".apk", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Choose an existing .apk file.");
        if (file.Length is <= 0 or > 1024L * 1024 * 1024)
            throw new InvalidDataException("APK size must be between 1 byte and 1 GiB.");

        var analyzer = ApkAnalyzerLocator.Find()
            ?? throw new InvalidOperationException("Android SDK apkanalyzer was not found. Install Android SDK Command-line Tools.");
        if (ApkAnalyzerLocator.FindJavaExecutable() is null)
            throw new InvalidOperationException("Java 17 or newer was not found. Android SDK apkanalyzer needs a Java runtime.");
        var packageId = await RunApkAnalyzerAsync(analyzer, "application-id", file.FullName, ct);
        var versionName = await RunApkAnalyzerAsync(analyzer, "version-name", file.FullName, ct);
        var versionCode = await RunApkAnalyzerAsync(analyzer, "version-code", file.FullName, ct);
        if (!Regex.IsMatch(packageId, "^[A-Za-z0-9_]+(\\.[A-Za-z0-9_]+)+$", RegexOptions.CultureInvariant))
            throw new InvalidDataException("The APK manifest does not contain a valid Android package ID.");

        await using var apkStream = File.OpenRead(file.FullName);
        var digest = Convert.ToHexString(await System.Security.Cryptography.SHA256.HashDataAsync(apkStream, ct));
        var installed = await ReadInstalledAppsAsync(device, ct);
        return new ApkInstallPreview(file.Name, packageId, versionName, versionCode, file.Length, digest,
            installed.Any(app => string.Equals(app.PackageId, packageId, StringComparison.Ordinal)));
    }

    /*
     * Each read plan stores the HMAC-bound target only in memory. Audit events
     * contain the operation type and outcome, never raw serials or result data.
     */

    public OperationPlan CreateInstallPlan(ConnectedDevice device, ApkInstallPreview preview, string apkPath)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(preview);
        ValidateDevice(device);
        return _approvalService.CreatePlan("app.install", device.Serial,
        [
            KeyValuePair.Create("fileName", preview.FileName),
            KeyValuePair.Create("localPath", Path.GetFullPath(apkPath)),
            KeyValuePair.Create("packageId", preview.PackageId),
            KeyValuePair.Create("sha256", preview.Sha256),
            KeyValuePair.Create("sizeBytes", preview.SizeBytes.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            KeyValuePair.Create("versionCode", preview.VersionCode),
            KeyValuePair.Create("versionName", preview.VersionName)
        ], ["VerifyPackageState"]);
    }

    public OperationPlan CreateRemovalPlan(ConnectedDevice device, AppRemovalPreview preview)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(preview);
        ValidateDevice(device);
        if (!string.Equals(device.Serial, preview.DeviceSerial, StringComparison.Ordinal))
            throw new InvalidOperationException("The selected phone changed. Prepare the removal preview again.");
        return _approvalService.CreatePlan("app.uninstall", device.Serial,
        [
            KeyValuePair.Create("packageId", preview.PackageId),
            KeyValuePair.Create("thirdPartyOnly", "true")
        ], ["VerifyPackageAbsent"]);
    }

    public Task InstallApkAsync(ConnectedDevice device, ApkInstallPreview preview, string apkPath,
        OperationPlan plan, OperationApproval approval, CancellationToken ct)
        => RunAuditedOperationAsync(plan, token => InstallApkCoreAsync(device, preview, apkPath, plan, approval, token), ct);

    async Task InstallApkCoreAsync(ConnectedDevice device, ApkInstallPreview preview, string apkPath,
        OperationPlan plan, OperationApproval approval, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(preview);
        ValidateDevice(device);
        if (!Regex.IsMatch(preview.PackageId, "^[A-Za-z0-9_]+(\\.[A-Za-z0-9_]+)+$", RegexOptions.CultureInvariant))
            throw new InvalidOperationException("The preview package ID is invalid.");

        var file = new FileInfo(apkPath);
        if (!file.Exists || file.Length != preview.SizeBytes || !string.Equals(file.Name, preview.FileName, StringComparison.Ordinal))
            throw new InvalidOperationException("The APK file changed after preview. Inspect it again before installing.");
        await using (var stream = File.OpenRead(file.FullName))
        {
            var currentDigest = Convert.ToHexString(await System.Security.Cryptography.SHA256.HashDataAsync(stream, ct));
            if (!string.Equals(currentDigest, preview.Sha256, StringComparison.Ordinal))
                throw new InvalidOperationException("The APK file changed after preview. Inspect it again before installing.");
        }
        if (preview.AlreadyInstalled)
            throw new InvalidOperationException("This package is already installed. Updates are not included in this workflow.");

        _approvalService.ValidateAndConsumeForOperation(plan, approval, "app.install", device.Serial,
        [
            KeyValuePair.Create("fileName", preview.FileName),
            KeyValuePair.Create("localPath", Path.GetFullPath(apkPath)),
            KeyValuePair.Create("packageId", preview.PackageId),
            KeyValuePair.Create("sha256", preview.Sha256),
            KeyValuePair.Create("sizeBytes", preview.SizeBytes.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            KeyValuePair.Create("versionCode", preview.VersionCode),
            KeyValuePair.Create("versionName", preview.VersionName)
        ], ["VerifyPackageState"]);

        var client = new AdbClient();
        var target = GetOnlineTarget(client, device);
        var stagedName = $"device-toolkit-{Guid.NewGuid():N}.apk";
        var stagedPath = Path.Combine(Path.GetTempPath(), stagedName);
        var remotePath = $"/data/local/tmp/{stagedName}";
        Exception? installError = null;
        try
        {
            File.Copy(file.FullName, stagedPath, overwrite: false);
            var packageManager = new PackageManager(client, target, skipInit: true);
            await packageManager.InstallPackageAsync(stagedPath, (Action<InstallProgressEventArgs>?)null, ct);
        }
        catch (Exception ex) { installError = ex; }
        finally
        {
            try { File.Delete(stagedPath); } catch { }
            using var cleanupTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            try { await client.ExecuteRemoteCommandAsync($"rm -f {remotePath}", target, cleanupTimeout.Token); } catch { }
        }

        var verification = new PackageManager(client, target, skipInit: true);
        await verification.RefreshPackagesAsync(ct);
        if (!verification.Packages.ContainsKey(preview.PackageId))
        {
            if (installError is not null)
                throw new InvalidOperationException("ADB reported an install error and the package is not present. Refresh the inventory before retrying.", installError);
            throw new InvalidOperationException("Android reported install completion, but the package is not present.");
        }
        var installed = await verification.GetVersionInfoAsync(preview.PackageId, ct);
        if (installed.VersionCode.ToString(System.Globalization.CultureInfo.InvariantCulture) != preview.VersionCode ||
            !string.Equals(installed.VersionName, preview.VersionName, StringComparison.Ordinal))
            throw new InvalidOperationException("The package appeared on the phone, but its installed version did not match the preview.");
    }

    public Task RemoveThirdPartyAppAsync(ConnectedDevice device, string packageId,
        OperationPlan plan, OperationApproval approval, CancellationToken ct)
        => RunAuditedOperationAsync(plan, token => RemoveThirdPartyAppCoreAsync(device, packageId, plan, approval, token), ct);

    async Task RemoveThirdPartyAppCoreAsync(ConnectedDevice device, string packageId,
        OperationPlan plan, OperationApproval approval, CancellationToken ct)
    {
        ValidateDevice(device);
        if (!Regex.IsMatch(packageId, "^[A-Za-z0-9_]+(\\.[A-Za-z0-9_]+)+$", RegexOptions.CultureInvariant))
            throw new InvalidOperationException("The selected package ID is invalid.");
        var client = new AdbClient();
        var target = GetOnlineTarget(client, device);
        var thirdParty = new PackageManager(client, target, arguments: ["-3"], skipInit: true);
        await thirdParty.RefreshPackagesAsync(ct);
        if (!thirdParty.Packages.ContainsKey(packageId))
            throw new InvalidOperationException("Only a package currently listed by Android as a user app can be removed here.");
        _approvalService.ValidateAndConsumeForOperation(plan, approval, "app.uninstall", device.Serial,
        [
            KeyValuePair.Create("packageId", packageId),
            KeyValuePair.Create("thirdPartyOnly", "true")
        ], ["VerifyPackageAbsent"]);
        var manager = new PackageManager(client, target, skipInit: true);
        await manager.UninstallPackageAsync(packageId, ct);
        await manager.RefreshPackagesAsync(ct);
        if (manager.Packages.ContainsKey(packageId))
            throw new InvalidOperationException("Android reported removal, but the package is still present.");
    }

    static DeviceData GetOnlineTarget(AdbClient client, ConnectedDevice device) =>
        client.GetDevices().SingleOrDefault(candidate => candidate.Serial == device.Serial && candidate.State == DeviceState.Online)
        ?? throw new InvalidOperationException("The selected device is no longer online.");

    static async Task<string> RunApkAnalyzerAsync(string analyzer, string subcommand, string apkPath, CancellationToken ct)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = ApkAnalyzerLocator.FindJavaExecutable()!,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.GetTempPath()
        };
        process.StartInfo.ArgumentList.Add($"-Dcom.android.sdklib.toolsdir={Directory.GetParent(Path.GetDirectoryName(analyzer)!)!.FullName}");
        process.StartInfo.ArgumentList.Add("-classpath");
        process.StartInfo.ArgumentList.Add(analyzer);
        process.StartInfo.ArgumentList.Add("com.android.tools.apk.analyzer.ApkAnalyzerCli");
        process.StartInfo.ArgumentList.Add("manifest");
        process.StartInfo.ArgumentList.Add(subcommand);
        process.StartInfo.ArgumentList.Add(apkPath);
        if (!process.Start()) throw new InvalidOperationException("Android SDK APK inspection could not start.");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(20));
        var outputTask = process.StandardOutput.ReadToEndAsync(timeout.Token);
        var errorTask = process.StandardError.ReadToEndAsync(timeout.Token);
        try { await process.WaitForExitAsync(timeout.Token); }
        catch { try { process.Kill(entireProcessTree: true); } catch { } throw; }
        var output = await outputTask;
        _ = await errorTask;
        if (process.ExitCode != 0 || output.Length > 4096)
            throw new InvalidDataException("Android SDK could not read the APK manifest.");
        return output.Trim();
    }

    public async IAsyncEnumerable<AndroidFileEntry> ListSharedStorageAsync(
        ConnectedDevice device,
        string remoteDirectory,
        [EnumeratorCancellation] CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(device);
        ValidateDevice(device);
        var normalizedDirectory = AndroidSharedStoragePaths.NormalizeDirectory(remoteDirectory);
        var plan = CreateReadPlan("file.list", device);
        var auditStore = await GetAuditStoreAsync(ct).ConfigureAwait(false);
        await foreach (var entry in auditStore.ExecuteStreamOperationAsync(plan,
                           token => ListSharedStorageCoreAsync(device, normalizedDirectory, token), ct)
                           .ConfigureAwait(false))
            yield return entry;
    }

    async IAsyncEnumerable<AndroidFileEntry> ListSharedStorageCoreAsync(
        ConnectedDevice device,
        string normalizedDirectory,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var client = new AdbClient();
        var target = GetOnlineTarget(client, device);
        await foreach (var stat in client.GetDirectoryAsyncListingEx(target, normalizedDirectory, ct).ConfigureAwait(false))
        {
            ct.ThrowIfCancellationRequested();
            var entry = AndroidFileEntryProjection.FromSyncEntry(normalizedDirectory, stat);
            if (entry is not null) yield return entry;
        }
    }

    public OperationPlan CreateFileTransferPlan(ConnectedDevice device, FileTransferPreview preview)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(preview);
        ValidateDevice(device);
        if (!string.Equals(device.Serial, preview.DeviceSerial, StringComparison.Ordinal) || preview.DestinationExists)
            throw new InvalidOperationException("The selected phone or destination changed. Prepare the transfer preview again.");
        var operationType = preview.Direction switch
        {
            FileTransferDirection.Upload => "file.push",
            FileTransferDirection.Download => "file.pull",
            _ => throw new ArgumentOutOfRangeException(nameof(preview))
        };
        var verification = preview.Direction == FileTransferDirection.Upload
            ? "VerifyRemoteSizeAndFinalPath"
            : "VerifyLocalSize";
        return _approvalService.CreatePlan(operationType, device.Serial,
        [
            KeyValuePair.Create("direction", preview.Direction.ToString()),
            KeyValuePair.Create("localPath", Path.GetFullPath(preview.LocalPath)),
            KeyValuePair.Create("remotePath", AndroidFileTransferPaths.NormalizeSharedFile(preview.RemotePath)),
            KeyValuePair.Create("sizeBytes", preview.SizeBytes.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            KeyValuePair.Create("destinationExists", "false")
        ], [verification]);
    }

    public Task<FileTransferResult> DownloadSharedFileAsync(
        ConnectedDevice device,
        AndroidFileEntry entry,
        FileTransferPreview preview,
        IProgress<SyncProgressChangedEventArgs>? progress,
        OperationPlan plan,
        OperationApproval approval,
        CancellationToken ct)
        => RunAuditedOperationAsync(plan,
            token => DownloadSharedFileCoreAsync(device, entry, preview, progress, plan, approval, token), ct);

    async Task<FileTransferResult> DownloadSharedFileCoreAsync(
        ConnectedDevice device,
        AndroidFileEntry entry,
        FileTransferPreview preview,
        IProgress<SyncProgressChangedEventArgs>? progress,
        OperationPlan plan,
        OperationApproval approval,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(preview);
        ValidateDevice(device);
        if (preview.Direction != FileTransferDirection.Download ||
            !string.Equals(preview.DeviceSerial, device.Serial, StringComparison.Ordinal) ||
            !string.Equals(preview.RemotePath, entry.RemotePath, StringComparison.Ordinal) ||
            preview.SizeBytes != entry.SizeBytes || preview.DestinationExists)
            throw new InvalidOperationException("The file transfer no longer matches its preview. Review it again before downloading.");
        if (entry.Kind != AndroidFileKind.File || entry.Error != UnixErrorCode.Default || entry.SizeBytes is null)
            throw new InvalidOperationException("Only a listed regular file can be downloaded.");
        var remotePath = AndroidFileTransferPaths.NormalizeSharedFile(entry.RemotePath);
        if (File.Exists(preview.LocalPath) || Directory.Exists(preview.LocalPath))
            throw new IOException("The local destination already exists. Choose a new destination before downloading.");

        var client = new AdbClient();
        var target = GetOnlineTarget(client, device);
        using var sync = new SyncService(client, target);
        var current = await sync.StatExAsync(remotePath, ct).ConfigureAwait(false);
        if (current.Error != UnixErrorCode.Default ||
            (current.FileMode & UnixFileStatus.TypeMask) != UnixFileStatus.Regular ||
            current.Size != entry.SizeBytes.Value || current.Size != preview.SizeBytes)
            throw new IOException("The remote file changed or is no longer a regular file. Refresh the folder and review it again.");

        _approvalService.ValidateAndConsumeForOperation(plan, approval, "file.pull", device.Serial,
        [
            KeyValuePair.Create("direction", preview.Direction.ToString()),
            KeyValuePair.Create("localPath", Path.GetFullPath(preview.LocalPath)),
            KeyValuePair.Create("remotePath", remotePath),
            KeyValuePair.Create("sizeBytes", preview.SizeBytes.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            KeyValuePair.Create("destinationExists", "false")
        ], ["VerifyLocalSize"]);

        var fullDestination = Path.GetFullPath(preview.LocalPath);
        var directory = Path.GetDirectoryName(fullDestination)
            ?? throw new ArgumentException("The local destination must include a parent directory.", nameof(preview.LocalPath));
        if (!Directory.Exists(directory)) throw new DirectoryNotFoundException("The selected local destination folder no longer exists.");
        var stagingPath = Path.Combine(directory, $".device-toolkit-{Guid.NewGuid():N}.partial");
        try
        {
            await using (var output = new FileStream(stagingPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                             1024 * 1024, FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await sync.PullAsync(remotePath, output,
                    progress is null ? null : args => progress.Report(args),
                    useV2: true, cancellationToken: ct).ConfigureAwait(false);
                await output.FlushAsync(ct).ConfigureAwait(false);
                output.Flush(flushToDisk: true);
                if ((ulong)output.Length != current.Size)
                    throw new IOException("The downloaded byte count does not match the remote file size.");
            }

            ct.ThrowIfCancellationRequested();
            File.Move(stagingPath, fullDestination, overwrite: false);
            return new FileTransferResult(current.Size, fullDestination);
        }
        finally
        {
            try { File.Delete(stagingPath); } catch { }
        }
    }

    public Task<FileTransferResult> UploadSharedFileAsync(
        ConnectedDevice device,
        FileTransferPreview preview,
        IProgress<SyncProgressChangedEventArgs>? progress,
        OperationPlan plan,
        OperationApproval approval,
        CancellationToken ct)
        => RunAuditedOperationAsync(plan,
            token => UploadSharedFileCoreAsync(device, preview, progress, plan, approval, token), ct);

    async Task<FileTransferResult> UploadSharedFileCoreAsync(
        ConnectedDevice device,
        FileTransferPreview preview,
        IProgress<SyncProgressChangedEventArgs>? progress,
        OperationPlan plan,
        OperationApproval approval,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(preview);
        ValidateDevice(device);
        if (preview.Direction != FileTransferDirection.Upload ||
            !string.Equals(preview.DeviceSerial, device.Serial, StringComparison.Ordinal) || preview.DestinationExists)
            throw new InvalidOperationException("The file transfer no longer matches its preview. Review it again before uploading.");
        var source = new FileInfo(preview.LocalPath);
        if (!source.Exists) throw new FileNotFoundException("The selected local file no longer exists.", preview.LocalPath);
        var destination = AndroidFileTransferPaths.NormalizeSharedFile(preview.RemotePath);
        var parent = AndroidSharedStoragePaths.GetParent(destination)
            ?? throw new ArgumentException("The remote destination must be inside a shared-storage folder.", nameof(preview.RemotePath));
        var targetName = destination[(parent.Length + 1)..];
        var size = checked((ulong)source.Length);
        if (size != preview.SizeBytes)
            throw new IOException("The selected local file size changed after its transfer preview was prepared.");
        var stage = AndroidFileTransferPaths.CreateStagingPath(destination, Guid.NewGuid());
        var stageName = stage[(parent.Length + 1)..];
        var client = new AdbClient();
        var target = GetOnlineTarget(client, device);
        var shell = new AdbShell();
        var committed = false;
        Exception? transferFailure = null;

        try
        {
            if (await FindSharedStorageEntryAsync(device, parent, targetName, ct).ConfigureAwait(false) is not null)
                throw new IOException("The remote destination already exists. Choose another name and prepare a new preview.");
            if (await FindSharedStorageEntryAsync(device, parent, stageName, ct).ConfigureAwait(false) is not null)
                throw new IOException("A temporary upload path already exists. Retry the operation to create a new path.");

            _approvalService.ValidateAndConsumeForOperation(plan, approval, "file.push", device.Serial,
            [
                KeyValuePair.Create("direction", preview.Direction.ToString()),
                KeyValuePair.Create("localPath", Path.GetFullPath(preview.LocalPath)),
                KeyValuePair.Create("remotePath", destination),
                KeyValuePair.Create("sizeBytes", preview.SizeBytes.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                KeyValuePair.Create("destinationExists", "false")
            ], ["VerifyRemoteSizeAndFinalPath"]);

            var mode = UnixFileStatus.Regular | UnixFileStatus.UserRead | UnixFileStatus.UserWrite
                       | UnixFileStatus.GroupRead | UnixFileStatus.OtherRead;
            await using (var input = new FileStream(source.FullName, FileMode.Open, FileAccess.Read, FileShare.Read,
                             1024 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan))
            using (var sync = new SyncService(client, target))
            {
                if ((ulong)input.Length != size)
                    throw new IOException("The selected local file changed after its transfer preview was prepared.");
                await sync.PushAsync(input, stage, mode, new DateTimeOffset(source.LastWriteTimeUtc, TimeSpan.Zero),
                    progress is null ? null : args => progress.Report(args),
                    useV2: true, cancellationToken: ct).ConfigureAwait(false);
            }

            var stagedEntry = await FindSharedStorageEntryAsync(device, parent, stageName, ct).ConfigureAwait(false);
            if (stagedEntry is null || stagedEntry.Kind != AndroidFileKind.File || stagedEntry.SizeBytes != size)
                throw new IOException("The staged upload did not match the selected local file size.");

            ct.ThrowIfCancellationRequested();
            var moveCommand = $"mv -n {AndroidFileTransferPaths.QuotePosixShellArgument(stage)} {AndroidFileTransferPaths.QuotePosixShellArgument(destination)}";
            await shell.RunAsync(device.Serial, moveCommand, TimeSpan.FromSeconds(15), ct).ConfigureAwait(false);

            var finalEntry = await FindSharedStorageEntryAsync(device, parent, targetName, ct).ConfigureAwait(false);
            var remainingStage = await FindSharedStorageEntryAsync(device, parent, stageName, ct).ConfigureAwait(false);
            if (remainingStage is not null || finalEntry?.Kind != AndroidFileKind.File || finalEntry.SizeBytes != size)
                throw new IOException("Android did not confirm a complete upload at the previewed destination.");

            committed = true;
            return new FileTransferResult(size, destination);
        }
        catch (Exception ex)
        {
            transferFailure = ex;
        }

        if (!committed)
        {
            try
            {
                using var cleanupDeadline = new CancellationTokenSource(TimeSpan.FromSeconds(8));
                var cleanupCommand = $"rm -f {AndroidFileTransferPaths.QuotePosixShellArgument(stage)}";
                await shell.RunAsync(device.Serial, cleanupCommand, TimeSpan.FromSeconds(7), cleanupDeadline.Token).ConfigureAwait(false);
                if (await FindSharedStorageEntryAsync(device, parent, stageName, cleanupDeadline.Token).ConfigureAwait(false) is not null)
                    throw new IOException("The temporary upload remains on the device.");
            }
            catch (Exception cleanupError)
            {
                throw new IOException($"The upload did not finish, and Android could not confirm cleanup of {stage}. Inspect this path before retrying.",
                    new AggregateException(transferFailure!, cleanupError));
            }
        }

        ExceptionDispatchInfo.Capture(transferFailure!).Throw();
        throw new InvalidOperationException("Unreachable transfer state.");
    }

    async Task<AndroidFileEntry?> FindSharedStorageEntryAsync(
        ConnectedDevice device,
        string directory,
        string name,
        CancellationToken ct)
    {
        await foreach (var entry in ListSharedStorageAsync(device, directory, ct).ConfigureAwait(false))
            if (string.Equals(entry.Name, name, StringComparison.Ordinal)) return entry;
        return null;
    }

    public Task<DiagnosticResult> CaptureAndAnalyzeAsync(
        ConnectedDevice device, TimeSpan duration, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(device);
        ValidateDevice(device);
        if (duration <= TimeSpan.Zero || duration > TimeSpan.FromMinutes(2))
            throw new ArgumentOutOfRangeException(nameof(duration));
        var plan = CreateReadPlan("diagnostics.logcat_capture", device);
        return RunAuditedOperationAsync(plan, token => CaptureAndAnalyzeCoreAsync(device, duration, token), ct);
    }

    async Task<DiagnosticResult> CaptureAndAnalyzeCoreAsync(
        ConnectedDevice device, TimeSpan duration, CancellationToken ct)
    {
        var path = Path.Combine(Path.GetTempPath(), $"device-toolkit-capture-{Guid.NewGuid():N}.log");
        var started = DateTimeOffset.UtcNow;
        var capture = new BoundedLogcatCapture(device.Serial, path);
        try
        {
            var elapsed = await capture.RunAsync(duration, ct);
            if (elapsed + TimeSpan.FromSeconds(1) < duration)
                throw new InvalidOperationException("The selected device disconnected before the capture window completed.");
            var finished = DateTimeOffset.UtcNow;
            using var document = await logcat.Services.LogDocument.BuildAsync(path, ct: ct);
            var groups = CrashAggregator.Aggregate(document.Crashes, document.ExcTypes, document.Pkgs);
            return new DiagnosticResult(document.RowCount, document.Crashes.Count, groups.Count, started, finished);
        }
        finally
        {
            try { File.Delete(path); } catch { }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _wpdProvider.Dispose();
        if (_ownsApprovalService) _approvalService.Dispose();
    }

    static void ValidateDevice(ConnectedDevice device)
    {
        if (device.State != "Online" || !Regex.IsMatch(device.Serial, "^[A-Za-z0-9._:-]{1,128}$", RegexOptions.CultureInvariant))
            throw new InvalidOperationException("The selected device is not in a safe, online ADB state.");
    }

    static void CleanupStaleCaptures()
    {
        var pattern = new Regex("^device-toolkit-capture-[0-9a-f]{32}\\.log$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        try
        {
            foreach (var path in Directory.EnumerateFiles(Path.GetTempPath(), "device-toolkit-capture-*.log", SearchOption.TopDirectoryOnly))
                if (pattern.IsMatch(Path.GetFileName(path)) &&
                    File.GetLastWriteTimeUtc(path) < DateTime.UtcNow.AddMinutes(-5))
                    try { File.Delete(path); } catch { }
        }
        catch { }
    }

    /// <summary>
    /// Narrow adapter for a bounded capture. The transplanted LogcatStream is
    /// unbounded and starts from the device's existing ring buffer. Its fixed
    /// command cannot express `-T 1`, so using it here would report old crashes
    /// as if they occurred during this short diagnostic window.
    /// </summary>
    sealed class BoundedLogcatCapture(string serial, string outputPath)
    {
        const long MaximumCaptureBytes = 64L * 1024 * 1024;

        public async Task<TimeSpan> RunAsync(TimeSpan duration, CancellationToken ct)
        {
            var client = new AdbClient();
            var device = client.GetDevices().SingleOrDefault(candidate => candidate.Serial == serial && candidate.State == DeviceState.Online)
                ?? throw new InvalidOperationException("The selected device is no longer online.");

            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

            using var limit = CancellationTokenSource.CreateLinkedTokenSource(ct);
            limit.CancelAfter(duration);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var lines = client.ExecuteRemoteEnumerableAsync(
                    "logcat -v threadtime -T 1", device, Encoding.UTF8, limit.Token);
                await BoundedLogcatFileWriter.WriteAsync(lines, outputPath, MaximumCaptureBytes, limit.Token);
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested) { }
            finally
            {
                stopwatch.Stop();
            }
            return stopwatch.Elapsed;
        }
    }
}
