using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class WebScrapsRepository : RepositoryBase, IWebScrapsRepository
    {
        private const string WebScrapsContainer = "WebScrapsContainer";

        private readonly Container _container;

        public WebScrapsRepository() : base(WebScrapsContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<WebScrap> SaveWebScrapAsync(WebScrap webScrap)
        {
            var schemaModelResponse = await _container.UpsertItemAsync(webScrap, new PartitionKey(webScrap.Id));
            return schemaModelResponse.Resource;
        }

        public async Task<List<WebScrap>> GetAllWebScrapsAsync()
        {
            const string queryText =
                "SELECT c.id, c.ParentId, c.MediaType, c.Url, c.SubUrls, c.RootUrl, c.UpdatedDate FROM c";
            return await ExecuteQuery(queryText);
        }

        public async Task<List<WebScrap>> GetWebScrapsForScrapeAsync()
        {
            var results = new List<WebScrap>();

            const int intervalInHours = 7 * 24;
            const string dateTimePart = "hh"; // Hours

            var rootWebScrapQueryText =
                "SELECT TOP 1 c.id, c.ParentId, c.MediaType, c.Url, c.SubUrls, c.RootUrl, c.UpdatedDate, c.LastScrapeDate FROM c " +
                "WHERE IS_NULL(c.ParentId) AND (IS_NULL(c.UpdatedDate) " +
                $"OR DateTimeDiff('{dateTimePart}',c.UpdatedDate,GetCurrentDateTime()) >= {intervalInHours})";
            var rootWebScrap = (await ExecuteQuery(rootWebScrapQueryText)).FirstOrDefault();
            if (rootWebScrap == null)
            {
                return results;
            }

            results.Add(rootWebScrap);
            var childrenWebScrapQueryText =
                "SELECT c.id, c.ParentId, c.MediaType, c.Url, c.SubUrls, c.RootUrl, c.LastScrapeDate FROM c " +
                $"WHERE c.RootUrl = '{rootWebScrap.Url}'";
            var childrenWebScraps = await ExecuteQuery(childrenWebScrapQueryText);

            results.AddRange(childrenWebScraps);
            return results;
        }

        public async Task<List<WebScrap>> GetWebScrapsWithRootUrlAsync(string rootUrl)
        {
            var url1 = rootUrl.ToLower().TrimEnd('/');
            var url2 = $"{url1}/";
            return await ExecuteQuery($"SELECT c.id, c.Url FROM c WHERE LOWER(c.RootUrl) IN ('{url1}', '{url2}')");
        }

        public async Task<List<WebScrap>> GetChildWebScrapsAsync()
        {
            return await ExecuteQuery("SELECT * FROM c WHERE IS_NULL(c.ParentId) = false");
        }

        public async Task<WebScrap?> GetWebScrapByUrlAsync(string url)
        {
            var url1 = url.ToLower().TrimEnd('/'); //eg.https://www.suntech.gi
            var url2 = $"{url1}/"; //eg.https://www.suntech.gi/

            var queryText = $"SELECT * FROM c WHERE LOWER(c.Url) IN ('{url1}', '{url2}')";
            var results = await ExecuteQuery(queryText);

            return results.FirstOrDefault();
        }

        public async Task<WebScrap?> GetWebScrapByIdAsync(string id)
        {
            var results = await ExecuteQuery($"SELECT * FROM c WHERE c.id = '{id}'");
            return results.FirstOrDefault();
        }

        private async Task<List<WebScrap>> ExecuteQuery(string queryText)
        {
            var queryResults = new List<WebScrap>();
            var query = _container.GetItemQueryIterator<WebScrap>(new QueryDefinition(queryText));

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                queryResults.AddRange(response.ToList());
            }

            return queryResults;
        }

        public async Task<bool> DeleteAllWebScrapsAsync()
        {
            var webScraps = await GetAllWebScrapsAsync();

            var deleteTasks = webScraps.Select(DeleteWebScrapAsync).Cast<Task>().ToList();

            await Task.WhenAll(deleteTasks);
            return true;
        }

        private async Task<bool> DeleteWebScrapAsync(WebScrap webScrap)
        {
            var found = await GetWebScrapById(webScrap.Id);

            if (found == null)
            {
                return false;
            }

            var itemResponse = await _container.DeleteItemAsync<WebScrap>(found.Id, new PartitionKey(found.Id));
            return itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        private async Task<WebScrap?> GetWebScrapById(string id)
        {
            var queryStr = $"SELECT * FROM c WHERE c.id = '{id}'";
            var query = _container.GetItemQueryIterator<WebScrap>(new QueryDefinition(queryStr));
            var results = new List<WebScrap>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }
    }
}