using DQXLauncher.Core.Utils;
using DQXLauncher.Core.StreamObfuscator;
using System.Xml.Linq;

namespace DQXLauncher.Core.Game.ConfigFile;

public class InvalidConfigException(string fileName, string fileContents) : Exception($"Invalid config in {fileName}")
{
    public string FileContents { get; } = fileContents;
}

public abstract class ConfigFile : IDisposable
{
    protected virtual string DefaultContents => string.Empty;
    public static string RootDirectory { get; set; } = Environment.CurrentDirectory;
    public readonly string Filename;
    private readonly Stream _fileStream;
    private readonly Stream _obfuscatorStream;
    public XDocument? Document;

    protected ConfigFile(string filename, int seed, Func<Stream, Stream> obfuscatorFactory)
    {
        var obfuscatedName = FilenameObfuscator.Obfuscate(filename, seed);
        Filename = Path.Combine(RootDirectory, obfuscatedName);

        if (Path.GetDirectoryName(Filename) is { } dir)
        {
            Directory.CreateDirectory(dir);
        }

        _fileStream = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        _obfuscatorStream = obfuscatorFactory(_fileStream);
    }

    public void Dispose()
    {
        _obfuscatorStream.Dispose();
        _fileStream.Dispose();
    }

    public virtual async Task Load()
    {
        _obfuscatorStream.Position = 0;
        
        // Write our default value if it's empty
        if (_fileStream.Length == 0)
        {
            await using var writer = new StreamWriter(_obfuscatorStream, leaveOpen: true);
            await writer.WriteAsync(DefaultContents);
            await writer.FlushAsync();
            _obfuscatorStream.Position = 0;
        }
        
        Document = await Task.Run(() => XDocument.Load(_obfuscatorStream));
    }
    
    public async Task Save()
    {
        _obfuscatorStream.Position = 0;
        Document?.Save(_obfuscatorStream);
        await _obfuscatorStream.FlushAsync();
    }
    
    protected InvalidConfigException Invalid()
    {
        _obfuscatorStream.Position = 0;
        var reader = new StreamReader(_obfuscatorStream);
        return new InvalidConfigException(Filename, reader.ReadToEnd());
    }
}