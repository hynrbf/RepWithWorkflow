using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class FcaPermissionsRepository : RepositoryBase, IFcaPermissionsRepository
    {
        private const string FcaPermissionsContainer = "FcaPermissionsContainer";
        private readonly Container _container;

        public FcaPermissionsRepository() : base(FcaPermissionsContainer) 
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<bool> InitializePermissionsAsync()
        {
            var existingPermission = await GetAllPermissionsAsync();

            if (existingPermission.Any())
            {
                return false;
            }

            foreach (var permission in FcaPermissionsData.GetFcaPermissionData())
            {
                await AddOrUpdatePermissionAsync(permission);
            }

            return true;
        }

        public async Task<FcaPermission> AddOrUpdatePermissionAsync(FcaPermission permission)
        {
            var existing = await GetPermissionById(permission.Id);

            if (existing != null)
            {
                permission.Id = existing.Id;
            }

            if (string.IsNullOrEmpty(permission.Id))
            {
                permission.Id = Guid.NewGuid().ToString();
            }

            var permissionResponse =
                await _container.UpsertItemAsync(permission, new PartitionKey(permission.Id));
            return permissionResponse.Resource;
        }

        public async Task<IEnumerable<FcaPermission>> GetAllPermissionsAsync()
        {
            var queryStr = "SELECT * FROM c";
            var query = _container.GetItemQueryIterator<FcaPermission>(new QueryDefinition(queryStr));
            var results = new List<FcaPermission>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<FcaPermission>> GetPermissionsByCategoryNameAsync(string categoryName)
        {
            var queryStr = $"SELECT * FROM c WHERE c.CategoryName = '{categoryName}'";
            var query = _container.GetItemQueryIterator<FcaPermission>(new QueryDefinition(queryStr));
            var results = new List<FcaPermission>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<FcaPermission>> GetPermissionsGroupNameAsync(string groupName)
        {
            var queryStr = $"SELECT * FROM c WHERE c.PermissionGroupName = '{groupName}'";
            var query = _container.GetItemQueryIterator<FcaPermission>(new QueryDefinition(queryStr));
            var results = new List<FcaPermission>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<bool> DeleteAllPermissionsAsync()
        {
            var permissions = await GetAllPermissionsAsync();

            foreach (var permission in permissions)
            {
                await DeletePermissionAsync(permission);
            }

            return true;
        }

        public async Task<FcaPermission> GetPermissionById(string id)
        {
            var queryStr = $"SELECT * FROM c WHERE c.id = '{id}'";
            var query = _container.GetItemQueryIterator<FcaPermission>(new QueryDefinition(queryStr));
            var results = new List<FcaPermission>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();

        }

        private async Task<bool> DeletePermissionAsync(FcaPermission permission)
        {
            var found = await GetPermissionById(permission.Id);

            if (found == null)
            {
                return false;
            }

            var itemResponse = await _container.DeleteItemAsync<FcaPermission>(found.Id, new PartitionKey(found.Id));
            return itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
    }
}
