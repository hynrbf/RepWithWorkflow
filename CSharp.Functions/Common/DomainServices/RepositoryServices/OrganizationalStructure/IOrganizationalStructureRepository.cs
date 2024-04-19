using Common.Entities;

namespace Common
{
    public interface IOrganizationalStructureRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<IEnumerable<Employee>> GetEmployeesAsync(string companyNumber);
        Task<IEnumerable<Employee>> GetAllEmployeesNotYetFinishedSignupAsync();
        Task<Employee?> GetEmployeeByEmailAsync(string email);
        Task<bool> DeleteEmployeeAsync(string companyNo);
        Task<Employee> SaveOrUpdateEmployeeAsync(Employee employee);
        Task SaveBulkEmployeesAsync(List<Employee> employees);
    }
}