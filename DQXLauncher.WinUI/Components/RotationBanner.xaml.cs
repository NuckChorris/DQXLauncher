using System;
using System.Collections.Generic;
using Windows.System;
using DQXLauncher.Core.Hiroba;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System.UserProfile;
using System.Linq;

namespace DQXLauncher.Components;

public sealed partial class RotationBanner : UserControl
{
    public List<BannerImage>? Banners { get; private set; }
    private readonly DispatcherTimer _flipTimer;
    
    public RotationBanner()
    {
        LoadBanners();
        InitializeComponent();

        _flipTimer = new DispatcherTimer {
            Interval = TimeSpan.FromSeconds(5)
        };
        _flipTimer.Tick += FlipTimer_Tick;
        _flipTimer.Start();
    }

    private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _flipTimer.Stop();
    }

    private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        _flipTimer.Start();
    }
    
    private void Grid_GotFocus(object sender, RoutedEventArgs e)
    {
        _flipTimer.Stop();
    }
    
    private void Grid_LostFocus(object sender, RoutedEventArgs e)
    {
        _flipTimer.Start();
    }
    
    private async void LoadBanners()
    {
        Banners = await BannerImage.GetBanners();
        FlipView.ItemsSource = Banners;
        FlipView.Visibility = Visibility.Visible;
        ProgressRing.Visibility = Visibility.Collapsed;
    }

    private void FlipTimer_Tick(object? sender, object e)
    {
        if (FlipView.Items.Count > 0)
        {
            FlipView.SelectedIndex = (FlipView.SelectedIndex + 1) % FlipView.Items.Count;
        }
    }
}