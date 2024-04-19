using Common.Entities;

namespace Common
{
    public interface IAppointedRepresentativeRepository
    {
        Task<IEnumerable<AppointedRepresentative>> GetAllAppointedRepresentativesAsync();
        Task<IEnumerable<AppointedRepresentative>> GetAppointedRepresentativesAsync();
        Task<IEnumerable<AppointedRepresentative>> GetAppointedRepresentativesAsync(string customerId);
        Task<IEnumerable<AppointedRepresentative>> GetAppointedRepresentativesByFirmRefNoAsync(string firmRefNo);
        Task<AppointedRepresentative?> GetAppointedRepresentativeByIdAsync(string id);
        Task<AppointedRepresentative?> GetAppointedRepresentativeByEmailAsync(string email);

        Task<IEnumerable<AppointedRepresentative>> GetAppointedRepresentativesForDataInitAsync();

        Task<AppointedRepresentative> SaveOrUpdateAppointedRepresentativeAsync(
            AppointedRepresentative appointedRepresentative);

        Task SaveBulkAppointedRepresentativesAsync(List<AppointedRepresentative> appointedRepresentatives);

        Task<bool> DeleteAppointedRepresentativeByIdAsync(string id);
        Task<bool> DeleteAllAppointedRepresentativesAsync(string firmRefNo);
    }
}