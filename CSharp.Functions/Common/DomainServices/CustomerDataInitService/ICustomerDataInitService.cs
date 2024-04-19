using Common.Entities;

namespace Common
{
    public interface ICustomerDataInitService
    {
        Task<string> GetRegisteredAddressAsync(string companyNumber);
        Task<AddressDetails> GetTradingAddressAsync(string firmRefNo);
        Task<List<string>> GetTradingNamesAsync(string firmRefNo);
        Task<List<MediaMarketingOutlet>> GetMediaMarketingOutletsAsync(string customerId, string firmRefNo);
        Task<List<AppointedRepresentative>> GetAppointedRepresentativesAsync(string firmRefNo);
        Task<List<CorporateController>> GetCorporateControllersAsync(string companyNumber);
        Task<List<IndividualController>> GetIndividualControllersAsync(string companyNumber);
        Task<DataProtectionLicense> GetDataProtectionLicenseAsync(string companyName);
        Task<List<Employee>> ComputeEmployeeAsync(Employee firstEmployee, string firmRefNo, string companyNo);
        Task<List<Employee>> ComputeEmployeeAsync(string firmRefNo, string companyNo);
        Task<List<CustomerPermission>> GetFirmFcaPermissionsAsync(string firmRefNo);
        void Register(string azureStorageBaseUrl, string azureStorageConnectionString, string blobStorageContainerName);
    }
}