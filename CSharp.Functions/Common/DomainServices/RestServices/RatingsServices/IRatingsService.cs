using Common.Entities;

namespace Common
{
    public interface IRatingsService
    {
        Task<InsurerRating> GetInsurerRatingAsync(string companyName);
    }
}