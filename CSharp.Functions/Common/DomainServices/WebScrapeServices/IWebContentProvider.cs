using HtmlAgilityPack;

namespace Common
{
    public interface IWebContentProvider
    {
        Task<HtmlDocument> GetWebContent(string url);
    }
}