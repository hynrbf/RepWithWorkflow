using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class AppointedRepresentativeRepository : RepositoryBase, IAppointedRepresentativeRepository
    {
        private const string AppointedRepresentativesContainer = "AppointedRepresentativesContainer";

        public AppointedRepresentativeRepository() : base(AppointedRepresentativesContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        private readonly Container _container;

        public async Task<IEnumerable<AppointedRepresentative>> GetAllAppointedRepresentativesAsync()
        {
            const string queryText = $"SELECT * FROM c";
            var query = _container.GetItemQueryIterator<AppointedRepresentative>(new QueryDefinition(queryText));
            var results = new List<AppointedRepresentative>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<AppointedRepresentative>> GetAppointedRepresentativesAsync()
        {
            const string queryText = "SELECT * FROM c WHERE c.IsFinishedSignUp = false";
            var query = _container.GetItemQueryIterator<AppointedRepresentative>(new QueryDefinition(queryText));
            var results = new List<AppointedRepresentative>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<AppointedRepresentative>> GetAppointedRepresentativesAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new NullReferenceException(
                    $"The customer id should have a value in {nameof(AppointedRepresentativeRepository)}.{nameof(GetAppointedRepresentativesAsync)}");
            }

            var queryText = $"SELECT * FROM c WHERE c.CustomerId = '{customerId}'";
            var query = _container.GetItemQueryIterator<AppointedRepresentative>(new QueryDefinition(queryText));
            var results = new List<AppointedRepresentative>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<AppointedRepresentative>> GetAppointedRepresentativesByFirmRefNoAsync(
            string firmRefNo)
        {
            if (string.IsNullOrEmpty(firmRefNo))
            {
                throw new NullReferenceException(
                    $"The customer ref no should have a value in {nameof(AppointedRepresentativeRepository)}.{nameof(GetAppointedRepresentativesByFirmRefNoAsync)}");
            }

            var queryText = $"SELECT * FROM c WHERE c.FirmReferenceNumber = '{firmRefNo}'";
            var query = _container.GetItemQueryIterator<AppointedRepresentative>(new QueryDefinition(queryText));
            var results = new List<AppointedRepresentative>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<AppointedRepresentative?> GetAppointedRepresentativeByIdAsync(string id)
        {
            var query = _container.GetItemQueryIterator<AppointedRepresentative>(
                new QueryDefinition($"SELECT * FROM c WHERE c.id = '{id}'"));
            var results = new List<AppointedRepresentative>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<AppointedRepresentative?> GetAppointedRepresentativeByEmailAsync(string email)
        {
            var query = _container.GetItemQueryIterator<AppointedRepresentative>(
                new QueryDefinition($"SELECT * FROM c WHERE c.Email = '{email}'"));
            var results = new List<AppointedRepresentative>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<AppointedRepresentative> SaveOrUpdateAppointedRepresentativeAsync(
            AppointedRepresentative appointedRepresentative)
        {
            //Note. We have an AR signup which we don't have way to associate customer id
            //so we wont need this checker
            //if (string.IsNullOrEmpty(appointedRepresentative.CustomerId))
            //{
            //    throw new NullReferenceException(
            //        $"The customer id should have a value in {nameof(AppointedRepresentativeRepository)}.{nameof(SaveOrUpdateAppointedRepresentativeAsync)}");
            //}

            if (appointedRepresentative.ProfileStatus.Equals(ProfileStatuses.Full.ToString()))
            {
                if (string.IsNullOrEmpty(appointedRepresentative.Email))
                {
                    throw new NullReferenceException(
                        $"The email address should have a value in {nameof(AppointedRepresentativeRepository)}.{nameof(SaveOrUpdateAppointedRepresentativeAsync)}");
                }

                if (string.IsNullOrEmpty(appointedRepresentative.TempPassword))
                {
                    appointedRepresentative.TempPassword = PasswordGenerator.Generate(8, 1);
                }
            }

            var schemaModelResponse =
                await _container.UpsertItemAsync(appointedRepresentative, new PartitionKey(appointedRepresentative.Id));
            return schemaModelResponse.Resource;
        }

        public async Task<IEnumerable<AppointedRepresentative>> GetAppointedRepresentativesForDataInitAsync()
        {
            var query = _container.GetItemQueryIterator<AppointedRepresentative>(new QueryDefinition(
                "SELECT * FROM c WHERE c.IsFinishedSignUp = true AND c.ProfileStatus = 'Full' AND IS_NULL(c.FirmDetail) AND c.IsInProgressDataInitializing = false"));
            var results = new List<AppointedRepresentative>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public Task SaveBulkAppointedRepresentativesAsync(List<AppointedRepresentative> appointedRepresentatives)
        {
            var savingTasks = appointedRepresentatives.Select(appointedRepresentative =>
                    _container.UpsertItemAsync(appointedRepresentative, new PartitionKey(appointedRepresentative.Id)))
                .Cast<Task>().ToList();
            return Task.WhenAll(savingTasks);
        }

        public async Task<bool> DeleteAppointedRepresentativeByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new NullReferenceException(
                    $"The id should have value in {nameof(AppointedRepresentativeRepository)}.{nameof(DeleteAppointedRepresentativeByIdAsync)}");
            }

            var existingRecord = await GetAppointedRepresentativeByIdAsync(id);
            if (existingRecord == null)
            {
                return false;
            }

            var itemResponse = await _container.DeleteItemAsync<AppointedRepresentative>(id, new PartitionKey(id));
            return itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<bool> DeleteAllAppointedRepresentativesAsync(string firmRefNo)
        {
            var existingRecords = (await GetAppointedRepresentativesByFirmRefNoAsync(firmRefNo)).ToList();

            if (!existingRecords.Any())
            {
                return true;
            }

            var deleteTasks = existingRecords.Select(record =>
                _container.DeleteItemAsync<AppointedRepresentative>(record.Id, new PartitionKey(record.Id))
            ).ToList();
            await Task.WhenAll(deleteTasks);
            return true;
        }
    }
}