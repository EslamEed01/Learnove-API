using Learnova.Domain.Entities;

namespace Learnova.Business.Services.Interfaces
{
    public interface IReviewService
    {

        Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId, int page, int pageSize);
        Task<Review?> GetReviewAsync(int reviewId);
        Task<Review> CreateReviewAsync(int courseId, string userId, Review review);
        Task<Review> UpdateReviewAsync(int reviewId, string userId, Review updatedReview);
        Task DeleteReviewAsync(int reviewId, string userId);
    }
}