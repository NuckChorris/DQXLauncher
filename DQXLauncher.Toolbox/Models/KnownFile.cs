﻿using System;
using System.IO;
using System.Collections.Generic;
using DQXLauncher.Core.StreamObfuscator;

namespace DQXLauncher.Toolbox.Models;

public class KnownFile
{
    public required Func<Stream, Stream> Obfuscator;
    public required string Name;
    
    public static readonly Dictionary<string, KnownFile> KnownFiles = new()
    {
        { "FbrBrpgbkOhwj!ouc", new KnownFile { Name = "KeyConfigFile.xml", Obfuscator = FixedObfuscator.Factory } },
        { "cxjYxsgheGzie!iyx", new KnownFile { Name = "dqxPlayerList.xml", Obfuscator = UsernameObfuscator.Factory } },
        { "NeaRxpzuwPlieoha!oub]#!xui", new KnownFile { Name = "PadButtonCaption.win32.xml", Obfuscator = FixedObfuscator.Factory } },
        { "YzjadjylzqvBmsvub!zni]]!vnh", new KnownFile { Name = "EnvironmentOption.win32.xml", Obfuscator = FixedObfuscator.Factory } }
    };

}