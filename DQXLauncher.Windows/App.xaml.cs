using System;
using System.Diagnostics.Contracts;
using System.IO;
using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Core.Game.ConfigFile;
using DQXLauncher.Core.Services;
using DQXLauncher.Windows.Services;
using DQXLauncher.Windows.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Serilog;

namespace DQXLauncher.Windows;

public partial class App : Application
{
    public static Window? AppWindow;
    private readonly ServiceProvider Services;

    public App()
    {
        Paths.AppData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            , "DQXLauncher");
        Paths.Create();
        ConfigFile.RootDirectory = LauncherSettings.Instance.SaveFolderPath;
        InitializeComponent();
        Services = CreateServiceProvider();
        Ioc.Default.ConfigureServices(Services);
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppWindow = Services.GetService<MainWindow>();
        Contract.Assert(AppWindow is not null);
        AppWindow.Activate();
    }

    private ServiceProvider CreateServiceProvider()
    {
        ServiceCollection services = new();
        services.AddTransient<MainWindow>();
        services.AddSingleton<MainFrameViewModel>();
        services.AddLogging(lb => { lb.AddSerilog(BuildLogger(), true); });

        return services.BuildServiceProvider();
    }

    private ILogger BuildLogger()
    {
        var cfg = new LoggerConfiguration();
        cfg.Enrich.FromLogContext();

        return cfg.CreateLogger();
    }
}