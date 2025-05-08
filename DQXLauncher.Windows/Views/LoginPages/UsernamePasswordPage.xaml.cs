using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.Views.LoginPages;

public sealed partial class UsernamePasswordPage : Page
{
    public LoginFrameViewModel ViewModel { get; set; }
    private LoginStrategy Strategy => ViewModel.Strategy ?? throw new InvalidOperationException("Invalid strategy");

    private AskUsernamePassword Step =>
        ViewModel.Step is AskUsernamePassword step ? step : throw new InvalidOperationException("Invalid step");

    public UsernamePasswordPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<LoginFrameViewModel>();
        Username.Text = Step.Username;
        Password.Password = Step.Password;
    }

    [RelayCommand]
    private async Task SubmitLogin()
    {
        var action = new UsernamePasswordAction(Username.Text, Password.Password);
        ViewModel.Forward(await Strategy.Step(action));
    }
}