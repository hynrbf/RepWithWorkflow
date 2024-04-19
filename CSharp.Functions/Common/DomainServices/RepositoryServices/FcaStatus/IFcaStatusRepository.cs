using Common.Entities;

namespace Common
{
    public interface IFcaStatusRepository
    {
        Task<bool> InitializeFcaStatusesAsync();
        Task<List<FcaStatus>> GetAllFcaStatusesAsync();
        Task<FcaStatus> GetStatusByIdAsync(string id);
        Task<List<FcaStatus>> GetFcaStatusesByGeneralStatusAsync(string generalStatus);
        Task<FcaStatus> GetStatusByActualStatusAsync(string actualStatus);
        Task<FcaStatus> AddOrUpdateStatusAsync(FcaStatus fcaStatus);
        Task<bool> DeleteStatusAsync(string actualStatus);
        Task<bool> DeleteAllStatusesAsync();
    }
}
