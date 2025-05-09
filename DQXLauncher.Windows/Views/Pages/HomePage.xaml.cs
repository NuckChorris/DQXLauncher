using CommunityToolkit.Mvvm.DependencyInjection;
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
        var strategy = e.Item.LoginStrategy;
        ViewModel.IsLoading = true;
        ViewModel.Start(strategy, await strategy.Start());
        ViewModel.IsLoading = false;
    }
}