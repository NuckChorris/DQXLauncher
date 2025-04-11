﻿using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DQXLauncher.Toolbox.Models;

namespace DQXLauncher.Toolbox.ViewModels;

public class SaveExplorerItem : ObservableObject
{
    public required string Path { get; set; }
    public string Name => System.IO.Path.GetFileName(Path);
    public bool IsDirectory => Directory.Exists(Path);
    public KnownFile? FileInfo => KnownFile.KnownFiles.TryGetValue(Name, out var file) ? file : null;
    public string? DeobfuscatedName => FileInfo?.Name;

    public ObservableCollection<SaveExplorerItem>? Children
    {
        get
        {
            if (!IsDirectory) return null;
            return new(Directory.GetFileSystemEntries(Path)
                .Select(x => new SaveExplorerItem { Path = x }).ToList());
        }
    }
}
