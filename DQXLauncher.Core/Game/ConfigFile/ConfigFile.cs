﻿using System.Text;
using System.Xml;
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
        using var reader = new StreamReader(obfuscatorStream, new UTF8Encoding(false));

        Document = await Task.Run(() => XDocument.Load(reader));
    }

    public async Task SaveAsync()
    {
        await using var fileStream = new FileStream(Filename, FileMode.Truncate, FileAccess.Write, FileShare.Read);
        await using var obfuscatorStream = _obfuscatorFactory(fileStream);
        await using var writer =
            new StreamWriter(obfuscatorStream, new UTF8Encoding(false));
        await writer.WriteAsync(await Serialize());
        await writer.FlushAsync();
    }

    private async Task<string> Serialize()
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "\t",
            Async = true,
            NewLineChars = "\n",
            NewLineHandling = NewLineHandling.Replace,
            Encoding = new UTF8Encoding(false),
            NewLineOnAttributes = false
        };
        await using var ms = new MemoryStream();
        await using var writer = XmlWriter.Create(ms, settings);
        await Task.Run(() => Document?.Save(writer));
        await writer.FlushAsync();
        var stringResult = Encoding.UTF8.GetString(ms.ToArray());
        // Precisely match the exact format of the original file, DQXLauncher is very brittle!
        return stringResult.Replace(" />", "/>").Replace("utf-8", "UTF-8") + "\n";
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