using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using DeviceToolkit.Core.Operations;
using DeviceToolkit.Desktop.Services;
using logcat.Services;

var serial = Environment.GetEnvironmentVariable("DEVICE_TOOLKIT_TEST_SERIAL");
var sourcePath = Environment.GetEnvironmentVariable("DEVICE_TOOLKIT_TEST_FILE");
if (string.IsNullOrWhiteSpace(serial) || string.IsNullOrWhiteSpace(sourcePath))
    throw new InvalidOperationException("Use run-smoke-on-selected-device.ps1 to choose the device and fixture explicitly.");

const string remotePath = "/sdcard/Download/DeviceToolkitInstallTest-smoke.apk";
const string remoteDirectory = "/sdcard/Download";
var source = new FileInfo(sourcePath);
if (!source.Exists) throw new FileNotFoundException("The transfer fixture is missing.", sourcePath);
var sourceHash = await HashAsync(source.FullName);
var downloadPath = Path.Combine(Path.GetTempPath(), $"device-toolkit-transfer-{Guid.NewGuid():N}.apk");

using var approvals = new ApprovalService();
using var workflow = new DeviceWorkflow(approvals);
var targetMatches = workflow.ListDevices()
    .Where(device => string.Equals(device.Serial, serial, StringComparison.Ordinal) && device.State == "Online")
    .ToArray();
if (targetMatches.Length != 1)
    throw new InvalidOperationException("The explicitly selected device is not uniquely online. No transfer was attempted.");
var target = targetMatches[0];

var initialEntry = await FindAsync(workflow, target, remoteDirectory, Path.GetFileName(remotePath));
if (initialEntry is not null)
    throw new InvalidOperationException("The exact smoke-test destination already exists. No file was changed.");

var cleanupRequired = false;
Exception? failure = null;
try
{
    cleanupRequired = true;
    var uploadPreview = new FileTransferPreview(FileTransferDirection.Upload, target.Model, target.Serial,
        source.FullName, remotePath, source.FullName, remotePath, checked((ulong)source.Length), false);
    var uploadPlan = workflow.CreateFileTransferPlan(target, uploadPreview);
    var uploadApproval = approvals.ApproveAfterUserConfirmation(uploadPlan);
    await workflow.RecordApprovalGrantedAsync(uploadPlan);
    var upload = await workflow.UploadSharedFileAsync(target, uploadPreview, null, uploadPlan, uploadApproval, CancellationToken.None);
    if (!string.Equals(upload.DestinationPath, remotePath, StringComparison.Ordinal) || upload.SizeBytes != (ulong)source.Length)
        throw new IOException("The upload result did not match the selected path and source size.");

    var remoteEntry = await FindAsync(workflow, target, remoteDirectory, Path.GetFileName(remotePath));
    if (remoteEntry is not { Kind: AndroidFileKind.File, Error: AdvancedSharpAdbClient.Models.UnixErrorCode.Default } ||
        remoteEntry.SizeBytes != (ulong)source.Length)
        throw new IOException("Android did not report the expected regular file and size after upload.");

    var downloadPreview = new FileTransferPreview(FileTransferDirection.Download, target.Model, target.Serial,
        remotePath, downloadPath, downloadPath, remotePath, checked((ulong)source.Length), false);
    var downloadPlan = workflow.CreateFileTransferPlan(target, downloadPreview);
    var downloadApproval = approvals.ApproveAfterUserConfirmation(downloadPlan);
    await workflow.RecordApprovalGrantedAsync(downloadPlan);
    var download = await workflow.DownloadSharedFileAsync(target, remoteEntry, downloadPreview, null, downloadPlan, downloadApproval, CancellationToken.None);
    var downloadedHash = await HashAsync(downloadPath);
    if (download.SizeBytes != (ulong)source.Length || !string.Equals(downloadedHash, sourceHash, StringComparison.Ordinal))
        throw new IOException("The downloaded copy did not match the fixture size and SHA-256.");

    Console.WriteLine($"Target: {target.Model} ({target.Serial})");
    Console.WriteLine($"Remote path: {remotePath}");
    Console.WriteLine($"Transferred bytes: {download.SizeBytes}");
    Console.WriteLine($"Source SHA-256: {sourceHash}");
    Console.WriteLine($"Downloaded SHA-256: {downloadedHash}");
    Console.WriteLine("Integrity check: PASS");
}
catch (Exception ex)
{
    failure = ex;
}
finally
{
    try
    {
        if (cleanupRequired)
        {
            var shell = new AdbShell();
            var command = $"rm -f {AndroidFileTransferPaths.QuotePosixShellArgument(remotePath)}";
            await shell.RunAsync(serial, command, TimeSpan.FromSeconds(10), CancellationToken.None);
            if (await FindAsync(workflow, target, remoteDirectory, Path.GetFileName(remotePath)) is not null)
                throw new IOException($"The smoke-test copy remains on the phone at {remotePath}.");
            Console.WriteLine($"Cleanup: verified removal of {remotePath}.");
        }
    }
    catch (Exception cleanupError)
    {
        throw new IOException($"Transfer cleanup could not be confirmed for the exact smoke-test path {remotePath}.",
            failure is null ? cleanupError : new AggregateException(failure, cleanupError));
    }
    finally
    {
        try { File.Delete(downloadPath); } catch { }
    }
}

if (failure is not null) throw new InvalidOperationException("The file-transfer smoke test failed.", failure);

static async Task<AndroidFileEntry?> FindAsync(DeviceWorkflow workflow, ConnectedDevice device, string directory, string name)
{
    await foreach (var entry in workflow.ListSharedStorageAsync(device, directory, CancellationToken.None))
        if (string.Equals(entry.Name, name, StringComparison.Ordinal)) return entry;
    return null;
}

static async Task<string> HashAsync(string path)
{
    await using var stream = File.OpenRead(path);
    return Convert.ToHexString(await SHA256.HashDataAsync(stream));
}
