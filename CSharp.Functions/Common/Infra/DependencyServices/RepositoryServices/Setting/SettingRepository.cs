using Common.Entities;
using Microsoft.Azure.Cosmos;

namespace Common.Infra
{
    public class SettingRepository : RepositoryBase, ISettingRepository
    {
        private const string SettingsContainer = "SettingsContainer";
        private readonly Container _container;

        public SettingRepository() : base(SettingsContainer)
            => _container = Client.GetContainer(DatabaseName, ContainerName);

        public async Task<bool> InitializeSettingsAsync()
        {
            var existingSetting = await GetAllSettingAsync();

            if (existingSetting.Any())
            {
                return false;
            }

            foreach (var setting in SettingData.GetSettingsData())
            {
                await AddOrUpdateSettingAsync(setting);
            }

            return true;
        }

        public async Task<Setting> AddOrUpdateSettingAsync(Setting setting)
        {
            if (string.IsNullOrEmpty(setting.Id))
            {
                setting.Id = Guid.NewGuid().ToString();
            }

            var settingResponse =
                await _container.UpsertItemAsync(setting, new PartitionKey(setting.Id));
            return settingResponse.Resource;
        }

        public async Task<IEnumerable<Setting>> GetAllSettingAsync()
        {
            var query = _container.GetItemQueryIterator<Setting>(
                new QueryDefinition("SELECT * FROM c"));
            var results = new List<Setting>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Setting> GetSettingByKeyAsync(string key)
        {
            var query = _container.GetItemQueryIterator<Setting>(
                new QueryDefinition($"SELECT * FROM c WHERE c.Key = '{key}'"));
            var results = new List<Setting>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<Setting> GetSettingByIdAsync(string id)
        {
            var query = _container.GetItemQueryIterator<Setting>(
                new QueryDefinition($"SELECT * FROM c WHERE c.id = '{id}'"));
            var results = new List<Setting>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.FirstOrDefault();
        }

        public async Task<bool> DeleteSettingAsync(string settingId)
        {
            if (string.IsNullOrEmpty(settingId))
            {
                return false;
            }

            var found = await GetSettingByIdAsync(settingId);

            if (found == null)
            {
                return false;
            }

            var itemResponse =
                await _container.DeleteItemAsync<Setting>(found.Id, new PartitionKey(found.Id));
            return itemResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<bool> DeleteAllSettingsAsync()
        {
            var settings = await GetAllSettingAsync();

            foreach (var setting in settings)
            {
                await DeleteSettingAsync(setting.Id);
            }

            return true;
        }
    }
}