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
        ViewModel.StepChanged += (_, change) =>
        {
            Frame.Navigate(PageForStep(change.Step), null, GetTransitionForDirection(change.Direction));
        };
    }

    private Type? PageForStep(LoginStep step)
    {
        switch (step)
        {
            case AskUsernamePassword: return typeof(UsernamePasswordPage);
            case AskPassword: return typeof(PasswordPage);
            case DisplayError: return typeof(DisplayErrorPage);
            case LoginCompleted: return typeof(LoginCompletedPage);
            default: return null;
        }
    }

    private NavigationTransitionInfo GetTransitionForDirection(LoginFrameViewModel.StepChangeDirection direction)
    {
        switch (direction)
        {
            case LoginFrameViewModel.StepChangeDirection.Backward:
                return new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromLeft };
            case LoginFrameViewModel.StepChangeDirection.None:
                return new SuppressNavigationTransitionInfo();
            default:
                return new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight };
        }
    }
}