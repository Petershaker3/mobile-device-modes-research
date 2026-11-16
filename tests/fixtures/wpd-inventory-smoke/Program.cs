using DeviceToolkit.Wpd;
using MediaDevices;

internal static class Program
{
    [STAThread]
    private static async Task<int> Main(string[] args)
    {
        if (!OperatingSystem.IsWindows())
        {
            Console.Error.WriteLine("This smoke runner requires Windows Portable Devices.");
            return 2;
        }

        Console.WriteLine($"COM apartment: {Thread.CurrentThread.GetApartmentState()}");
        try
        {
            if (args.Contains("--direct-count", StringComparer.Ordinal))
                return DirectCount();
            if (args.Contains("--metadata-only", StringComparer.Ordinal))
                return await EnumerateMetadataOnlyAsync();
            if (args.Contains("--worker-timeout-probe", StringComparer.Ordinal))
                return await ProbeWorkerTimeoutAsync();
            if (args.Contains("--worker-transfer-probe", StringComparer.Ordinal))
                return await ProbeWorkerTransfersAsync();
            return await RunProviderSmokeAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"WPD smoke failed: {ex.GetType().Name}");
            return 1;
        }
    }

    private static async Task<int> RunProviderSmokeAsync()
    {
        using var provider = new WpdDeviceFileProvider();
        var devices = await provider.EnumerateDevicesAsync();
        var named = devices.Count(device => !string.IsNullOrWhiteSpace(device.FriendlyName));
        var manufacturers = devices.Count(device => !string.IsNullOrWhiteSpace(device.Manufacturer));
        Console.WriteLine($"WPD devices enumerated: {devices.Count}");
        Console.WriteLine($"Friendly names available: {named}");
        Console.WriteLine($"Manufacturers available: {manufacturers}");
        if (devices.Count == 0) return 1;

        var selected = SelectPhone(devices);
        Console.WriteLine("Opening explicitly selected phone read-only through the worker...");
        using var session = await provider.OpenReadOnlyAsync(selected.SelectionId);
        Console.WriteLine("Read-only session opened.");

        var rootDirectories = await session.ListDirectoriesAsync(session.RootDirectoryId);
        Console.WriteLine($"WPD root directories enumerated: {rootDirectories.Count}");
        var storageRoots = rootDirectories
            .Where(entry => entry.Name is "Internal storage" or "Phone")
            .ToArray();
        if (storageRoots.Length != 1)
            throw new InvalidOperationException("Exactly one known phone storage root is required; refusing to guess.");
        var storage = storageRoots[0];

        var storageDirectories = await session.ListDirectoriesAsync(storage.EntryId);
        Console.WriteLine($"Internal storage subdirectories enumerated: {storageDirectories.Count}");
        var download = storageDirectories.SingleOrDefault(entry => entry.Name == "Download");
        if (download is null) throw new InvalidOperationException("Expected Download folder was not found.");

        var downloadDirectories = await session.ListDirectoriesAsync(download.EntryId);
        Console.WriteLine($"Download subdirectories enumerated: {downloadDirectories.Count}");
        var downloadFiles = await session.ListFilesAsync(download.EntryId);
        Console.WriteLine($"Download files enumerated: {downloadFiles.Count}");
        var testFile = downloadFiles.SingleOrDefault(entry => entry.Name == "DeviceToolkitInstallTest-wpd-provider-smoke.apk");
        if (testFile is null)
        {
            Console.WriteLine("Provider download verification skipped: the exact temporary file is absent.");
            return 0;
        }

        var readback = Path.Combine(Path.GetTempPath(), $"wpd-provider-readback-{Guid.NewGuid():N}.apk");
        try
        {
            var result = await session.DownloadFileAsync(testFile.EntryId, readback);
            var expectedHash = "8616B71B2DD880D3E0E64F5BF403942A4F581B687A35E50E9FA31457BFE17416";
            var actualHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(readback)));
            Console.WriteLine($"Provider download bytes: {result.SizeBytes}");
            Console.WriteLine($"Provider download SHA256 match: {actualHash == expectedHash}");
            return result.SizeBytes == 8602 && actualHash == expectedHash ? 0 : 1;
        }
        finally
        {
            try { File.Delete(readback); } catch { }
        }
    }

    private static async Task<int> EnumerateMetadataOnlyAsync()
    {
        using var provider = new WpdDeviceFileProvider();
        var devices = await provider.EnumerateDevicesAsync();
        Console.WriteLine($"WPD devices enumerated: {devices.Count}");
        Console.WriteLine($"Friendly names reported: {devices.Count(device => !string.IsNullOrWhiteSpace(device.FriendlyName))}");
        Console.WriteLine($"Manufacturers reported: {devices.Count(device => !string.IsNullOrWhiteSpace(device.Manufacturer))}");
        Console.WriteLine($"Descriptions reported: {devices.Count(device => !string.IsNullOrWhiteSpace(device.Description))}");
        if (devices.Count != 1)
        {
            Console.WriteLine("Detailed metadata probe skipped: select exactly one connected WPD phone.");
            return devices.Count == 0 ? 1 : 0;
        }

        using var session = await provider.OpenReadOnlyAsync(devices[0].SelectionId);
        var details = await session.ReadDeviceDetailsAsync();
        Console.WriteLine($"Model reported: {Reported(details.Model)}");
        Console.WriteLine($"Name reported: {Reported(details.Name)}");
        Console.WriteLine($"Firmware reported: {Reported(details.FirmwareVersion)}");
        Console.WriteLine($"Battery reported: {Reported(details.PowerLevel)}");
        Console.WriteLine($"Power source reported: {Reported(details.PowerSource)}");
        Console.WriteLine($"Protocol reported: {Reported(details.Protocol)}");
        Console.WriteLine($"Device time reported: {Reported(details.DeviceDateTime)}");
        Console.WriteLine($"Storage volumes reported: {details.Storage.Count}");
        foreach (var storage in details.Storage)
            Console.WriteLine($"{storage.Name}: total={storage.TotalBytes?.ToString() ?? "not reported"}; free={storage.FreeBytes?.ToString() ?? "not reported"}; filesystem={Reported(storage.FileSystem)}");
        Console.WriteLine("No serial, PnP ID, volume labels, storage paths, or file names were printed or queried.");
        return 0;
    }

    private static string Reported(string value) => value == "Not reported" ? "not reported" : value;

    private static async Task<int> ProbeWorkerTimeoutAsync()
    {
        using var provider = new WpdDeviceFileProvider(
            operationTimeout: TimeSpan.FromSeconds(8),
            connectionTimeout: TimeSpan.FromMinutes(2));
        Console.WriteLine("Worker timeout probe: enumerating devices...");
        Console.Out.Flush();
        var devices = await provider.EnumerateDevicesAsync();
        var selected = SelectPhone(devices);
        Console.WriteLine("Worker timeout probe: opening selected device...");
        Console.Out.Flush();
        using var session = await provider.OpenReadOnlyAsync(selected.SelectionId);
        Console.WriteLine("Worker timeout probe: listing root directories...");
        Console.Out.Flush();
        var rootDirectories = await session.ListDirectoriesAsync(session.RootDirectoryId);
        var storageRoots = rootDirectories.Where(entry => entry.Name is "Internal storage" or "Phone").ToArray();
        if (storageRoots.Length != 1)
            throw new InvalidOperationException("Exactly one known phone storage root is required; refusing to guess.");
        var storage = storageRoots[0];
        var storageDirectories = await session.ListDirectoriesAsync(storage.EntryId);
        var candidateDirectories = storageDirectories
            .Where(entry => entry.Name is "DCIM" or "Pictures" or "Download" or "Documents" or "Movies" or "Music" or "Camera")
            .OrderBy(entry => entry.Name switch
            {
                "DCIM" => 0,
                "Pictures" => 1,
                "Download" => 2,
                "Camera" => 3,
                "Documents" => 4,
                "Movies" => 5,
                _ => 6
            })
            .ToArray();
        if (candidateDirectories.Length == 0)
            throw new InvalidOperationException("No known media or download directory is available for the timeout probe.");

        var nonemptyDirectories = 0;
        foreach (var probeDirectory in candidateDirectories)
        {
            Console.WriteLine($"Worker timeout probe: listing files in {probeDirectory.Name} with an 8-second deadline...");
            Console.Out.Flush();
            try
            {
                var files = await session.ListFilesAsync(probeDirectory.EntryId);
                Console.WriteLine($"Selected directory file count: {files.Count}");
                if (files.Count > 0) nonemptyDirectories++;
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Selected directory file listing timed out; worker process was terminated.");
                try
                {
                    _ = await session.ListDirectoriesAsync(session.RootDirectoryId);
                    throw new InvalidOperationException("The WPD session remained usable after its worker timed out.");
                }
                catch (InvalidOperationException ex) when (ex.Message.StartsWith("The WPD session was stopped;", StringComparison.Ordinal))
                {
                    Console.WriteLine("Timed-out session was marked stopped and rejected another operation.");
                }
                return 0;
            }
        }
        Console.WriteLine(nonemptyDirectories == 0
            ? "All existing standard probe folders were empty; no slow WPD enumeration occurred."
            : "All populated standard probe folders completed within the 8-second operation timeout.");
        return 0;
    }

    private static async Task<int> ProbeWorkerTransfersAsync()
    {
        const ulong maxProbeSize = 256UL * 1024 * 1024;
        const ulong cancellationProbeSize = 32UL * 1024 * 1024;
        using var provider = new WpdDeviceFileProvider(
            operationTimeout: TimeSpan.FromMinutes(2),
            connectionTimeout: TimeSpan.FromMinutes(2));
        var devices = await provider.EnumerateDevicesAsync();
        var selected = SelectPhone(devices);
        using var session = await provider.OpenReadOnlyAsync(selected.SelectionId);
        var storage = (await session.ListDirectoriesAsync(session.RootDirectoryId))
            .Single(entry => entry.Name is "Internal storage" or "Phone");
        var download = (await session.ListDirectoriesAsync(storage.EntryId))
            .Single(entry => entry.Name == "Download");
        var files = await session.ListFilesAsync(download.EntryId);
        var unicodeFiles = files
            .Where(entry => entry.Name.Any(character => character > 127) && entry.SizeBytes is <= maxProbeSize)
            .ToArray();
        var sizedFiles = files
            .Where(entry => entry.SizeBytes is not null and <= maxProbeSize)
            .OrderByDescending(entry => entry.SizeBytes)
            .ToArray();

        Console.WriteLine($"Download entries: {files.Count}; Unicode-name candidates: {unicodeFiles.Length}");
        Console.WriteLine($"Largest known file: {(sizedFiles.FirstOrDefault()?.SizeBytes?.ToString() ?? "unknown")} bytes");
        Console.WriteLine("Candidate transfers are limited to files no larger than 256 MiB; filenames and content are not printed.");

        var probeDirectory = Path.Combine(Path.GetTempPath(), $"DeviceToolkit-WpdTransferProbe-{Guid.NewGuid():N}");
        Directory.CreateDirectory(probeDirectory);
        try
        {
            var candidate = unicodeFiles.FirstOrDefault() ?? sizedFiles.FirstOrDefault();
            if (candidate is null)
            {
                Console.WriteLine("No known-size file within the safe probe limit was available for download.");
                return 0;
            }

            var localFile = Path.Combine(probeDirectory, "readback.bin");
            var result = await session.DownloadFileAsync(candidate.EntryId, localFile);
            var localSize = checked((ulong)new FileInfo(localFile).Length);
            if (candidate.SizeBytes is { } expected && expected != localSize || result.SizeBytes != localSize)
                throw new IOException("The WPD readback size did not match the listed file size.");
            await using (var readback = File.OpenRead(localFile))
                _ = await System.Security.Cryptography.SHA256.HashDataAsync(readback);
            Console.WriteLine($"Read-only download completed and size-verified: {localSize} bytes; Unicode path: {unicodeFiles.Any(entry => entry.EntryId == candidate.EntryId)}");
            File.Delete(localFile);

            var cancellationCandidate = sizedFiles.FirstOrDefault(entry => entry.SizeBytes >= cancellationProbeSize);
            if (cancellationCandidate is null)
            {
                Console.WriteLine("No file of at least 32 MiB was available for an interruption probe.");
                return 0;
            }

            using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));
            var cancellationDestination = Path.Combine(probeDirectory, "cancelled.bin");
            try
            {
                _ = await session.DownloadFileAsync(cancellationCandidate.EntryId, cancellationDestination, cancellation.Token);
                Console.WriteLine("Cancellation probe file completed before cancellation; no interrupted transfer was observed.");
                File.Delete(cancellationDestination);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancellation stopped the WPD request; verifying the session rejects further operations.");
                try
                {
                    _ = await session.ListFilesAsync(download.EntryId);
                    throw new InvalidOperationException("The session remained usable after cancellation.");
                }
                catch (InvalidOperationException ex) when (ex.Message.StartsWith("The WPD session was stopped;", StringComparison.Ordinal))
                {
                    Console.WriteLine("Cancelled session rejected further operations.");
                }
            }

            Console.WriteLine($"Local probe artifacts remaining: {Directory.EnumerateFiles(probeDirectory).Count()}");
            return Directory.EnumerateFiles(probeDirectory).Any() ? 1 : 0;
        }
        finally
        {
            try { Directory.Delete(probeDirectory, recursive: true); } catch { }
        }
    }

    private static int DirectCount()
    {
        var devices = MediaDeviceManager.Instance.GetDevices()?.ToArray() ?? [];
        Console.WriteLine($"Direct WPD devices enumerated: {devices.Length}");
        Console.WriteLine($"Direct PnP IDs available: {devices.Count(device => !string.IsNullOrWhiteSpace(device.PnPDeviceID))}");
        foreach (var device in devices) device.Dispose();
        return devices.Length > 0 ? 0 : 1;
    }

    private static WpdDeviceInfo SelectPhone(IReadOnlyList<WpdDeviceInfo> devices)
    {
        if (devices.Count != 1)
            throw new InvalidOperationException("The WPD smoke requires exactly one connected device and refuses ambiguous targets.");
        return devices[0];
    }
}
