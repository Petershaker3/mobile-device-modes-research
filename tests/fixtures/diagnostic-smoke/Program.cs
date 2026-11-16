using DeviceToolkit.Desktop.Services;

var serial = Environment.GetEnvironmentVariable("DEVICE_TOOLKIT_TEST_SERIAL");
if (string.IsNullOrWhiteSpace(serial))
    throw new InvalidOperationException("Use run-smoke-on-selected-device.ps1 to choose the target serial explicitly.");

using var workflow = new DeviceWorkflow();
var matches = (await workflow.ListDevicesAsync())
    .Where(device => string.Equals(device.Serial, serial, StringComparison.Ordinal) && device.State == "Online")
    .ToArray();
if (matches.Length != 1)
    throw new InvalidOperationException("The explicitly selected device is not uniquely online. No diagnostic was run.");

var target = matches[0];
Console.WriteLine($"Target selected: {target.Model} ({target.Serial})");

var facts = await workflow.ReadFactsAsync(target, CancellationToken.None);
Console.WriteLine("Device facts read through DeviceWorkflow:");
Console.WriteLine($"Model: {facts.Model}");
Console.WriteLine($"Manufacturer: {facts.Manufacturer}");
Console.WriteLine($"Android: {facts.Android}");
Console.WriteLine($"API: {facts.Api}");
Console.WriteLine($"ABI: {facts.Abi}");
Console.WriteLine($"Private DNS mode: {facts.PrivateDns.Mode}");
Console.WriteLine($"Private DNS provider configured: {facts.PrivateDns.ProviderHostname is not null}");

Console.WriteLine("Capturing and analyzing the selected device's recent logcat for 30 seconds. Log contents will not be printed or retained as a report.");
var result = await workflow.CaptureAndAnalyzeAsync(target, TimeSpan.FromSeconds(30), CancellationToken.None);
Console.WriteLine("Diagnostic summary:");
Console.WriteLine($"Lines: {result.Lines}");
Console.WriteLine($"Crash events: {result.CrashEvents}");
Console.WriteLine($"Crash groups: {result.CrashGroups}");
Console.WriteLine($"Elapsed: {(result.Finished - result.Started).TotalSeconds:F1} seconds");
