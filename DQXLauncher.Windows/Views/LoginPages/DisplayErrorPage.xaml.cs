using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Windows.ViewModels;
using Microsoft.UI.Xaml;

namespace DQXLauncher.Windows.Views.LoginPages;

public partial class DisplayErrorPage
{
    public LoginFrameViewModel ViewModel { get; set; }
    private LoginStrategy Strategy => ViewModel.Strategy ?? throw new InvalidOperationException("Invalid strategy");

    private DisplayError Step =>
        ViewModel.Step is DisplayError step ? step : throw new InvalidOperationException("Invalid step");


    public DisplayErrorPage()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<LoginFrameViewModel>();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel.Backward(Step.Continue);
    }
}