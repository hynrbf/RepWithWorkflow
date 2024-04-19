using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class ProductMappingRepository : RepositoryBase, IProductMappingRepository
    {
        private const string ProductMappingsContainer = "ProductMappingsContainer";

        private readonly Container _container;

        public ProductMappingRepository() : base(ProductMappingsContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<IEnumerable<ProductMapping>> GetProductMappingsAsync()
        {
            var query = _container.GetItemQueryIterator<ProductMapping>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<ProductMapping>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<ProductMapping> SaveOrUpdateProductMappingAsync(ProductMapping productMapping)
        {
            var response =
                await _container.UpsertItemAsync(productMapping, new PartitionKey(productMapping.Id));
            return response.Resource;
        }
    }
}