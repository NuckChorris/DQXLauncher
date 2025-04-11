using Windows.Storage.Pickers;
using System;
using System.Threading.Tasks;
using DQXLauncher.Toolbox.Services;

namespace DQXLauncher.Toolbox.Views;

public sealed partial class SettingsPage
{
    public Settings SettingsModel { get; }

    public SettingsPage()
    {
        this.InitializeComponent();
        SettingsModel = Settings.Instance;
    }

    private async void OnGameFolderBrowseClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var folderPath = await OpenFolderPickerAsync();
        if (!string.IsNullOrEmpty(folderPath))
        {
            SettingsModel.GameFolderPath = folderPath;
            SettingsModel.Save();
            Bindings.Update();
        }
    }

    private async void OnSaveFolderBrowseClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var folderPath = await OpenFolderPickerAsync();
        if (!string.IsNullOrEmpty(folderPath))
        {
            SettingsModel.SaveFolderPath = folderPath;
            SettingsModel.Save();
            Bindings.Update();
        }
    }

    private async Task<string> OpenFolderPickerAsync()
    {
        var picker = new FolderPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            FileTypeFilter = { "*" }
        };

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle((App.Current as App).AppWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var folder = await picker.PickSingleFolderAsync();
        return folder?.Path;
    }
}
