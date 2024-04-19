using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class IntroducersRepository : RepositoryBase, IIntroducersRepository
    {
        private const string IntroducersContainer = "IntroducersContainer";

        private readonly Container _container;

        public IntroducersRepository() : base(IntroducersContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<Introducer?> GetIntroducerByEmailAsync(string email)
        {
            var query = _container.GetItemQueryIterator<Introducer>(
                new QueryDefinition($"SELECT * FROM c WHERE c.Email = '{email}'"));
            var results = new List<Introducer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<IEnumerable<Introducer>> GetIntroducersByCustomerIdAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new NullReferenceException(
                    $"The customer id should have a value in {nameof(IntroducersRepository)}.{nameof(GetIntroducersByCustomerIdAsync)}");
            }

            var queryText = $"SELECT * FROM c WHERE c.CustomerId = '{customerId}'";
            var query = _container.GetItemQueryIterator<Introducer>(new QueryDefinition(queryText));
            var results = new List<Introducer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Introducer> SaveOrUpdateIntroducersAsync(Introducer introducer)
        {
            if (introducer?.Details == null)
            {
                throw new NullReferenceException(
                    $"The introducer should not be null in {nameof(IntroducersRepository)}.{nameof(SaveOrUpdateIntroducersAsync)}");
            }

            if (string.IsNullOrEmpty(introducer.Details.CompanyNumber) &&
                string.IsNullOrEmpty(introducer.Details.FcaFirmRefNo))
            {
                throw new NullReferenceException(
                    $"The company no or fca firm reference no should have a value in {nameof(IntroducersRepository)}.{nameof(SaveOrUpdateIntroducersAsync)}");
            }

            if (string.IsNullOrEmpty(introducer.Details.EmailAddress))
            {
                throw new NullReferenceException(
                    $"The email should have value in {nameof(IntroducersRepository)}.{nameof(SaveOrUpdateIntroducersAsync)}");
            }

            var existingIntroducer = await GetIntroducerByEmailAsync(introducer.Details.EmailAddress);

            if (existingIntroducer != null)
            {
                introducer.Id = existingIntroducer.Id;
                introducer.TempPassword = existingIntroducer.TempPassword;
            }
            else
            {
                introducer.TempPassword = PasswordGenerator.Generate(8, 1);
            }

            //hack to avoid needing to add First name and Last name
            if (introducer.Details.IsCompany)
            {
                introducer.FirstName = "Company";
                introducer.LastName = "User";
            }

            //This is a dirty hack because the front end is sending underscrore and component is hard to change
            if (!string.IsNullOrEmpty(introducer.Details.ContactNumber?.Number))
            {
                introducer.Details.ContactNumber.Number = introducer.Details.ContactNumber.Number.Replace("_", "");
            }

            var schemaModelResponse =
                await _container.UpsertItemAsync(introducer, new PartitionKey(introducer.Id));
            return schemaModelResponse.Resource;
        }

        public async Task<bool> DeleteIntroducerAsync(string id)
        {
            var existingRecord = await GetIntroducerAsync(id);

            if (existingRecord == null)
            {
                return false;
            }

            await _container.DeleteItemAsync<Introducer>(existingRecord.Id, new PartitionKey(existingRecord.Id));
            return true;
        }

        public async Task<IEnumerable<Introducer>> GetAllIntroducersNotYetFinishedSignupAsync()
        {
            var query = _container.GetItemQueryIterator<Introducer>(
                new QueryDefinition($"SELECT * FROM c WHERE c.IsFinishedSignUp = false"));
            var results = new List<Introducer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        private async Task<Introducer?> GetIntroducerAsync(string id)
        {
            var query = _container.GetItemQueryIterator<Introducer>(
                new QueryDefinition($"SELECT * FROM c WHERE c.Id = '{id}'"));
            var results = new List<Introducer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }
    }
}