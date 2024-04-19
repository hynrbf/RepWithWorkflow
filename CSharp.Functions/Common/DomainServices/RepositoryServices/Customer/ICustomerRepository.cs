using Common.Entities;

namespace Common
{
    public interface ICustomerRepository
    {
        Task<Customer> SaveCustomerAsync(Customer customer);
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<IEnumerable<Customer>> GetCustomersForProposalEmailAsync();
        Task<IEnumerable<Customer>> GetCustomersNotSignedForProposalEmailAsync();
        Task<IEnumerable<Customer>> GetCustomersForDirectDebitEmailAsync();
        Task<IEnumerable<Customer>> GetCustomersForDirectDebitFollowupAsync();
        Task<IEnumerable<Customer>> GetCustomersNotSignedDirectDebitAsync();
        Task<IEnumerable<Customer>> GetCustomersForDataInitAsync();
        Task<IEnumerable<Customer>> GetColleaguesAsync(string email);
        Task<bool> DeleteCustomerByIdEmailAsync(string id, string email);
        Task<bool> CheckIfCompanyHasSignedProposalAlreadyAsync(string companyNumber);

        void Register(bool isDisabledCheckingForMultipleSignUpsForSingleCompany);
    }
}