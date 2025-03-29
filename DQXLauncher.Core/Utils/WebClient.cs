﻿using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace DQXLauncher.Core.Utils;

/// <summary>
/// Provides an HttpClient configured to use a cookie jar and roughly emulate the behavior of the original launcher's
/// WebViews.
/// </summary>
public class WebClient
{
    private static readonly ConcurrentDictionary<string, Task<HttpClient>> HttpClients = new ConcurrentDictionary<string, Task<HttpClient>>();
    
    /// <summary>
    /// Get an HttpClient configured to use a cookie jar with the given key. If one already exists for the given key, it
    /// will be returned.
    /// </summary>
    /// <param name="key">The name of the cookie jar to use</param>
    /// <returns>The HttpClient to use</returns>
    public static async Task<HttpClient> GetHttpClient(string key = "default")
    {
        return await HttpClients.GetOrAdd(key, BuildHttpClient);
    }
    
    private static async Task<HttpClient> BuildHttpClient(string key = "default")
    {
        var handler = await CookieJar.HandlerForJar(new HttpClientHandler(), key);
        var client = new HttpClient(handler);
        
        client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
        client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml;q=0.9,*/*;q=0.8");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
        // I wish I were making this up lmao
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", $"User-Agent: SQEXAuthor/2.0.0(Windows 6.2; ja-jp; {MakeComputerId()})");
        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        
        return client;
    }
    
    /// <summary>
    /// Generate a computer ID to send to Square Enix to identify this computer.
    /// </summary>
    /// <remarks>
    /// This method is lifted from and copyright of the FFXIVQuickLauncher project, specifically from the
    /// <c>XIVLauncher.Common.Game.Launcher</c> class. The original source code can be found on
    /// <see href="https://github.com/goatcorp/FFXIVQuickLauncher/blob/19c603de1ec038136bdb14d65924bd525131d3fb/src/XIVLauncher.Common/Game/Launcher.cs#L664-L680">their github</see>.
    /// </remarks>
    private static string MakeComputerId()
    {
        var hashString = Environment.MachineName + Environment.UserName + Environment.OSVersion +
                         Environment.ProcessorCount;

        using var sha1 = SHA1.Create();

        var bytes = new byte[5];

        Array.Copy(sha1.ComputeHash(Encoding.Unicode.GetBytes(hashString)), 0, bytes, 1, 4);

        var checkSum = (byte) -(bytes[1] + bytes[2] + bytes[3] + bytes[4]);
        bytes[0] = checkSum;

        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}