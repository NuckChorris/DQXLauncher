using CommunityToolkit.Mvvm.DependencyInjection;
using DQXLauncher.Windows.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Windows.Views;

public partial class TitleBar : UserControl
{
    public double TitleBarHeight { get; set; }
    public double ActualTitleBarHeight => TitleBarHeight / XamlRoot.RasterizationScale;
    public double TitleBarRightInset { get; set; }
    public double TitleBarLeftInset { get; set; }

    public Thickness TitleBarMargins => new(TitleBarLeftInset / XamlRoot.RasterizationScale + 10, 0,
        TitleBarRightInset / XamlRoot.RasterizationScale + 10, 0);

    public MainFrameViewModel ViewModel { get; set; }

    public TitleBar()
    {
        this.InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<MainFrameViewModel>();
    }
}