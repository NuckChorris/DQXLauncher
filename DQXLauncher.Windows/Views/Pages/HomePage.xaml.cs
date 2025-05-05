using System.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.Views.Pages;

public sealed partial class HomePage : Page
{
    public LoginFrameViewModel ViewModel { get; set; }

    public HomePage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<LoginFrameViewModel>();
    }

    private async void PlayerList_OnPlayerSelected(object? sender, PlayerListView.PlayerSelectedEventArgs e)
    {
        if (e.Item is SavedPlayerItem savedPlayer)
        {
            var strategy = new SavedPlayerLoginStrategy();
            ViewModel.IsLoading = true;
            ViewModel.Start(strategy, await strategy.Step(savedPlayer.Player.Token));
            ViewModel.IsLoading = false;
            Debug.WriteLine("Stepping saved strategy");
        }
        else if (e.Item is NewPlayerItem)
        {
            var strategy = new NewPlayerLoginStrategy();
            ViewModel.IsLoading = true;
            ViewModel.Start(strategy, await strategy.Step());
            ViewModel.IsLoading = false;
            Debug.WriteLine("Stepping Guest strategy");
        }
    }
}