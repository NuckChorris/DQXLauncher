using System;
using System.Collections.Generic;
using Windows.System;
using DQXLauncher.Core.Hiroba;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DQXLauncher.Components;

public sealed partial class RotationBanner : UserControl
{
    public List<BannerImage>? Banners { get; private set; }
    
    public RotationBanner()
    {
        LoadBanners();
        InitializeComponent();
    }
    
    private async void LoadBanners()
    {
        Banners = await BannerImage.GetBanners();
        FlipView.ItemsSource = Banners;
        FlipView.Visibility = Visibility.Visible;
        ProgressRing.Visibility = Visibility.Collapsed;
    }

    private async void Banner_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is BannerImage banner && banner.Href != null)
        {
            var uri = new Uri(banner.Href);
            await Launcher.LaunchUriAsync(uri);
        }
    }
}