using System.Xml.Linq;
using DQXLauncher.Core.Utils;

namespace DQXLauncher.Core.Game.ConfigFile;

public class InvalidConfigException(string fileContents) : Exception("Invalid config")
{
    public string FileContents { get; } = fileContents;
}

public abstract class ConfigFile
{
    protected virtual string DefaultContents => string.Empty;
    public static string RootDirectory { get; set; } = Environment.CurrentDirectory;
    public readonly string Filename;
    private readonly Func<Stream, Stream> _obfuscatorFactory;
    public XDocument? Document;

    protected ConfigFile(string filename, int seed, Func<Stream, Stream> obfuscatorFactory)
    {
        var obfuscatedName = FilenameObfuscator.Obfuscate(filename, seed);
        Filename = Path.Combine(RootDirectory, obfuscatedName);

        if (Path.GetDirectoryName(Filename) is { } dir)
        {
            Directory.CreateDirectory(dir);
        }

        _obfuscatorFactory = obfuscatorFactory;
    }

    protected virtual async Task _LoadAsync()
    {
        await EnsureCreated();

        await using var fileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        await using var obfuscatorStream = _obfuscatorFactory(fileStream);

        Document = await Task.Run(() => XDocument.Load(obfuscatorStream));
    }

    public async Task SaveAsync()
    {
        await using var fileStream = new FileStream(Filename, FileMode.Truncate, FileAccess.Write, FileShare.Read);
        await using var obfuscatorStream = _obfuscatorFactory(fileStream);
        await Task.Run(() => Document?.Save(obfuscatorStream));
        await obfuscatorStream.FlushAsync();
    }

    private async Task EnsureCreated()
    {
        if (!File.Exists(Filename))
        {
            await using var fileStream = new FileStream(Filename, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            await using var obfuscatorStream = _obfuscatorFactory(fileStream);
            await using var writer = new StreamWriter(obfuscatorStream);

            await writer.WriteAsync(DefaultContents);
            await writer.FlushAsync();
        }
    }

    protected InvalidConfigException Invalid()
    {
        using var fileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var obfuscatorStream = _obfuscatorFactory(fileStream);
        var reader = new StreamReader(obfuscatorStream);
        return new InvalidConfigException(reader.ReadToEnd());
    }
}