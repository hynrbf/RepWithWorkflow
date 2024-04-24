using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class AffiliatesRepository : RepositoryBase, IAffiliatesRepository
    {
        private const string AffiliatesContainer = "AffiliatesContainer";

        private readonly Container _container;

        public AffiliatesRepository() : base(AffiliatesContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<Affiliate?> GetAffiliateByEmailAsync(string email)
        {
            var query = _container.GetItemQueryIterator<Affiliate>(
                new QueryDefinition($"SELECT * FROM c WHERE c.Email = '{email}'"));
            var results = new List<Affiliate>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<IEnumerable<Affiliate>> GetAffiliatesByCustomerIdAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new NullReferenceException(
                    $"The customer id should have a value in {nameof(AffiliatesRepository)}.{nameof(GetAffiliatesByCustomerIdAsync)}");
            }

            var queryText = $"SELECT * FROM c WHERE c.CustomerId = '{customerId}'";
            var query = _container.GetItemQueryIterator<Affiliate>(new QueryDefinition(queryText));
            var results = new List<Affiliate>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Affiliate> SaveOrUpdateAffiliatesAsync(Affiliate affiliate)
        {
            if (affiliate?.Details == null)
            {
                throw new NullReferenceException(
                    $"The Affiliate should not be null in {nameof(AffiliatesRepository)}.{nameof(SaveOrUpdateAffiliatesAsync)}");
            }

            if (string.IsNullOrEmpty(affiliate.Details.CompanyNumber))
            {
                throw new NullReferenceException(
                    $"The company no should have a value in {nameof(AffiliatesRepository)}.{nameof(SaveOrUpdateAffiliatesAsync)}");
            }

            if (string.IsNullOrEmpty(affiliate.Details.EmailAddress))
            {
                throw new NullReferenceException(
                    $"The email should have value in {nameof(AffiliatesRepository)}.{nameof(SaveOrUpdateAffiliatesAsync)}");
            }

            var existingAffiliate = await GetAffiliateByEmailAsync(affiliate.Details.EmailAddress);

            if (existingAffiliate != null)
            {
                affiliate.Id = existingAffiliate.Id;
                affiliate.TempPassword = existingAffiliate.TempPassword;
            }
            else
            {
                affiliate.TempPassword = PasswordGenerator.Generate(8, 1);
            }

            //This is a dirty hack because the front end is sending underscrore and component is hard to change
            if (!string.IsNullOrEmpty(affiliate.Details.ContactNumber?.Number))
            {
                affiliate.Details.ContactNumber.Number = affiliate.Details.ContactNumber.Number.Replace("_", "");
            }

            var schemaModelResponse =
                await _container.UpsertItemAsync(affiliate, new PartitionKey(affiliate.Id));
            return schemaModelResponse.Resource;
        }

        public async Task<bool> DeleteAffiliateAsync(string id)
        {
            var existingRecord = await GetAffiliateAsync(id);

            if (existingRecord == null)
            {
                return false;
            }

            await _container.DeleteItemAsync<Affiliate>(existingRecord.Id, new PartitionKey(existingRecord.Id));
            return true;
        }

        public async Task<IEnumerable<Affiliate>> GetAllAffiliatesNotYetFinishedSignupAsync()
        {
            var query = _container.GetItemQueryIterator<Affiliate>(
                new QueryDefinition($"SELECT * FROM c WHERE c.IsFinishedSignUp = false"));
            var results = new List<Affiliate>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        private async Task<Affiliate?> GetAffiliateAsync(string id)
        {
            var query = _container.GetItemQueryIterator<Affiliate>(
                new QueryDefinition($"SELECT * FROM c WHERE c.Id = '{id}'"));
            var results = new List<Affiliate>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }
    }
}