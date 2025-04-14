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

    public TitleBar()
    {
        this.InitializeComponent();
    }
}