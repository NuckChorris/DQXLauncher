using System;
using System.IO;
using DQXLauncher.Core.Game.ConfigFile;
using DQXLauncher.Core.Services;
using DQXLauncher.Windows.Services;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DQXLauncher.Windows;

/// <summary>
///     Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static Window? AppWindow;

    /// <summary>
    ///     Initializes the singleton application object.  This is the first line of authored code
    ///     executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        Paths.AppData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            , "DQXLauncher");
        Paths.Create();
        ConfigFile.RootDirectory = LauncherSettings.Instance.SaveFolderPath;
        InitializeComponent();
    }

    /// <summary>
    ///     Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppWindow = new MainWindow();
        AppWindow.Activate();
    }
}