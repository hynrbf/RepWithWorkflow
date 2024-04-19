using Common.Entities;

namespace Common
{
    public interface IEmailQueueRepository
    {
        Task<IEnumerable<EmailQueue>> GetAllEmailsInQueueAsync();
        Task<bool> SaveEmailInQueueAsync(IEnumerable<EmailQueue> models);
    }
}