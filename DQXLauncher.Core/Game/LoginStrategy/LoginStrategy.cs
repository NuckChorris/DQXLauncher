using DQXLauncher.Core.Utils;
using HtmlAgilityPack;

namespace DQXLauncher.Core.Game.LoginStrategy;

public abstract class LoginStrategy
{
    protected string LOGIN_URL = "https://dqx-login.square-enix.com/oauth/sp/sso/dqxwin/login?client_id=dqx_win&redirect_uri=https%3a%2f%2fdqx%2dlogin%2esquare%2denix%2ecom%2f&response_type=code";
    protected async Task<HtmlDocument> GetLoginDocument(Dictionary<string, string> payload)
    {
        var httpClient = await GetHttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, LOGIN_URL);

        request.Content = new FormUrlEncodedContent(payload);
        
        var doc = new HtmlDocument();
        var response = await httpClient.SendAsync(request);
        doc.LoadHtml(await response.Content.ReadAsStringAsync());

        // TODO: we should check that the URL didn't redirect and pass it out to use for absolutization
        return doc;
    }
    
    protected Task<HttpClient> GetHttpClient()
    {
        return WebClient.GetHttpClient($"oauth-{this.GetType().Name}");
    }
}