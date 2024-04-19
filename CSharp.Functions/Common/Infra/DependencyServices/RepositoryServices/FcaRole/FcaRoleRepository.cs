using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class FcaRoleRepository : RepositoryBase, IFcaRoleRepository
    {
        private const string FcaRolesContainer = "FcaRolesContainer";

        private readonly Container _container;

        public FcaRoleRepository() : base(FcaRolesContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<IEnumerable<FcaRole>> GetFcaRolesAsync()
        {
            var query = _container.GetItemQueryIterator<FcaRole>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<FcaRole>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<FcaRole> SaveOrUpdateFcaRoleAsync(FcaRole fcaRole)
        {
            var response =
                await _container.UpsertItemAsync(fcaRole, new PartitionKey(fcaRole.Id));
            return response.Resource;
        }
    }
}