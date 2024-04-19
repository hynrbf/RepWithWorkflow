using Common.Entities;

namespace Common
{
    public interface IFcaRoleRepository
    {
        Task<IEnumerable<FcaRole>> GetFcaRolesAsync();
        Task<FcaRole> SaveOrUpdateFcaRoleAsync(FcaRole fcaRole);
    }
}