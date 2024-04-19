using Common.Entities;

namespace Common
{
    public interface IInvitedUsersRepository
    {
        Task<bool> InviteUser(Customer customer);
    }
}
