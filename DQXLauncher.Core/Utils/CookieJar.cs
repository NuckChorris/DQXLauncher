using System.Diagnostics;
using System.Net;
using System.Text.Json;
using DQXLauncher.Core.Services;

namespace DQXLauncher.Core.Utils;

public class CookieJar : DelegatingHandler
{
    public class CookiesDictionary : Dictionary<string, List<Cookie>>
    {
        public List<Cookie> GetOrCreate(string key)
        {
            if (!TryGetValue(key, out var list))
            {
                list = new List<Cookie>();
                this[key] = list;
            }

            return list;
        }
    }

    private static string JarPath => Paths.Cache;
    private readonly string _jarFile;
    public CookiesDictionary Cookies = new();

    private CookieJar(HttpMessageHandler innerHandler, string jarName) : base(innerHandler)
    {
        _jarFile = Path.Combine(JarPath, $"{jarName}.cookies.json");
        Directory.CreateDirectory(JarPath);
    }

    public static async Task<CookieJar> HandlerForJar(HttpMessageHandler innerHandler, string jarName)
    {
        var jar = new CookieJar(innerHandler, jarName);
        await jar.Load();
        return jar;
    }

    private void Cleanup()
    {
        foreach (var key in Cookies.Keys.ToList()) Cookies[key].RemoveAll(c => c.Expires <= DateTime.UtcNow);
    }

    public async Task Save()
    {
        Cleanup();
        await using FileStream jarStream = new FileStream(_jarFile, FileMode.Create);
        await JsonSerializer.SerializeAsync(jarStream, Cookies);
    }

    public async Task Load()
    {
        try
        {
            await using var stream =
                new FileStream(_jarFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            Cookies = await JsonSerializer.DeserializeAsync<CookiesDictionary>(stream) ?? new CookiesDictionary();
        }
        catch (Exception)
        {
            Cookies = new CookiesDictionary();
        }

        Cleanup();
    }

    public async Task Clear()
    {
        Cookies.Clear();
        await Save();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Cleanup();
        Debug.Assert(request.RequestUri != null, "request.RequestUri != null");

        // Attach cookies for the current URI
        foreach (var cookie in GetCookiesForDomainAndPath(request.RequestUri.Host, request.RequestUri.AbsolutePath))
        {
            request.Headers.Add("Cookie", cookie.ToString());
        }

        // Send the request
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        // Save any Set-Cookie headers
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookieHeader in cookies)
            {
                SetCookie(cookieHeader, request.RequestUri.Host);
            }

            await Save();
        }

        return response;
    }

    private void SetCookie(string header, string domain)
    {
        var cookie = CookieParser.ParseCookie(header, domain);
        var cookies = Cookies.GetOrCreate(domain);

        // Remove any existing cookie with the same name
        cookies.RemoveAll(c => c.Name == cookie.Name);
        cookies.Add(cookie);
    }

    private List<Cookie> GetCookiesForDomainAndPath(string domain, string path)
    {
        if (!Cookies.TryGetValue(domain, out var cookies))
        {
            return [];
        }

        return cookies
            .Where(cookie => path.StartsWith(cookie.Path) && cookie.Expires > DateTime.UtcNow)
            .ToList();
    }
}