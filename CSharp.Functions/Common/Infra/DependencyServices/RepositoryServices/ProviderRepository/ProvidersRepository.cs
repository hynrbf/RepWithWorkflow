using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class ProvidersRepository : RepositoryBase, IProvidersRepository
    {
        private const string ProvidersContainer = "ProvidersContainer";

        private readonly Container _container;

        public ProvidersRepository() : base(ProvidersContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<Providers?> GetProviderByEmailAsync(string email)
        {
            var query = _container.GetItemQueryIterator<Providers>(
                new QueryDefinition($"SELECT * FROM c WHERE c.Email = '{email}'"));
            var results = new List<Providers>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<IEnumerable<Providers>> GetProvidersByCustomerIdAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new NullReferenceException(
                    $"The customer id should have a value in {nameof(ProvidersRepository)}.{nameof(GetProvidersByCustomerIdAsync)}");
            }

            var queryText = $"SELECT * FROM c WHERE c.CustomerId = '{customerId}'";
            var query = _container.GetItemQueryIterator<Providers>(new QueryDefinition(queryText));
            var results = new List<Providers>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Providers> SaveOrUpdateProvidersAsync(Providers provider)
        {
            if (provider?.Details == null)
            {
                throw new NullReferenceException(
                    $"The provider should not be null in {nameof(ProvidersRepository)}.{nameof(SaveOrUpdateProvidersAsync)}");
            }

            if (string.IsNullOrEmpty(provider.Details.CompanyNumber))
            {
                throw new NullReferenceException(
                    $"The company no should have a value in {nameof(ProvidersRepository)}.{nameof(SaveOrUpdateProvidersAsync)}");
            }

            if (string.IsNullOrEmpty(provider.Details.EmailAddress))
            {
                throw new NullReferenceException(
                    $"The email should have value in {nameof(ProvidersRepository)}.{nameof(SaveOrUpdateProvidersAsync)}");
            }

            var existingProvider = await GetProviderByEmailAsync(provider.Details.EmailAddress);

            if (existingProvider != null)
            {
                provider.Id = existingProvider.Id;
                provider.TempPassword = existingProvider.TempPassword;
            }
            else
            {
                provider.TempPassword = PasswordGenerator.Generate(8, 1);
            }

            //hack to avoid needing to add First name and Last name
            if (string.IsNullOrEmpty(provider.FirstName))
            {
                provider.FirstName = "Company";
            }

            if (string.IsNullOrEmpty(provider.LastName))
            {
                provider.LastName = "User";
            }

            //This is a dirty hack because the front end is sending underscrore and component is hard to change
            if (!string.IsNullOrEmpty(provider.Details.ContactNumber?.Number))
            {
                provider.Details.ContactNumber.Number = provider.Details.ContactNumber.Number.Replace("_", "");
            }

            var schemaModelResponse =
                await _container.UpsertItemAsync(provider, new PartitionKey(provider.Id));
            return schemaModelResponse.Resource;
        }

        public async Task<bool> DeleteProviderAsync(string id)
        {
            var existingRecord = await GetProviderAsync(id);

            if (existingRecord == null)
            {
                return false;
            }

            await _container.DeleteItemAsync<Providers>(existingRecord.Id, new PartitionKey(existingRecord.Id));
            return true;
        }

        public async Task<IEnumerable<Providers>> GetAllProvidersNotYetFinishedSignupAsync()
        {
            var query = _container.GetItemQueryIterator<Providers>(
                new QueryDefinition($"SELECT * FROM c WHERE c.IsFinishedSignUp = false"));
            var results = new List<Providers>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        private async Task<Providers?> GetProviderAsync(string id)
        {
            var query = _container.GetItemQueryIterator<Providers>(
                new QueryDefinition($"SELECT * FROM c WHERE c.Id = '{id}'"));
            var results = new List<Providers>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }
    }
}