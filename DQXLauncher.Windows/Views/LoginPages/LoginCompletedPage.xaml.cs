using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Core.Game;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Windows.ViewModels;
using Microsoft.UI.Xaml;

namespace DQXLauncher.Windows.Views.LoginPages;

public partial class LoginCompletedPage
{
    public LoginFrameViewModel ViewModel { get; set; }
    public PlayerListViewModel PlayerListViewModel { get; set; }
    public Launcher Launcher { get; set; }

    public LoginCompletedPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<LoginFrameViewModel>();
        PlayerListViewModel = Ioc.Default.GetRequiredService<PlayerListViewModel>();
        Launcher = Ioc.Default.GetRequiredService<Launcher>();
    }

    private async void LoginCompletedPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Step is LoginCompleted step)
        {
            if (ViewModel.Strategy is NewPlayerLoginStrategy)
            {
                PlayerListViewModel.PlayerList?.Add(step.Token!);
                await PlayerListViewModel.PlayerList?.SaveAsync();
            }

            Launcher.SessionId = step.SessionId;
            await Launcher.LaunchGame();
        }
        else
        {
            throw new InvalidOperationException("Invalid step");
        }
    }
}