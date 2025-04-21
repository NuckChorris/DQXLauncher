namespace DQXLauncher.Core.Services;

public static class Paths
{
    // We NEED this to not be null for anything to function.
    public static string AppData { get; set; } = null!;

    public static string Cache => Path.Combine(AppData, "Cache");

    public static void Create()
    {
        Directory.CreateDirectory(Cache);
        Directory.CreateDirectory(AppData);
    }
}