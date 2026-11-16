using System.Diagnostics;
using System.Text.RegularExpressions;
using DeviceToolkit.Core.Windows;

var inventory = new PnpDeviceInventory().EnumeratePresentDevices();
var problems = inventory.Where(device => device.ProblemCode is > 0).ToArray();
var unavailableCount = inventory.Count(device => device.ProblemCode is null);
var hasProblemFlagCount = inventory.Count(device => device.StatusFlags is { } flags && (flags & 0x00000400) != 0);
var connectedIds = await ReadPnputilInstanceIdsAsync("/enum-devices", "/connected", "/deviceids");
var problemIds = await ReadPnputilInstanceIdsAsync("/enum-devices", "/problem", "/deviceids");
var apiIds = inventory.Select(device => device.InstanceId).ToHashSet(StringComparer.OrdinalIgnoreCase);

Console.WriteLine($"SetupAPI present devices: {inventory.Count}");
Console.WriteLine($"SetupAPI nonzero problem codes: {problems.Length}");
Console.WriteLine($"SetupAPI unavailable problem statuses: {unavailableCount}");
Console.WriteLine($"SetupAPI DN_HAS_PROBLEM flags: {hasProblemFlagCount}");
Console.WriteLine($"PnPUtil connected instance IDs: {connectedIds.Count}");
Console.WriteLine($"SetupAPI IDs absent from PnPUtil connected list: {apiIds.Except(connectedIds, StringComparer.OrdinalIgnoreCase).Count()}");
Console.WriteLine($"PnPUtil connected IDs absent from SetupAPI list: {connectedIds.Except(apiIds, StringComparer.OrdinalIgnoreCase).Count()}");
Console.WriteLine($"PnPUtil problem instance IDs: {problemIds.Count}");
foreach (var group in problems.GroupBy(device => device.ProblemCode))
    Console.WriteLine($"API problem code {group.Key}: {group.Count()} device(s)");

static async Task<HashSet<string>> ReadPnputilInstanceIdsAsync(params string[] arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "pnputil.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    foreach (string argument in arguments)
        process.StartInfo.ArgumentList.Add(argument);
    process.Start();
    string output = await process.StandardOutput.ReadToEndAsync();
    await process.WaitForExitAsync();
    if (process.ExitCode != 0)
        throw new InvalidOperationException($"PnPUtil exited with code {process.ExitCode}.");
    return Regex.Matches(output, @"(?im)^\s*Instance ID:\s*(.+?)\s*$")
        .Select(match => match.Groups[1].Value.Trim())
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
}
