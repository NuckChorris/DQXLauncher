using DQXLauncher.Core.Utils.WebClient;
using HtmlAgilityPack;

namespace DQXLauncher.Core.Game.LoginStrategy;

public record LoginResponse
{
    public required HttpResponseMessage Response { get; init; }
    public required HtmlDocument Document { get; init; }

    // ReSharper disable once ReturnTypeCanBeNotNullable
    public HtmlNode? SqexAuth => Document.DocumentNode.SelectSingleNode("//x-sqexauth");

    // HtmlAgilityPack lies about the nullability of its properties, these are *very* nullable.
    // ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    public string? ErrorMessage => SqexAuth?.Attributes["message"]?.Value;
    public string? SessionId => SqexAuth?.Attributes["sid"]?.Value;
    public string? Lang => SqexAuth?.Attributes["lang"]?.Value;
    public string? Region => SqexAuth?.Attributes["region"]?.Value;
    public string? Utc => SqexAuth?.Attributes["utc"]?.Value;
    public string? Mode => SqexAuth?.Attributes["mode"]?.Value;

    public string? Token => SqexAuth?.Attributes["id"]?.Value;
    // ReSharper restore ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

    public WebForm? Form
    {
        get
        {
            var form = Document.DocumentNode.SelectSingleNode("//form[@name='mainForm']");
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (form is null) return null;
            return WebForm.FromHtmlForm(form, Response.RequestMessage!.RequestUri!);
        }
    }

    public static async Task<LoginResponse> FromHttpResponse(HttpResponseMessage response)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(await response.Content.ReadAsStringAsync());

        return new LoginResponse
        {
            Document = doc,
            Response = response
        };
    }
}