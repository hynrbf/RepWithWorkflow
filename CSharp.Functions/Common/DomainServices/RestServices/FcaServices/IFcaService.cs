using Common.Entities;

namespace Common
{
    public interface IFcaService
    {
        Task<IEnumerable<Company>> SearchMatchedFirmsAsync(string companyName, bool isFirm,
            string companyNo = "", string companyAddress = "");

        Task<IEnumerable<Company>> SearchFirmsByFirmNameKeywordAsync(string keyword);

        Task<FcaPermissionsResult> SearchFirmPermissionsAsync(string firmRefNo);

        Task<string> SearchFirmClientMoneyPermissionAsync(string firmRefNo);
        Task<FcaFirmDetail> SearchFcaFirmDetailsAsync(string firmRefNo);
        Task<IEnumerable<string?>> GetTradingNamesAsync(string firmRefNo);
        Task<IEnumerable<FcaAddressDetails?>> GetFirmAddressDetailsAsync(string firmRefNo, string addressType = "");
        Task<Company?> GetMatchedCompanyAsync(string companyName, string companyNumber);
        Task<IEnumerable<FcaIndividual>> GetFirmIndividualsAsync(string firmRefNo);
        Task<IEnumerable<FcaRegulator>> GetFirmRegulatorsAsync(string firmRefNo);
        Task<IEnumerable<FcaAppointedRepresentative>> GetAppointedRepresentativesAsync(string firmRefNo, string type);
        Task<FcaRolesResult> GetIndividualControlledFunctionsAsync(string individualRefNo);
        Task<List<FirmPermission>> GetFirmPermissionsAsync(string firmRefNo);
    }
}