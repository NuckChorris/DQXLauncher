using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace DQXLauncher.Components;

public partial class TitleBar : UserControl
{
    public double TitleBarHeight { get; set; }
    public double ActualTitleBarHeight => TitleBarHeight / XamlRoot.RasterizationScale;
    public double TitleBarRightInset { get; set; }
    public double TitleBarLeftInset { get; set; }

    public Thickness TitleBarMargins => new Thickness(TitleBarLeftInset / XamlRoot.RasterizationScale + 10, 0, TitleBarRightInset / XamlRoot.RasterizationScale + 10, 0);
    
    public TitleBar()
    {
        this.InitializeComponent();
    }
}