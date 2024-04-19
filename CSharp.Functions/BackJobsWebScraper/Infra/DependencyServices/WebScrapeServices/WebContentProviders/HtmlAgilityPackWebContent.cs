using System.Threading.Tasks;
using Common;
using HtmlAgilityPack;

namespace BackJobsWebScraper.Infra
{
    public class HtmlAgilityPackWebContent : IWebContentProvider
    {
        public async Task<HtmlDocument> GetWebContent(string url)
        {
            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);

            return await Task.FromResult(htmlDoc);
        }
    }
}