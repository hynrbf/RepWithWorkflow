using Common.Infra;
using Common;
using Microsoft.Azure.Cosmos;
using Common.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Api.Infra
{
    public class CompanyHouseRepository : RepositoryBase, ICompanyHouseRepository
    {
        private const string CompanyHouseStatusesContainer = "CompanyHouseStatusesContainer";
        private readonly Container _container;

        public CompanyHouseRepository() : base(CompanyHouseStatusesContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<List<CompanyHouseStatus>> GetAllCompanyHouseStatusesAsync()
        {
            var queryStr = "SELECT * FROM c";
            var query = _container.GetItemQueryIterator<CompanyHouseStatus>(new QueryDefinition(queryStr));
            var results = new List<CompanyHouseStatus>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<CompanyHouseStatus> AddOrUpdateStatusAsync(CompanyHouseStatus companyHouseStatus)
        {
            var existing = await GetStatusByIdAsync(companyHouseStatus.Id);

            if (existing != null)
            {
                companyHouseStatus.Id = existing.Id;
            }

            if (string.IsNullOrEmpty(companyHouseStatus.Id))
            {
                companyHouseStatus.Id = Guid.NewGuid().ToString();
            }

            var statusResponse =
                await _container.UpsertItemAsync(companyHouseStatus, new PartitionKey(companyHouseStatus.Id));
            return statusResponse.Resource;
        }

        public async Task<CompanyHouseStatus> GetStatusByIdAsync(string id)
        {
            var queryStr = $"SELECT * FROM c WHERE c.id = '{id}'";
            var query = _container.GetItemQueryIterator<CompanyHouseStatus>(
                new QueryDefinition(queryStr));
            var results = new List<CompanyHouseStatus>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }
    }
}
