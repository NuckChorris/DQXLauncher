using DQXLauncher.Windows.Services;

namespace DQXLauncher.Windows;

public enum Theme
{
    Light,
    Dark,
    System
}

public class Settings
{
    public Theme Theme { get; set; } = Theme.Light;
    public string? GamePath { get; set; } = InstallInfo.Location;
}