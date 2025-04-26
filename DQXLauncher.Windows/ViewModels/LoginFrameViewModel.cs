using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Core.Game.LoginStrategy;

namespace DQXLauncher.Windows.ViewModels;

public partial class LoginFrameViewModel : ObservableObject
{
    [ObservableProperty] private LoginStrategy _strategy;

    [ObservableProperty] private LoginStep _step;
}