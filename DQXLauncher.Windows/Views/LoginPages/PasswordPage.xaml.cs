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

    public PasswordPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<LoginFrameViewModel>();
        if (ViewModel.Step is AskPassword step)
            Username.Text = step.Username;
        else
            throw new InvalidOperationException("Invalid step");
    }

    [RelayCommand]
    private async Task SubmitLogin()
    {
        var action = new PasswordAction(Password.Password);
        ViewModel.Step = await ViewModel.Strategy.Step(action);
    }
}