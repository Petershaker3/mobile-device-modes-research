using DeviceToolkit.Desktop.Services;
using DeviceToolkit.Core.Operations;
using logcat.Services;

var serial = Environment.GetEnvironmentVariable("DEVICE_TOOLKIT_TEST_SERIAL");
var apkPath = Environment.GetEnvironmentVariable("DEVICE_TOOLKIT_TEST_APK");
if (string.IsNullOrWhiteSpace(serial) || string.IsNullOrWhiteSpace(apkPath))
    throw new InvalidOperationException("Use run-smoke-on-single-device.ps1 to select the target and APK.");

using var approvals = new ApprovalService();
using var workflow = new DeviceWorkflow(approvals);
var target = workflow.ListDevices().SingleOrDefault(device =>
    string.Equals(device.Serial, serial, StringComparison.Ordinal) && device.State == "Online")
    ?? throw new InvalidOperationException("The explicitly selected device is no longer online.");
var installPreview = await workflow.InspectApkAsync(target, apkPath, CancellationToken.None);
if (installPreview.AlreadyInstalled)
    throw new InvalidOperationException("The test package was already installed before this run; no device changes were made.");

Console.WriteLine("INSTALL PREVIEW");
Console.WriteLine($"Target: the explicitly selected online phone ({target.Model})");
Console.WriteLine($"Package: {installPreview.PackageId}");
Console.WriteLine($"Version: {installPreview.VersionName} ({installPreview.VersionCode})");
Console.WriteLine($"APK: {installPreview.FileName}, {installPreview.SizeBytes} bytes");
Console.WriteLine($"SHA-256: {installPreview.Sha256}");

var installAttempted = false;
try
{
    installAttempted = true;
    var installPlan = workflow.CreateInstallPlan(target, installPreview, apkPath);
    var installApproval = approvals.ApproveAfterUserConfirmation(installPlan);
    await workflow.RecordApprovalGrantedAsync(installPlan);
    await workflow.InstallApkAsync(target, installPreview, apkPath, installPlan, installApproval, CancellationToken.None);
    var installed = (await workflow.ReadInstalledAppsAsync(target, CancellationToken.None))
        .SingleOrDefault(app => app.PackageId == installPreview.PackageId);
    if (installed is null || !installed.IsThirdParty)
        throw new InvalidOperationException("The package was not present in the user-app inventory after install.");
    var installedVersion = await workflow.ReadAppVersionAsync(target, installed, CancellationToken.None);
    if (installedVersion.VersionName != installPreview.VersionName || installedVersion.VersionCode != installPreview.VersionCode)
        throw new InvalidOperationException("The installed version differs from the preview.");
    Console.WriteLine("Install verified: package and version match the preview; Android lists it as a user app.");

    Console.WriteLine("REMOVAL PREVIEW");
    Console.WriteLine($"Target: the explicitly selected online phone ({target.Model})");
    Console.WriteLine($"Package: {installPreview.PackageId}");
    Console.WriteLine("Kind: user app installed by this smoke test");
    var removalPlan = workflow.CreateRemovalPlan(target,
        new AppRemovalPreview(installPreview.PackageId, target.Model, target.Serial));
    var removalApproval = approvals.ApproveAfterUserConfirmation(removalPlan);
    await workflow.RecordApprovalGrantedAsync(removalPlan);
    await workflow.RemoveThirdPartyAppAsync(target, installPreview.PackageId, removalPlan, removalApproval, CancellationToken.None);
    var remaining = (await workflow.ReadInstalledAppsAsync(target, CancellationToken.None))
        .Any(app => app.PackageId == installPreview.PackageId);
    if (remaining) throw new InvalidOperationException("The package is still installed after removal.");
    Console.WriteLine("Removal verified: the test package is absent from the phone.");
}
catch
{
    if (installAttempted)
    {
        try
        {
            var installed = (await workflow.ReadInstalledAppsAsync(target, CancellationToken.None))
                .SingleOrDefault(app => app.PackageId == installPreview.PackageId && app.IsThirdParty);
            if (installed is not null)
            {
                var installedVersion = await workflow.ReadAppVersionAsync(target, installed, CancellationToken.None);
                if (installedVersion.VersionName == installPreview.VersionName && installedVersion.VersionCode == installPreview.VersionCode)
                {
                    var cleanupPlan = workflow.CreateRemovalPlan(target,
                        new AppRemovalPreview(installPreview.PackageId, target.Model, target.Serial));
                    var cleanupApproval = approvals.ApproveAfterUserConfirmation(cleanupPlan);
                    await workflow.RecordApprovalGrantedAsync(cleanupPlan);
                    await workflow.RemoveThirdPartyAppAsync(target, installPreview.PackageId, cleanupPlan, cleanupApproval, CancellationToken.None);
                    Console.WriteLine("Cleanup: removed the matching test package after an incomplete smoke test.");
                }
                else
                {
                    Console.WriteLine("Cleanup skipped: the installed package version did not match this test APK.");
                }
            }
        }
        catch (Exception cleanupError)
        {
            Console.WriteLine($"Cleanup needs attention for test package {installPreview.PackageId}: {cleanupError.Message}");
        }
    }
    throw;
}
