using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class FinancialPromotionRepository : RepositoryBase, IFinancialPromotionRepository
    {
        private const string FinancialPromotionsContainer = "FinancialPromotionsContainer";

        public FinancialPromotionRepository() : base(FinancialPromotionsContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        private readonly Container _container;

        public async Task<IEnumerable<FinancialPromotion>> GetAllFinancialPromotionsAsync()
        {
            // Using * here is heavy and too slow (bec of c.Content, c.EditorContent). Use a separate FP retrieval when needs to save
            const string queryText =
                "SELECT c.id, c.CustomerId, c.MediaOutlet, c.ContentUrl, c.IsRootUrl, c.Owner, c.Moderator, c.Type, c.CreatedAt, c.LastScrapeDate " +
                "FROM c WHERE IS_DEFINED(c.ContentUrl) AND LENGTH(TRIM(c.ContentUrl)) > 0";
            var query = _container.GetItemQueryIterator<FinancialPromotion>(new QueryDefinition(queryText));
            var results = new List<FinancialPromotion>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<FinancialPromotion>> GetFinancialPromotionsAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new NullReferenceException(
                    $"The customer id should have a value in {nameof(FinancialPromotionRepository)}.{nameof(GetFinancialPromotionsAsync)}");
            }

            var queryText = $"SELECT * FROM c WHERE c.CustomerId = '{customerId}'";
            var query = _container.GetItemQueryIterator<FinancialPromotion>(new QueryDefinition(queryText));
            var results = new List<FinancialPromotion>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<FinancialPromotion?> GetFinancialPromotionByIdAsync(string id)
        {
            var query = _container.GetItemQueryIterator<FinancialPromotion>(
                new QueryDefinition($"SELECT * FROM c WHERE c.id = '{id}'"));
            var results = new List<FinancialPromotion>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<FinancialPromotion> SaveOrUpdateFinancialPromotionAsync(FinancialPromotion financialPromotion)
        {
            if (string.IsNullOrEmpty(financialPromotion.CustomerId))
            {
                throw new NullReferenceException(
                    $"The customer id should have a value in {nameof(FinancialPromotionRepository)}.{nameof(SaveOrUpdateFinancialPromotionAsync)}");
            }

            var schemaModelResponse =
                await _container.UpsertItemAsync(financialPromotion, new PartitionKey(financialPromotion.Id));
            return schemaModelResponse.Resource;
        }

        public async Task<bool> DeleteFinancialPromotionByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new NullReferenceException(
                    $"The id should have value in {nameof(FinancialPromotionRepository)}.{nameof(DeleteFinancialPromotionByIdAsync)}");
            }

            var existingRecord = await GetFinancialPromotionByIdAsync(id);
            if (existingRecord == null)
            {
                return false;
            }

            var itemResponse = await _container.DeleteItemAsync<FinancialPromotion>(id, new PartitionKey(id));
            return itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<bool> DeleteFinancialPromotionsAsync(string customerId)
        {
            var existingRecords = (await GetFinancialPromotionsAsync(customerId)).ToList();
            if (!existingRecords.Any())
            {
                return false;
            }

            var deleteTasks = existingRecords.Select(record =>
                    _container.DeleteItemAsync<FinancialPromotion>(record.Id, new PartitionKey(record.Id))).Cast<Task>()
                .ToList();
            await Task.WhenAll(deleteTasks);
            return true;
        }
    }
}