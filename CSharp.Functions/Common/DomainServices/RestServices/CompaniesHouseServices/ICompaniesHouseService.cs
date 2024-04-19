using Common.Entities;

namespace Common
{
    public interface ICompaniesHouseService
    {
        Task<IEnumerable<Company>> SearchCompaniesWithDetailsAsync(string keyword, int itemsPerPage = AppConstants.DefaultItemsPerPageNoUsedInFcaOrCh);
        Task<IEnumerable<CompanyOfficer>> GetCompanyOfficersAsync(string companyNumber);
        Task<IEnumerable<CompanyOfficer>> GetCompanyActiveDirectorsAsync(string companyNumber);
        Task<IEnumerable<CompanyOfficerAppointmentDetails>> GetDirectorsAndAppointmentsAsync(string companyNumber);
        Task<IEnumerable<Controller>> GetControllingInterestAsync(string companyNumber);
        Task<List<Controller>> GetIndividualControllersAsync(string companyNumber);
        Task<List<Controller>> GetCorporateControllersRecursiveAsync(string companyNumber, int currentLevel = 1);
        Task<IEnumerable<CompanyFilingHistoryItem>> GetCompanyFilingHistoryAsync(string companyNumber);
        Task<CompanyFilingHistoryItem> GetCompanyFilingHistoryItemAsync(string companyNumber, string transactionId);
        Task<string> SaveCompanyFilingHistoryDocumentAsync(string documentId);
        Task<CompanyProfileK?> GetCompanyProfileAsync(string companyNumber);
    }
}