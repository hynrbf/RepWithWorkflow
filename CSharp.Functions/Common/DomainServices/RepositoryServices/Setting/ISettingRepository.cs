using Common.Entities;

namespace Common
{
    public interface ISettingRepository
    {
        Task<bool> InitializeSettingsAsync();
        Task<Setting> AddOrUpdateSettingAsync(Setting setting);
        Task<IEnumerable<Setting>> GetAllSettingAsync();
        Task<Setting> GetSettingByIdAsync(string id);
        Task<Setting> GetSettingByKeyAsync(string key);
        Task<bool> DeleteSettingAsync(string settingId);
        Task<bool> DeleteAllSettingsAsync();
    }
}
