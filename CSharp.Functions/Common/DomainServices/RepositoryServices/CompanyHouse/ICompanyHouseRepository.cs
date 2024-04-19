using Common.Entities;

namespace Common
{
    public interface ICompanyHouseRepository
    {
        Task<List<CompanyHouseStatus>> GetAllCompanyHouseStatusesAsync();

        Task<CompanyHouseStatus> AddOrUpdateStatusAsync(CompanyHouseStatus companyHouseStatus);

        Task<CompanyHouseStatus> GetStatusByIdAsync(string id);
    }
}
