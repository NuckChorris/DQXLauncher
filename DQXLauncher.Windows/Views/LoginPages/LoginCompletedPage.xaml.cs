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
    private LoginStrategy Strategy => ViewModel.Strategy ?? throw new InvalidOperationException("Invalid strategy");

    private LoginCompleted Step =>
        ViewModel.Step is LoginCompleted step ? step : throw new InvalidOperationException("Invalid step");

    public LoginCompletedPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<LoginFrameViewModel>();
        PlayerListViewModel = Ioc.Default.GetRequiredService<PlayerListViewModel>();
        Launcher = Ioc.Default.GetRequiredService<Launcher>();
    }

    private async void LoginCompletedPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Strategy is NewPlayerLoginStrategy strategy && Step.Token is not null)
        {
            PlayerListViewModel.AddPlayer(Step.Token, strategy.Username);
            await PlayerListViewModel.SaveAsync();
        }

        Launcher.SessionId = Step.SessionId;
        await Launcher.LaunchGame();
    }
}