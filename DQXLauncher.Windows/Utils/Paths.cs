using System;
using System.IO;

namespace DQXLauncher.Windows.Utils;

public static class Paths
{
    public static string AppData => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        , "DQXLauncher");

    public static string Cache => Path.Combine(AppData, "Cache");

    public static void Create()
    {
        Directory.CreateDirectory(AppData);
        Directory.CreateDirectory(Cache);
    }
}