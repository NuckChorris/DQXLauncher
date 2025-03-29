using HtmlAgilityPack;

namespace DQXLauncher.Core.Hiroba;

public class BannerImage
{
    public string Alt { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Src { get; set; } = string.Empty;

    public static async Task<List<BannerImage>> GetBanners()
    {
        var httpClient = await Utils.WebClient.GetHttpClient("banners");

        var doc = new HtmlDocument();
        var result = await httpClient.GetAsync("https://hiroba.dqx.jp/sc/rotationbanner");
        doc.LoadHtml(await result.Content.ReadAsStringAsync());

        return doc.DocumentNode.SelectNodes("//li[contains(@class, 'slide')]/a").Select(slide =>
        {
            var img = slide.SelectSingleNode(".//img");

            return new BannerImage
            {
                Alt = slide.GetAttributeValue("alt", string.Empty),
                Href = slide.GetAttributeValue("href", string.Empty),
                Src = img.GetAttributeValue("src", string.Empty)
            };
        }).ToList();
    }
}