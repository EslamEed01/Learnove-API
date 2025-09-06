using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetBycourseIdAsync(int courseId, int page, int pageSize);
        Task<Review?> GetByIdAsync(int reviewId);
        Task AddAsync(Review review);
        void Update(Review review);
        void Delete(Review review);
        Task<bool> ExistsForUserAsync(int courseId, string userId);
        Task<bool> HasUserPurchasedCourseAsync(int courseId, string userId);
        Task SaveChangesAsync();
    }
}