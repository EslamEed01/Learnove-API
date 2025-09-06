using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(int reviewId, CancellationToken cancellationToken = default);
        Task<Review?> GetUserReviewForCourseAsync(string userId, int courseId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetUserReviewsAsync(string userId, CancellationToken cancellationToken = default);
        Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default);
        Task<Review> UpdateAsync(Review review, CancellationToken cancellationToken = default);
        Task DeleteAsync(int reviewId, CancellationToken cancellationToken = default);
        Task<int> GetCourseReviewCountAsync(int courseId, CancellationToken cancellationToken = default);
        Task<double> GetCourseAverageRatingAsync(int courseId, CancellationToken cancellationToken = default);
        Task<int[]> GetCourseRatingDistributionAsync(int courseId, CancellationToken cancellationToken = default);
    }
}