using Common.Entities;

namespace Common
{
    public interface IAffiliatesRepository
    {
        Task<IEnumerable<Affiliate>> GetAllAffiliatesNotYetFinishedSignupAsync();
        Task<Affiliate?> GetAffiliateByEmailAsync(string email);
        Task<IEnumerable<Affiliate>> GetAffiliatesByCustomerIdAsync(string customerId);
        Task<bool> DeleteAffiliateAsync(string id);
        Task<Affiliate> SaveOrUpdateAffiliatesAsync(Affiliate affiliate);
    }
}