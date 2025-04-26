using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Windows.ViewModels;
using DQXLauncher.Windows.Views.LoginPages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace DQXLauncher.Windows.Views;

public partial class LoginFrame : UserControl
{
    public LoginFrameViewModel ViewModel { get; set; }

    public LoginFrame()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<LoginFrameViewModel>();
        ViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ViewModel.Step)) DisplayStep(ViewModel.Step);
        };
    }

    private void Navigate(Type pageType, object? parameter = null)
    {
        Frame.Navigate(pageType, parameter,
            new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
    }

    private void DisplayStep(LoginStep step)
    {
        switch (step)
        {
            case AskUsernamePassword:
                Navigate(typeof(UsernamePasswordPage));
                break;
            case AskPassword:
                Frame.Navigate(typeof(PasswordPage));
                break;
        }
    }
}