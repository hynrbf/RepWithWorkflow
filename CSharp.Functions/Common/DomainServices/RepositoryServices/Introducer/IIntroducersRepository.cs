using Common.Entities;

namespace Common
{
    public interface IIntroducersRepository
    {
        Task<IEnumerable<Introducer>> GetAllIntroducersNotYetFinishedSignupAsync();
        Task<Introducer?> GetIntroducerByEmailAsync(string email);
        Task<IEnumerable<Introducer>> GetIntroducersByCustomerIdAsync(string customerId);
        Task<bool> DeleteIntroducerAsync(string id);
        Task<Introducer> SaveOrUpdateIntroducersAsync(Introducer introducer);
    }
}