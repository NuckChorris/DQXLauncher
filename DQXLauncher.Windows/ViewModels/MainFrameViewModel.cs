using System;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Windows.Views.Pages;

namespace DQXLauncher.Windows.ViewModels;

public enum MainFramePage
{
    Home = 0,
    Settings = 1
}

public partial class MainFrameViewModel : ObservableObject
{
    [ObservableProperty] private int _pageIndex;

    [ObservableProperty] private MainFramePage _currentPage = MainFramePage.Home;

    [ObservableProperty] private int _windowHeight;

    partial void OnPageIndexChanged(int oldValue, int newValue)
    {
        CurrentPage = (MainFramePage)newValue;
        WindowHeight = newValue == (int)MainFramePage.Home ? 370 : 460;
    }

    partial void OnCurrentPageChanged(MainFramePage oldValue, MainFramePage newValue)
    {
        PageIndex = (int)newValue;
        WindowHeight = newValue == MainFramePage.Home ? 370 : 460;
    }

    public Type Page
    {
        get
        {
            switch (CurrentPage)
            {
                case MainFramePage.Home: return typeof(HomePage);
                case MainFramePage.Settings: return typeof(SettingsPage);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}