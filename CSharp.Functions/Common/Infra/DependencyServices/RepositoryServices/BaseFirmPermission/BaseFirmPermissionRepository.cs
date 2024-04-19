using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class BaseFirmPermissionRepository : RepositoryBase, IBaseFirmPermissionRepository
    {
        private const string BaseFirmPermissionsContainer = "BaseFirmPermissionsContainer";

        private readonly Container _container;

        public BaseFirmPermissionRepository() : base(BaseFirmPermissionsContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<IEnumerable<BaseFirmPermission>> GetBaseFirmPermissionsAsync()
        {
            var query = _container.GetItemQueryIterator<BaseFirmPermission>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<BaseFirmPermission>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<BaseFirmPermission> SaveOrUpdateBaseFirmPermissionAsync(BaseFirmPermission baseFirmPermission)
        {
            var response =
                await _container.UpsertItemAsync(baseFirmPermission, new PartitionKey(baseFirmPermission.Id));
            return response.Resource;
        }
    }
}