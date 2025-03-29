using DQXLauncher.Core.Utils;
using DQXLauncher.Core.StreamObfuscator;

namespace DQXLauncher.Core.Game;

public class ConfigFile
{
    public static string RootDirectory { get; set; } = Environment.CurrentDirectory;
    
    private static readonly Dictionary<Type, Func<Stream, Stream>> ObfuscatorMappings = new()
    {
        { typeof(PlayerListObfuscator), s => new PlayerListObfuscator(s, Environment.UserName) },
        { typeof(ConfigObfuscator), s => new ConfigObfuscator(s) }
    };
    
    public static Task<Stream> OpenAsync(string originalFilename, Func<Stream, Stream> obfuscatorFactory, FileAccess access)
    {
        string obfuscatedName = FilenameObfuscator.Obfuscate(originalFilename);
        string fullPath = Path.Combine(RootDirectory, obfuscatedName);

        Directory.CreateDirectory(RootDirectory);

        var baseStream = access == FileAccess.Read ? new FileStream(fullPath, FileMode.Open, access, FileShare.Read) : new FileStream(fullPath, FileMode.OpenOrCreate, access, FileShare.None);

        return Task.FromResult(obfuscatorFactory(baseStream));
    }

    public static async Task<Stream> OpenAsync<T>(string originalFilename, FileAccess access) where T : Stream
    {
        if (!ObfuscatorMappings.TryGetValue(typeof(T), out var factory))
            throw new InvalidOperationException($"No factory registered for type {typeof(T).Name}.");

        return await OpenAsync(originalFilename, factory, access);
    }
    
    public static async Task<Stream> OpenPlayerListAsync(FileAccess access)
    {
        return await OpenAsync<PlayerListObfuscator>("dqxPlayerList.xml", access);
    }
}