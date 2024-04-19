using Common.Entities;

namespace Common
{
    public interface ICustomerReplicationRepository
    {
        Task<Customer> SaveCustomerReplicaAsync(Customer customer);
        Task<bool> DeleteAllCustomerReplicationAsync();
    }

    public interface ICustomerFcaReplicationRepository
    {
        Task<Customer> SaveCustomerFcaReplicaAsync(Customer customer);
        Task<bool> DeleteAllCustomerFcaReplicationAsync();
    }
}