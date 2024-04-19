using Common.Entities;

namespace Common
{
    public interface IProvidersRepository
    {
        Task<IEnumerable<Providers>> GetAllProvidersNotYetFinishedSignupAsync();
        Task<Providers?> GetProviderByEmailAsync(string email);
        Task<IEnumerable<Providers>> GetProvidersByCustomerIdAsync(string customerId);
        Task<bool> DeleteProviderAsync(string id);
        Task<Providers> SaveOrUpdateProvidersAsync(Providers provider);
    }
}