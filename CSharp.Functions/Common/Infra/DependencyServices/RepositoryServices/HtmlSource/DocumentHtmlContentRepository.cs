using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class DocumentHtmlContentRepository : RepositoryBase, IHtmlRepository
    {
        private const string DocumentHtmlContentContainer = "DocumentHtmlContentContainer";
        private readonly Container _container;

        public DocumentHtmlContentRepository() : base(DocumentHtmlContentContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<List<HtmlContent>> GetHtmlSourcesAsync()
        {
            var query = _container.GetItemQueryIterator<HtmlContent>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<HtmlContent>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<HtmlContent> GetHtmlSourceAsync(string name, string version, string consultant)
        {
            var query = _container.GetItemQueryIterator<HtmlContent>(new QueryDefinition(
                $"SELECT * FROM c WHERE c.Name = '{name}' AND c.Version = '{version}' AND c.Consultant = '{consultant}'"));
            var results = new List<HtmlContent>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results?.FirstOrDefault() ?? new HtmlContent();
        }

        public async Task<HtmlContent> GetHtmlSourcesAsync(string name)
        {
            var query = _container.GetItemQueryIterator<HtmlContent>(
                new QueryDefinition($"SELECT * FROM c where c.Name='{name}'"));
            var results = new List<HtmlContent>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<HtmlContent> UpdateHtmlSourceAsync(HtmlContent htmlContent)
        {
            var schemaModelResponse =
                await _container.UpsertItemAsync(htmlContent, new PartitionKey(htmlContent.Id));
            return schemaModelResponse.Resource;
        }
    }
}