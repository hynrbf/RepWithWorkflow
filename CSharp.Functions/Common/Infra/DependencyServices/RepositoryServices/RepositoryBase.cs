using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public abstract class RepositoryBase
    {
        private static CosmosClient _cosmosClient;
        private static Lazy<CosmosClient> _lazyCosmosClient;

        protected static CosmosClient Client => _lazyCosmosClient.Value;
        protected string DatabaseName { get; }
        protected string ContainerName { get; }

        protected RepositoryBase(string containerKey = null, string partitionKey = "/id")
        {
            DatabaseName = Environment.GetEnvironmentVariable("Database", EnvironmentVariableTarget.Process);
            ContainerName = Environment.GetEnvironmentVariable(containerKey ?? nameof(ContainerName),
                EnvironmentVariableTarget.Process);

            if (_lazyCosmosClient?.Value != null)
            {
                return;
            }

            var account = Environment.GetEnvironmentVariable("CosmosDbAccount", EnvironmentVariableTarget.Process);
            var key = Environment.GetEnvironmentVariable("CosmosDbKey", EnvironmentVariableTarget.Process);
            _lazyCosmosClient = new Lazy<CosmosClient>(GetCosmosClientInstance(account, key));

            //// Containers are already created in Terraform, so we dont need this one
            //// Wait to complete for now
            //CreateContainerIfNotExist().Wait();
        }

        protected static string GenerateGuid()
            => Guid.NewGuid().ToString();

        private static CosmosClient GetCosmosClientInstance(string account, string key)
        {
            // ref: https://github.com/MicrosoftDocs/azure-docs/issues/40649#issuecomment-552454495
            var options = new CosmosClientOptions { ConnectionMode = ConnectionMode.Gateway };

            //Very dangerous if will not do this!
            //make this single instance, because azure functions will consume a lot of connections to the azure and azure will block our
            //connection https://github.com/Azure/Azure-Functions/issues/1127#issuecomment-463739744    
            return _cosmosClient ??= new CosmosClient(account, key, options);
        }

        //private async Task CreateContainerIfNotExist()
        //{
        //    // see: https://github.com/Azure/azure-cosmos-dotnet-v3/issues/591
        //    var db = Client.GetDatabase(DatabaseName);
        //    _ = await db.CreateContainerIfNotExistsAsync(ContainerName, PartitionKey);
        //}
    }
}