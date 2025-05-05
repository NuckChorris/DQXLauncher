using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Windows.Services;
using DQXLauncher.Windows.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.Views.SettingsPages;

public partial class LauncherSettingsPage : Page
{
    private LauncherSettings Settings => Ioc.Default.GetRequiredService<LauncherSettings>();

    public LauncherSettingsPage()
    {
        InitializeComponent();
    }

    private async Task<string?> OpenFolderPickerAsync(string startDir)
    {
        var picker = new FolderPicker { InputPath = startDir };
        picker.ShowDialog();
        return picker.ResultPaths.Count > 0 ? picker.ResultPaths[0] : null;
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var dir = await OpenFolderPickerAsync(Settings.GameFolderPath);
        if (dir != null)
        {
            Settings.GameFolderPath = dir;
            Settings.Save();
        }
    }
}