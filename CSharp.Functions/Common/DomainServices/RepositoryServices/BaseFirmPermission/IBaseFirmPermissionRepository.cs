using Common.Entities;

namespace Common
{
    public interface IBaseFirmPermissionRepository
    {
        Task<IEnumerable<BaseFirmPermission>> GetBaseFirmPermissionsAsync();
        Task<BaseFirmPermission> SaveOrUpdateBaseFirmPermissionAsync(BaseFirmPermission baseFirmPermission);
    }
}