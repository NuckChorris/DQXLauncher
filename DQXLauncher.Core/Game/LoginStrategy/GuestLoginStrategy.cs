using System.Net;
using System.Security.Cryptography;
using System.Text;
using HtmlAgilityPack;

namespace DQXLauncher.Core.Game.LoginStrategy;

public class GuestLoginStrategy : LoginStrategy
{
    public class LoginForm
    {
        public required string Action { get; set; }
        public required Dictionary<string, string> Form { get; set; }
    }

    public async Task<LoginForm> GetLoginForm()
    {
        var doc = await GetLoginDocument(new Dictionary<string, string>
        {
            { "dqxmode", "3" }
        });

        var form = doc.DocumentNode.SelectSingleNode("//form[@name='mainForm']");
        var action = new Uri(new Uri(LOGIN_URL), WebUtility.HtmlDecode(form.GetAttributeValue("action", "")));
        return new LoginForm
        {
            Action = action.ToString(),
            Form = form
                .Descendants("input")
                .ToDictionary(
                    node => node.GetAttributeValue("name", ""),
                    node => node.GetAttributeValue("value", "")
                )
        };
    }

    public async Task<string> Login(string username, string password)
    {
        var httpClient = await GetHttpClient();
        var loginForm = await GetLoginForm();

        var request = new HttpRequestMessage(HttpMethod.Post, loginForm.Action);
        
        loginForm.Form["sqexid"] = username;
        loginForm.Form["password"] = password;
        request.Content = new FormUrlEncodedContent(loginForm.Form);
        
        var doc = new HtmlDocument();
        var response = await httpClient.SendAsync(request);
        doc.LoadHtml(await response.Content.ReadAsStringAsync());

        return doc.DocumentNode.SelectSingleNode("//x-sqexauth").GetAttributeValue("sid", "");
    }

    public string EncodeSessionId(string sid)
    {
        if (!IsValidHex(sid)) throw new ArgumentException("Input must be a 56-character hex string.");

        string timeStr = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60).ToString();

        string input = $"DQUEST10{sid}";
        byte[] md5 = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes($"{timeStr}DraqonQuestX"));

        byte[] output = new byte[64];
        // Encoding loop
        for (int i = 0; i < 64; i++)
        {
            int ecx = md5[i % 16];
            int eax = i < input.Length ? input[i] : 0;
            ecx -= 48;
            eax += ecx;
            eax %= 78;
            eax += 48;
            output[i] = (byte)eax;
        }

        return Encoding.UTF8.GetString(output, 0, 64);
    }

    private bool IsValidHex(string str)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(str, "^[0-9a-fA-F]{56}$");
    }
}