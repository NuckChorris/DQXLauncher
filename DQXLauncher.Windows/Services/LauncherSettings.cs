using System;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Core.Services;

namespace DQXLauncher.Windows.Services;

public partial class LauncherSettings : ObservableObject
{
    [ObservableProperty] private string? _gameFolderPath;
    [ObservableProperty] private string? _saveFolderPath;

    private static readonly string SettingsPath = Path.Combine(Paths.AppData, "LauncherSettings.json");

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