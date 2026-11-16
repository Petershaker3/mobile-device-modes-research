using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace logcat.Services;

public enum WirelessAdbServiceKind { Pairing, Connect }
public sealed record WirelessAdbService(WirelessAdbServiceKind Kind, IPEndPoint Endpoint);

public static partial class WirelessAdbAddress
{
    static readonly Regex Ipv4Pattern = Ipv4Regex();
    static readonly Regex PairingCodePattern = PairingCodeRegex();

    public static IPEndPoint ParseIpv4Endpoint(string address, string portText)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(portText);
        var normalizedAddress = address.Trim();
        var normalizedPort = portText.Trim();
        if (!Ipv4Pattern.IsMatch(normalizedAddress) ||
            !IPAddress.TryParse(normalizedAddress, out var ipAddress) ||
            ipAddress.AddressFamily != AddressFamily.InterNetwork)
            throw new ArgumentException("Enter the phone's IPv4 address shown in Wireless debugging.", nameof(address));
        if (!int.TryParse(normalizedPort, NumberStyles.None, CultureInfo.InvariantCulture, out var port) || port is < 1 or > 65535)
            throw new ArgumentOutOfRangeException(nameof(portText), "Enter a port from 1 through 65535.");
        return new IPEndPoint(ipAddress, port);
    }

    public static void ValidatePairingCode(string code)
    {
        ArgumentNullException.ThrowIfNull(code);
        if (!PairingCodePattern.IsMatch(code))
            throw new ArgumentException("Enter the six-digit pairing code currently shown on the phone.", nameof(code));
    }

    public static bool IsPairingSuccess(string? response)
        => response?.TrimStart().StartsWith("Successfully paired", StringComparison.OrdinalIgnoreCase) == true;

    public static bool IsConnectSuccess(string? response)
        => response is not null &&
           (response.TrimStart().StartsWith("connected to ", StringComparison.OrdinalIgnoreCase) ||
            response.TrimStart().StartsWith("already connected to ", StringComparison.OrdinalIgnoreCase));

    public static IReadOnlyList<WirelessAdbService> ParseMdnsServicesOutput(string output)
    {
        ArgumentNullException.ThrowIfNull(output);
        var services = new List<WirelessAdbService>();
        foreach (var line in output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var columns = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (columns.Length < 3) continue;

            var kind = columns[^2] switch
            {
                "_adb-tls-pairing._tcp" => WirelessAdbServiceKind.Pairing,
                "_adb-tls-connect._tcp" => WirelessAdbServiceKind.Connect,
                _ => (WirelessAdbServiceKind?)null
            };
            if (kind is null) continue;

            var endpoint = columns[^1].Split(':');
            if (endpoint.Length != 2) continue;
            try
            {
                services.Add(new WirelessAdbService(kind.Value, ParseIpv4Endpoint(endpoint[0], endpoint[1])));
            }
            catch (ArgumentException) { }
        }

        return services
            .DistinctBy(service => (service.Kind, service.Endpoint.ToString()))
            .ToArray();
    }

    [GeneratedRegex("^(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])$", RegexOptions.CultureInvariant)]
    private static partial Regex Ipv4Regex();

    [GeneratedRegex("^[0-9]{6}$", RegexOptions.CultureInvariant)]
    private static partial Regex PairingCodeRegex();
}
