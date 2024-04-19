using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class CurrencyConversionRepository : RepositoryBase, ICurrencyConversionRepository
    {
        private const string CurrencyConversionsContainer = "CurrencyConversionsContainer";
        private readonly Container _container;

        public CurrencyConversionRepository() : base(CurrencyConversionsContainer) =>
            _container = Client.GetContainer(DatabaseName, ContainerName);

        public Task<CurrencyConversion> GetCurrencyConversionByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<CurrencyConversion?> GetCurrencyConversionByDateTime(string dateTime)
        {
            if (dateTime == null) throw new ArgumentNullException(nameof(dateTime));

            try
            {
                var queryStr = $"SELECT * FROM c WHERE c.DateTimestamp = '{dateTime}'";
                var query = _container.GetItemQueryIterator<CurrencyConversion>(new QueryDefinition(queryStr));
                var results = new List<CurrencyConversion>();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    results.AddRange(response.ToList());
                }

                return results.FirstOrDefault();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Cannot convert string to date. Error: {ex.Message}");
            }

            return null;
        }

        public async Task<CurrencyConversion> SaveCurrencyConversionAsync(CurrencyConversion currencyConversion)
        {
            var existing = await GetCurrencyConversionByDateTime(currencyConversion.DateTimestamp);
            
            if (existing != null)
            {
                currencyConversion.Id = existing.Id;
            }

            if (string.IsNullOrEmpty(currencyConversion.Id))
            {
                currencyConversion.Id = Guid.NewGuid().ToString();
            }

            var schemaModelResponse =
                await _container.UpsertItemAsync(currencyConversion, new PartitionKey(currencyConversion.Id));
            return schemaModelResponse.Resource;
        }
    }
}