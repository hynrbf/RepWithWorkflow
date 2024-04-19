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
    public class SchemaAnswerRepository : RepositoryBase, ISchemaAnswerRepository
    {
        private const string SchemaAnswersContainer = "SchemaAnswersContainer";
        private readonly Container _container;

        public SchemaAnswerRepository() : base(SchemaAnswersContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<bool> InitializeSchemaAnswersAsync()
        {
            // check if there is existing data. If so, do nothing
            var existingSchemaAnswers = await GetSchemaAnswersAsync();

            if (existingSchemaAnswers.Any())
            {
                return false;
            }

            var schemaAnswers = SchemaAnswerData.GetInitialSchemaAnswersData();

            foreach (var answer in schemaAnswers)
            {
                await AddOrUpdateSchemaAnswerAsync(answer);
            }

            return true;
        }

        public async Task<List<SchemaAnswer>> GetSchemaAnswersAsync()
        {
            var query = _container.GetItemQueryIterator<SchemaAnswer>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<SchemaAnswer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<SchemaAnswer> AddOrUpdateSchemaAnswerAsync(SchemaAnswer schemaAnswer)
        {
            var schemaModelResponse =
                await _container.UpsertItemAsync(schemaAnswer, new PartitionKey(schemaAnswer.Id));
            return schemaModelResponse.Resource;
        }
        private async Task<SchemaAnswer> GetSchemaAnswersByIdAsync(string id)
        {
            var query = _container.GetItemQueryIterator<SchemaAnswer>(
                new QueryDefinition($"SELECT * FROM c WHERE c.id = '{id}'"));
            var results = new List<SchemaAnswer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }
        
        public async Task<bool> DeleteSchemaAnswerByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new NullReferenceException(
                    $"The id should have value in {nameof(SchemaAnswerRepository)}.{nameof(DeleteSchemaAnswerByIdAsync)}");
            }

            var existingRecord = await GetSchemaAnswersByIdAsync(id);

            if (existingRecord == null)
            {
                return false;
            }

            var itemResponse = await _container.DeleteItemAsync<SchemaAnswer>(id, new PartitionKey(id));
            return itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
    }
}