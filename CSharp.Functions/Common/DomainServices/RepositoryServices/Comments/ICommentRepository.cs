using Common.Entities;

namespace Common
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentsAsync();
        Task<IEnumerable<Comment>> GetCommentsAsync(string contentId);
        Task<Comment> AddOrEditCommentAsync(Comment comment);
        Task<bool> DeleteCommentByIdAsync(string id);
    }
}
