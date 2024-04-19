using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Entities;
using Common.Infra;
using Microsoft.Azure.Cosmos;

namespace Api.Infra
{
    [Obsolete]
    public class SchemaRepository : RepositoryBase, ISchemaRepository
    {
        private const string SchemaContainer = "SchemaContainer";
        private readonly Container _container;

        public SchemaRepository() : base(SchemaContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<bool> InitializeSchemasAsync()
        {
            // check if there is existing data. If so, do nothing
            var existingSchemas = await GetAllSchemaAsync();

            if (existingSchemas.Any())
            {
                return false;
            }

            var schemas = SchemaData.GetInitialSchemaData();

            foreach (var schema in schemas)
            {
                await SaveSchemaAsync(schema);
            }

            return true;
        }

        public async Task<List<SchemaModel>> GetAllSchemaAsync()
        {
            var query = _container.GetItemQueryIterator<SchemaModel>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<SchemaModel>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<SchemaModel> GetSchemaAsync(string formNameKey)
        {
            var query = _container.GetItemQueryIterator<SchemaModel>(
                new QueryDefinition($"SELECT * FROM c WHERE c.FormNameKey = '{formNameKey}'"));
            var results = new List<SchemaModel>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<SchemaModel> AddOrUpdateSchemaAsync(SchemaModel schemaModel)
        {
            var schemaModelResponse =
                await _container.UpsertItemAsync(schemaModel, new PartitionKey(schemaModel.Id));
            return schemaModelResponse.Resource;
        }

        private async Task<SchemaModel> SaveSchemaAsync(SchemaModel schemaModel)
        {
            var schemaModelResponse =
                await _container.CreateItemAsync(schemaModel, new PartitionKey(schemaModel.Id));
            return schemaModelResponse.Resource;
        }
    }
}