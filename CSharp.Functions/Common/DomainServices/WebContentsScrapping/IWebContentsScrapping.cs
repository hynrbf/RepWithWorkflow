using Common.Entities;
using HtmlAgilityPack;

namespace Common
{
    public interface IWebContentsScrapping
    {
        IEnumerable<string> GetContentFromUrlByKeyword(string url, string keyword);
        IEnumerable<string> GetContentFromHtmlByKeyword(string htmlContent, string keyword);
        ScrappedWebContent GetScrappedWebContent(string url);
        HtmlNodeCollection? GetNodesFromHtmlByTag(string htmlContent, string tag, string keyword = "");
    }
}
