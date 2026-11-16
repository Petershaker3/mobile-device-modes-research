using System.Text.RegularExpressions;

namespace DeviceToolkit.Core.Services;

public static partial class SamsungFirmwareLinks
{
    [GeneratedRegex("^SM-[A-Z0-9]{3,18}$", RegexOptions.CultureInvariant)]
    private static partial Regex ModelPattern();

    [GeneratedRegex("^[A-Z0-9]{3}$", RegexOptions.CultureInvariant)]
    private static partial Regex CscPattern();

    public static Uri? ForReportedDevice(string manufacturer, string model, string salesCode)
    {
        if (!manufacturer.Contains("samsung", StringComparison.OrdinalIgnoreCase)) return null;
        var normalizedModel = model.Trim().ToUpperInvariant();
        if (!ModelPattern().IsMatch(normalizedModel)) return null;

        var normalizedSalesCode = salesCode.Trim().ToUpperInvariant();
        var path = CscPattern().IsMatch(normalizedSalesCode)
            ? $"firmware/{normalizedModel}/{normalizedSalesCode}"
            : $"firmware/{normalizedModel}";
        return new Uri($"https://samfw.com/{path}", UriKind.Absolute);
    }
}
