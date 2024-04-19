using Common.Entities;

namespace Common
{
    public interface IFcaPermissionsRepository
    {
        Task<bool> InitializePermissionsAsync();
        Task<IEnumerable<FcaPermission>> GetAllPermissionsAsync();
        Task<FcaPermission> GetPermissionById(string id);
        Task<IEnumerable<FcaPermission>> GetPermissionsGroupNameAsync(string groupName);
        Task<FcaPermission> AddOrUpdatePermissionAsync(FcaPermission permission);
        Task<IEnumerable<FcaPermission>> GetPermissionsByCategoryNameAsync(string categoryName);
        Task<bool> DeleteAllPermissionsAsync();
    }
}
