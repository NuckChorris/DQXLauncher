using DQXLauncher.Windows.Views.SettingsPages;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.Views.Pages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void Navigation_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        SettingsScrollViewer.ChangeView(null, 0, null, true);
        var selectedItem = args.SelectedItem as NavigationViewItem;

        switch (selectedItem?.Tag.ToString())
        {
            case "Launcher": SettingsFrame.Navigate(typeof(LauncherSettingsPage)); break;
            default: SettingsFrame.Navigate(typeof(LauncherSettingsPage)); break;
        }
    }
}