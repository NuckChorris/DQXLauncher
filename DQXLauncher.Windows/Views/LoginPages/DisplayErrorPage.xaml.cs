using System.Diagnostics.Contracts;
using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Core.Game.LoginStrategy;
using DQXLauncher.Windows.ViewModels;
using Microsoft.UI.Xaml;

namespace DQXLauncher.Windows.Views.LoginPages;

public partial class DisplayErrorPage
{
    public LoginFrameViewModel ViewModel { get; set; }

    public DisplayError Step
    {
        get
        {
            Contract.Assert(ViewModel.Step is not null);
            Contract.Assert(ViewModel.Step is DisplayError);
            return (DisplayError)ViewModel.Step;
        }
    }

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