using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class EmailQueueRepository : RepositoryBase, IEmailQueueRepository
    {
        private const string EmailQueueContainer = "EmailQueueContainer";
        private readonly Container _container;

        public EmailQueueRepository() : base(EmailQueueContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<IEnumerable<EmailQueue>> GetAllEmailsInQueueAsync()
        {
            var query = _container.GetItemQueryIterator<EmailQueue>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<EmailQueue>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<bool> SaveEmailInQueueAsync(IEnumerable<EmailQueue> models)
        {
            foreach (var model in models)
            {
                var emailQueueResponse =
                await _container.UpsertItemAsync(model, new PartitionKey(model.Id));
            }

            return true;
        }
    }
}