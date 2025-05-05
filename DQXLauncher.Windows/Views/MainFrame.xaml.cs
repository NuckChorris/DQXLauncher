using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Windows.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.Views;

public partial class MainFrame : UserControl
{
    public MainFrame()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<MainFrameViewModel>();
        Frame.Navigate(ViewModel.Page);
        ViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ViewModel.CurrentPage)) Frame.Navigate(ViewModel.Page);
            if (e.PropertyName == nameof(ViewModel.WindowHeight)) App.AppWindow.Height = ViewModel.WindowHeight;
        };
    }

    public MainFrameViewModel ViewModel { get; set; }
}