using Common.Infra;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Entities;

namespace BackJobs.Infra
{
    public class SaveFieldsRepository : RepositoryBase, ISaveFieldsRepository
    {
        private const string SaveFieldsContainer = "SaveFieldsContainer";

        private readonly Container _container;

        public SaveFieldsRepository() : base(SaveFieldsContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<bool> GetSaveFieldsByIdAsync(string id)
        {
            var query = _container.GetItemQueryIterator<EditDocumentPayload>(
                new QueryDefinition($"SELECT * FROM c WHERE c.id = '{id}'"));
            var hasResults = false;

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                hasResults = response.Any();
            }

            return hasResults;
        }

        public async Task<EditDocumentPayload> SaveFieldsAsync(EditDocumentPayload documentPayload)
        {
            if (string.IsNullOrEmpty(documentPayload.Id))
            {
                throw new NullReferenceException(
                    $"The Id should have value in {nameof(SaveFieldsRepository)}.{nameof(SaveFieldsAsync)}");
            }

            var schemaModelResponse =
                await _container.UpsertItemAsync(documentPayload, new PartitionKey(documentPayload.Id));
            return schemaModelResponse.Resource;
        }
    }
}