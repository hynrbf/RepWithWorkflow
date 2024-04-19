using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Entities;
using Common.Infra;
using Microsoft.Azure.Cosmos;

namespace Api.Infra
{
    public class UiSchemaRepository : RepositoryBase, IUiSchemaRepository
    {
        private const string UiSchemaContainer = "UiSchemaContainer";
        private readonly Container _container;

        public UiSchemaRepository() : base(UiSchemaContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<bool> InitializeUiSchemasAsync()
        {
            // check if there is existing data. If so, do nothing
            var existingUiSchemas = await GetAllUiSchemaAsync();

            if (existingUiSchemas.Any())
            {
                return false;
            }

            var uiSchemas = UiSchemaData.GetInitialUiSchemaData();

            foreach (var uiSchema in uiSchemas)
            {
                await SaveUiSchemaAsync(uiSchema);
            }

            return true;
        }

        public async Task<List<UiSchemaModel>> GetAllUiSchemaAsync()
        {
            var query = _container.GetItemQueryIterator<UiSchemaModel>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<UiSchemaModel>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<UiSchemaModel> GetUiSchemaAsync(string formNameKey)
        {
            var query = _container.GetItemQueryIterator<UiSchemaModel>(
                new QueryDefinition($"SELECT * FROM c WHERE c.FormNameKey = '{formNameKey}'"));
            var results = new List<UiSchemaModel>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<UiSchemaModel> AddOrUpdateUiSchemaAsync(UiSchemaModel uiSchemaModel)
        {
            var uiSchemaModelResponse =
                await _container.UpsertItemAsync(uiSchemaModel, new PartitionKey(uiSchemaModel.Id));
            return uiSchemaModelResponse.Resource;
        }

        private async Task<UiSchemaModel> SaveUiSchemaAsync(UiSchemaModel uiSchemaModel)
        {
            var schemaModelResponse =
                await _container.CreateItemAsync(uiSchemaModel, new PartitionKey(uiSchemaModel.Id));
            return schemaModelResponse.Resource;
        }
    }
}