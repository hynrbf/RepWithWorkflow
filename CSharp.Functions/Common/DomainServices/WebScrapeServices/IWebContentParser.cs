using Common.Entities;

namespace Common
{
    public interface IWebContentParser
    {
        StructuredContent GetStructuredWebContent(WebScrapeRun webScrapeRun, List<string> namesForWarning);
    }
}