using System.Text.Json;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace DeviceToolkit.Desktop;

public partial class App : Application
{
    void OnStartup(object sender, StartupEventArgs e)
    {
        LoadTokens();
        var window = new MainWindow();
        MainWindow = window;
        window.Show();
    }

    void LoadTokens()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "tokens", "desktop.json");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var colors = document.RootElement.GetProperty("colors");
        foreach (var color in colors.EnumerateObject())
            Resources[$"{ToPascal(color.Name)}Brush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color.Value.GetString()!)!);

        var sizes = document.RootElement.GetProperty("sizes");
        foreach (var size in sizes.EnumerateObject())
        {
            var value = size.Value.GetDouble();
            Resources[$"{ToPascal(size.Name)}Size"] = value;
            Resources[ToPascal(size.Name)] = size.Name is "columnGutter" or "labelColumn" or "factRowHeight" or "navigationRailWidth" or "utilityRailWidth"
                ? new GridLength(value)
                : value;
        }

        foreach (var inset in document.RootElement.GetProperty("insets").EnumerateObject())
        {
            var values = inset.Value.EnumerateArray().Select(value => value.GetDouble()).ToArray();
            Resources[ToPascal(inset.Name)] = new Thickness(values[0], values[1], values[2], values[3]);
        }

        foreach (var corner in document.RootElement.GetProperty("corners").EnumerateObject())
            Resources[$"{ToPascal(corner.Name)}Corner"] = new CornerRadius(corner.Value.GetDouble());

        foreach (var opacity in document.RootElement.GetProperty("opacity").EnumerateObject())
            Resources[$"{ToPascal(opacity.Name)}Opacity"] = opacity.Value.GetDouble();

        Resources["PagePadding"] = new Thickness(sizes.GetProperty("pagePadding").GetDouble());
        Resources["SectionGap"] = new Thickness(0, sizes.GetProperty("sectionGap").GetDouble(), 0, 0);
        Resources["ControlGap"] = new Thickness(0, sizes.GetProperty("controlGap").GetDouble(), 0, 0);
        Resources["BodySize"] = sizes.GetProperty("body").GetDouble();
        Resources["SmallSize"] = sizes.GetProperty("small").GetDouble();
        Resources["DisplaySize"] = sizes.GetProperty("display").GetDouble();
        Resources["SectionSize"] = sizes.GetProperty("section").GetDouble();
        Resources["ControlHeight"] = sizes.GetProperty("controlHeight").GetDouble();
    }

    static string ToPascal(string name)
        => string.Concat(name.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(part => char.ToUpperInvariant(part[0]) + part[1..]));
}
