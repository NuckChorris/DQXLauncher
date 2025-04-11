using System;
using System.IO;
using System.Text.Json;
using DQXLauncher.Core.Game;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DQXLauncher.Toolbox.Services;

public partial class Settings : ObservableObject
{
    private static Settings? _instance;
    public static Settings Instance => _instance ??= Load();

    [ObservableProperty]
    public string gameFolderPath = InstallInfo.Location ?? throw new Exception("InstallInfo.Location is null");
    
    [ObservableProperty]
    public string saveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                                                "My Games", "Dragon Quest X");

    private static readonly string SettingsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                     "DQXLauncher", "Toolbox", "settings.json");

    private Settings() { }

    public static Settings Load()
    {
        if (File.Exists(SettingsPath))
        {
            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
        }
        return new Settings();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this));
    }
}
