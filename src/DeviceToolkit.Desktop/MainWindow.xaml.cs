using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using logcat.Services;

namespace DeviceToolkit.Desktop;

public partial class MainWindow : Window
{
    readonly MainViewModel _viewModel = new();

    void OpenSamFwFirmware_Click(object sender, RequestNavigateEventArgs e)
    {
        if (e.Uri is not { Scheme: "https", Host: "samfw.com" } uri) return;
        Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        SelectWorkArea(0);
        Loaded += (_, _) => _viewModel.RefreshCommand.Execute(null);
        Closed += (_, _) => _viewModel.Dispose();
    }

    void SelectWorkArea_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string indexText } && int.TryParse(indexText, out var index))
            SelectWorkArea(index);
    }

    void SelectWorkArea(int index)
    {
        WorkAreaTabs.SelectedIndex = index;
        foreach (var button in WorkAreaNavigation.Children.OfType<Button>())
        {
            var selected = int.TryParse(button.Tag?.ToString(), out var buttonIndex) && buttonIndex == index;
            button.Background = (System.Windows.Media.Brush)FindResource(selected ? "AccentSoftBrush" : "SurfaceBrush");
            button.BorderBrush = (System.Windows.Media.Brush)FindResource(selected ? "AccentBrush" : "LineBrush");
            button.Foreground = (System.Windows.Media.Brush)FindResource("InkBrush");
        }
    }

    async void ChooseApk_Click(object sender, RoutedEventArgs e)
    {
        var picker = new OpenFileDialog
        {
            Title = "Choose an Android APK",
            Filter = "Android package (*.apk)|*.apk",
            CheckFileExists = true,
            Multiselect = false
        };
        if (picker.ShowDialog(this) == true)
            await _viewModel.PrepareInstallAsync(picker.FileName);
    }

    async void ConnectWireless_Click(object sender, RoutedEventArgs e)
    {
        var pairingCode = WirelessPairingCode.Password;
        WirelessPairingCode.Clear();
        await _viewModel.ConnectWirelessAsync(pairingCode);
    }

    async void FileEntryList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListView { SelectedItem: AndroidFileEntry entry } && entry.CanNavigate)
            await _viewModel.OpenDirectoryAsync(entry);
    }

    async void WpdEntryList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListView { SelectedItem: WpdBrowserEntry entry } && entry.IsDirectory)
            await _viewModel.WpdBrowser.OpenDirectoryAsync(entry);
    }

    void DownloadSelectedFile_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedFileEntry is not { } entry) return;
        var picker = new SaveFileDialog
        {
            Title = "Choose a local destination for the selected Android file",
            FileName = entry.Name,
            OverwritePrompt = false,
            AddExtension = false,
            ValidateNames = true
        };
        if (picker.ShowDialog(this) == true)
            _viewModel.PrepareDownload(picker.FileName);
    }

    void DownloadSelectedWpdFile_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.WpdBrowser.SelectedEntry is not { IsDirectory: false } entry) return;
        var picker = new SaveFileDialog
        {
            Title = "Choose a local destination for the selected MTP file",
            FileName = entry.Name,
            OverwritePrompt = false,
            AddExtension = false,
            ValidateNames = true
        };
        if (picker.ShowDialog(this) == true)
            _viewModel.WpdBrowser.PrepareDownload(picker.FileName);
    }

    void UploadFile_Click(object sender, RoutedEventArgs e)
    {
        var picker = new OpenFileDialog
        {
            Title = "Choose one local file to upload to the current Android folder",
            CheckFileExists = true,
            Multiselect = false
        };
        if (picker.ShowDialog(this) == true)
            _viewModel.PrepareUpload(picker.FileName);
    }
}
