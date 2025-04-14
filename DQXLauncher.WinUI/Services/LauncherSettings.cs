using System;
using System.IO;
using System.Text.Json;
using DQXLauncher.Core.Game;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DQXLauncher.Services;

public partial class LauncherSettings : ObservableObject
{
    private static LauncherSettings? _instance;
    public static LauncherSettings Instance => _instance ??= Load();

    [ObservableProperty] private string? _gameFolderPath;
    [ObservableProperty] private string? _saveFolderPath;

    private static readonly string SettingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DQXLauncher", "launcherSettings.json");

    private LauncherSettings() {}

    private static LauncherSettings GetDefaults()
    {
        return new LauncherSettings
        {
            GameFolderPath = InstallInfo.Location,
            SaveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "My Games", "Dragon Quest X")
        };
    }

    public static LauncherSettings Load()
    {
        if (File.Exists(SettingsPath))
        {
            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<LauncherSettings>(json) ?? GetDefaults();
        }
        return GetDefaults();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this));
    }
}