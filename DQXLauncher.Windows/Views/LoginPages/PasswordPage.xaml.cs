using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.Views.LoginPages;

public sealed partial class PasswordPage : Page
{
    public LoginFrameViewModel ViewModel { get; set; }
    private LoginStrategy Strategy => ViewModel.Strategy ?? throw new InvalidOperationException("Invalid strategy");

    private AskPassword Step =>
        ViewModel.Step is AskPassword step ? step : throw new InvalidOperationException("Invalid step");

    public PasswordPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<LoginFrameViewModel>();
        Username.Text = Step.Username;
        Password.Password = Step.Password;
    }

    [RelayCommand]
    private async Task SubmitLogin()
    {
        var action = new PasswordAction(Password.Password);
        ViewModel.Forward(await Strategy.Step(action));
    }
}