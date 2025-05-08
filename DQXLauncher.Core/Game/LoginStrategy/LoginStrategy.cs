﻿using DQXLauncher.Core.Utils.WebClient;
using HtmlAgilityPack;

namespace DQXLauncher.Core.Game.LoginStrategy;

internal interface ILoginStepHandler<in TAction> where TAction : LoginStepAction
{
    Task<LoginStep> Step(TAction action);
}

public abstract class LoginStrategy
{
    private static readonly string LoginUrl =
        "https://dqx-login.square-enix.com/oauth/sp/sso/dqxwin/login?client_id=dqx_win&redirect_uri=https%3a%2f%2fdqx%2dlogin%2esquare%2denix%2ecom%2f&response_type=code";

    public abstract Task<LoginStep> Restart();

    public virtual Task<LoginStep> Step<TAction>(TAction action) where TAction : LoginStepAction
    {
        if (this is ILoginStepHandler<TAction> handler) return handler.Step(action);

        throw new InvalidOperationException($"Strategy {GetType()} cannot handle {typeof(TAction)}");
    }

    protected async Task<WebForm> GetLoginForm(Dictionary<string, string> payload)
    {
        var httpClient = await GetWebClient();
        var request = new HttpRequestMessage(HttpMethod.Post, LoginUrl);

        request.Content = new FormUrlEncodedContent(payload);

        var doc = new HtmlDocument();
        var response = await httpClient.SendAsync(request);
        doc.LoadHtml(await response.Content.ReadAsStringAsync());

        var form = doc.DocumentNode.SelectSingleNode("//form[@name='mainForm']");
        return WebForm.FromHtmlForm(form, request.RequestUri!);
    }

    protected Task<WebClient> GetWebClient()
    {
        return WebClient.Get($"oauth-{this.GetType().Name}");
    }
}