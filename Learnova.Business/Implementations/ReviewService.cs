using Learnova.Business.Services.Interfaces;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Interfaces;

namespace Learnova.Business.Implementations
{
    public class ReviewService : IReviewService
    {

        private readonly IReviewRepository _reviewRepository;
        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<Review> CreateReviewAsync(int courseId, string userId, Review review)
        {
            if (!await _reviewRepository.HasUserPurchasedCourseAsync(courseId, userId))
                throw new InvalidOperationException("You must purchase this product before reviewing it.");

            if (await _reviewRepository.ExistsForUserAsync(courseId, userId))
                throw new InvalidOperationException("You have already reviewed this product.");

            review.CourseId = courseId;
            review.UserId = userId;
            review.ReviewDate = DateTime.UtcNow;

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return review;
        }

        public async Task DeleteReviewAsync(int reviewId, string userId)
        {

            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You cannot delete this review.");

            _reviewRepository.Delete(review);
            await _reviewRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId, int page, int pageSize)
        {
            return await _reviewRepository.GetBycourseIdAsync(courseId, page, pageSize);
        }

        public async Task<Review?> GetReviewAsync(int reviewId)
        {
            return await _reviewRepository.GetByIdAsync(reviewId);
        }

        public async Task<Review> UpdateReviewAsync(int reviewId, string userId, Review updatedReview)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You cannot edit this review.");

            review.Rating = updatedReview.Rating;
            review.Title = updatedReview.Title;
            review.Comment = updatedReview.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            _reviewRepository.Update(review);
            await _reviewRepository.SaveChangesAsync();

            return review;
        }
    }
}