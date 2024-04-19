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
    public class FcaStatusRepository : RepositoryBase, IFcaStatusRepository
    {
        private const string FcaStatusesContainer = "FcaStatusesContainer";
        private readonly Container _container;

        public FcaStatusRepository() : base(FcaStatusesContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<bool> InitializeFcaStatusesAsync()
        {
            var existingStatuses = await GetAllFcaStatusesAsync();

            if (existingStatuses.Any())
            {
                return false;
            }

            foreach (var status in FcaStatusData.GetFcaStatusesData())
            {
                await AddOrUpdateStatusAsync(status);
            }

            return true;
        }

        public async Task<List<FcaStatus>> GetAllFcaStatusesAsync()
        {
            var queryStr = "SELECT * FROM c";
            var query = _container.GetItemQueryIterator<FcaStatus>(new QueryDefinition(queryStr));
            var results = new List<FcaStatus>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<FcaStatus> AddOrUpdateStatusAsync(FcaStatus fcaStatus)
        {
            var existing = await GetStatusByIdAsync(fcaStatus.Id);

            if (existing != null)
            {
                fcaStatus.Id = existing.Id;
            }

            if (string.IsNullOrEmpty(fcaStatus.Id))
            {
                fcaStatus.Id = Guid.NewGuid().ToString();
            }

            var statusResponse =
                await _container.UpsertItemAsync(fcaStatus, new PartitionKey(fcaStatus.Id));
            return statusResponse.Resource;
        }

        public async Task<FcaStatus> GetStatusByActualStatusAsync(string actualStatus)
        {
            var queryStr = $"SELECT * FROM c WHERE c.ActualStatus = '{actualStatus}'";
            var query = _container.GetItemQueryIterator<FcaStatus>(
                new QueryDefinition(queryStr));
            var results = new List<FcaStatus>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<List<FcaStatus>> GetFcaStatusesByGeneralStatusAsync(string generalStatus)
        {
            var queryStr = $"SELECT * FROM c WHERE c.GeneralStatus = '{generalStatus}'";
            var query = _container.GetItemQueryIterator<FcaStatus>(
                new QueryDefinition(queryStr));
            var results = new List<FcaStatus>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<bool> DeleteAllStatusesAsync()
        {
            var permissions = await GetAllFcaStatusesAsync();

            foreach (var permission in permissions)
            {
                await DeleteStatusAsync(permission.ActualStatus);
            }

            return true;
        }

        public async Task<bool> DeleteStatusAsync(string actualStatus)
        {
            if (string.IsNullOrEmpty(actualStatus))
            {
                return false;
            }

            var found = await GetStatusByActualStatusAsync(actualStatus);

            if (found == null)
            {
                return false;
            }

            var itemResponse =
                await _container.DeleteItemAsync<FcaStatus>(found.Id, new PartitionKey(found.Id));
            return itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<FcaStatus> GetStatusByIdAsync(string id)
        {
            var queryStr = $"SELECT * FROM c WHERE c.id = '{id}'";
            var query = _container.GetItemQueryIterator<FcaStatus>(
                new QueryDefinition(queryStr));
            var results = new List<FcaStatus>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }
    }
}