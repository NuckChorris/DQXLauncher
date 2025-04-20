using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace DQXLauncher.Core.Utils;

public class CookieJar : DelegatingHandler
{
    private class Cookies : Dictionary<string, List<Cookie>>
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

    public static string? JarPath;
    private readonly string _jarFile;
    private Cookies _cookies = new Cookies();

    private CookieJar(HttpMessageHandler innerHandler, string jarName) : base(innerHandler)
    {
        if (JarPath is null)
            throw new InvalidOperationException("CookieJar.JarPath must be set before creating a CookieJar");
        _jarFile = Path.Combine(JarPath, $"{jarName}.cookies.json");
        Directory.CreateDirectory(JarPath);
    }

    public static async Task<CookieJar> HandlerForJar(HttpMessageHandler innerHandler, string jarName)
    {
        var jar = new CookieJar(innerHandler, jarName);
        await jar.Load();
        return jar;
    }

    public async Task Save()
    {
        await using FileStream jarStream = new FileStream(_jarFile, FileMode.Create);
        await JsonSerializer.SerializeAsync(jarStream, _cookies);
    }

    public async Task Load()
    {
        try
        {
            await using var stream =
                new FileStream(_jarFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            _cookies = await JsonSerializer.DeserializeAsync<Cookies>(stream) ?? new Cookies();
        }
        catch (Exception)
        {
            _cookies = new Cookies();
        }
    }

    public async Task Clear()
    {
        _cookies.Clear();
        await Save();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
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
        var cookie = ParseCookie(header, domain);
        var cookies = _cookies.GetOrCreate(domain);

        // Remove any existing cookie with the same name
        cookies.RemoveAll(c => c.Name == cookie.Name);
        cookies.Add(cookie);
    }

    private List<Cookie> GetCookiesForDomainAndPath(string domain, string path)
    {
        if (!_cookies.TryGetValue(domain, out var cookies))
        {
            return [];
        }

        return cookies
            .Where(cookie => path.StartsWith(cookie.Path) && cookie.Expires > DateTime.UtcNow)
            .ToList();
    }

    private Cookie ParseCookie(string header, string domain)
    {
        // Example header: "DQXLogin=1234567890; Path=/; Secure; Expires=Wed, 21 Oct 2023 07:28:00 GMT"
        // Split by ; to get flag segments and the first one is the key=value pair
        var splitFlags = header.Split(';');
        // Split out the key=value pair, that part is done
        var keyValue = splitFlags[0].Split('=');
        // Flags are complicated. We parse them into a dictionary from key=value pairs, and keys without values are true
        var flags = splitFlags
            .Skip(1)
            .Select(part => part.Trim())
            .Where(part => !string.IsNullOrEmpty(part)).Select(
                part =>
                {
                    var flagParts = part.Split('=', 2);
                    if (flagParts.Length == 2) return new KeyValuePair<string, object>(flagParts[0], flagParts[1]);

                    return new KeyValuePair<string, object>(flagParts[0], true);
                }).ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

        var expiry = ParseCookieExpiry(
            flags.TryGetValue("Max-Age", out var maxAgeObj) ? maxAgeObj as string : null,
            flags.TryGetValue("Expires", out var expiresObj) ? expiresObj as string : null
        );

        var cookie = new Cookie()
        {
            Name = keyValue[0],
            Value = keyValue[1],
            Secure = flags.ContainsKey("Secure"),
            HttpOnly = flags.ContainsKey("HttpOnly"),
            Domain = flags.TryGetValue("Domain", out var domainObj) ? domainObj as string : domain,
            Path = flags.TryGetValue("Path", out var pathObj) ? pathObj as string : "/"
        };
        // Manually apply the expiry because... yeah
        if (expiry is not null) cookie.Expires = expiry.Value;

        return cookie;
    }

    private DateTime? ParseCookieExpiry(string? maxAge, string? expires)
    {
        if (!String.IsNullOrEmpty(maxAge) && int.TryParse(maxAge, out var maxAgeSeconds))
        {
            return DateTime.UtcNow.AddSeconds(maxAgeSeconds);
        }
        else if (!string.IsNullOrEmpty(expires))
        {
            return ParseDate(expires);
        }

        return null;
    }

    private DateTime ParseDate(string? time)
    {
        if (string.IsNullOrEmpty(time))
        {
            return DateTime.MaxValue;
        }

        return DateTime.ParseExact(time, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", null, DateTimeStyles.AssumeUniversal);
    }
}