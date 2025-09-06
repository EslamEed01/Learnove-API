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

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public void Delete(Review review)
        {
            _context.Reviews.Remove(review);
        }

        public async Task<bool> ExistsForUserAsync(int courseId, string userId)
        {
            return await _context.Reviews.AnyAsync(r => r.CourseId == courseId && r.UserId == userId);
        }

        public async Task<Review?> GetByIdAsync(int reviewId)
        {
            return await _context.Reviews
                        .Include(r => r.User)
                        .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }

        public async Task<IEnumerable<Review>> GetBycourseIdAsync(int courseId, int page, int pageSize)
        {
            return await _context.Reviews
            .Where(r => r.CourseId == courseId)
            .OrderByDescending(r => r.ReviewId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(r => r.User)
            .ToListAsync();
        }

        public async Task<bool> HasUserPurchasedCourseAsync(int courseId, string userId)
        {

            return await _context.Orders
                .AnyAsync(o => o.UserId == userId && o.OrderItems.Any(i => i.CourseId == courseId));
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Review review)
        {
            _context.Reviews.Update(review);
        }
    }
}
