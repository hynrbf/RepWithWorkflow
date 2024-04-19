using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class CustomerReplicationRepository : RepositoryBase, ICustomerReplicationRepository
    {
        private const string CustomerLogHistoryReplicaContainer = "CustomerLogHistoryReplicaContainer";

        private readonly Container _container;

        public CustomerReplicationRepository() : base(CustomerLogHistoryReplicaContainer) =>
            _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<Customer> SaveCustomerReplicaAsync(Customer customer)
        {
            var schemaModelResponse =
                await _container.UpsertItemAsync(customer, new PartitionKey(customer.CompanyName));
            return schemaModelResponse.Resource;
        }

        public async Task<bool> DeleteAllCustomerReplicationAsync()
        {
            var existingRecords = (await GetCustomerReplicationAsync()).ToList();
            var deleteTasks = existingRecords.Select(record =>
                _container.DeleteItemAsync<Customer>(record.Id, new PartitionKey(record.CompanyName))).ToList();
            await Task.WhenAll(deleteTasks);
            return true;
        }

        private async Task<IEnumerable<Customer>> GetCustomerReplicationAsync()
        {
            const string queryText = "SELECT * FROM c";
            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(queryText));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }
    }

    public class CustomerFcaReplicationRepository : RepositoryBase, ICustomerFcaReplicationRepository
    {
        private const string CustomerFcaReplicaContainer = "CustomerFcaReplicaContainer";

        private readonly Container _container;

        public CustomerFcaReplicationRepository() : base(CustomerFcaReplicaContainer) =>
            _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<Customer> SaveCustomerFcaReplicaAsync(Customer customer)
        {
            var schemaModelResponse =
                await _container.UpsertItemAsync(customer, new PartitionKey(customer.CompanyName));
            return schemaModelResponse.Resource;
        }

        public async Task<bool> DeleteAllCustomerFcaReplicationAsync()
        {
            var existingRecords = (await GetCustomerFcaReplicationAsync()).ToList();
            var deleteTasks = existingRecords.Select(record =>
                _container.DeleteItemAsync<Customer>(record.Id, new PartitionKey(record.CompanyName))).ToList();
            await Task.WhenAll(deleteTasks);
            return true;
        }

        private async Task<IEnumerable<Customer>> GetCustomerFcaReplicationAsync()
        {
            const string queryText = "SELECT * FROM c";
            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(queryText));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}