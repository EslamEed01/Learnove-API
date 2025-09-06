using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data.Context;
using Learnova.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly LearnoveDbContext _context;

        public ReviewRepository(LearnoveDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetByIdAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId, cancellationToken);
        }

        public async Task<Review?> GetUserReviewForCourseAsync(string userId, int courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId, cancellationToken);
        }

        public async Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.CourseId == courseId)
                .OrderByDescending(r => r.ReviewDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Include(r => r.Course)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Review> AddAsync(Review review, CancellationToken cancellationToken = default)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);
            return review;
        }

        public async Task<Review> UpdateAsync(Review review, CancellationToken cancellationToken = default)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync(cancellationToken);
            return review;
        }

        public async Task DeleteAsync(int reviewId, CancellationToken cancellationToken = default)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<int> GetCourseReviewCountAsync(int courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Reviews.CountAsync(r => r.CourseId == courseId, cancellationToken);
        }

        public async Task<double> GetCourseAverageRatingAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var ratings = await _context.Reviews
                .Where(r => r.CourseId == courseId)
                .Select(r => r.Rating)
                .ToListAsync(cancellationToken);

            return ratings.Any() ? ratings.Average() : 0;
        }

        public async Task<int[]> GetCourseRatingDistributionAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var reviews = await _context.Reviews
                .Where(r => r.CourseId == courseId)
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var distribution = new int[5]; // For ratings 1-5
            foreach (var item in reviews)
            {
                if (item.Rating >= 1 && item.Rating <= 5)
                {
                    distribution[item.Rating - 1] = item.Count;
                }
            }

            return distribution;
        }
    }
}