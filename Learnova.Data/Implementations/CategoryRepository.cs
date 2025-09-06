using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data.Context;
using Learnova.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {

        private readonly LearnoveDbContext _learnoveDbContext;

        public CategoryRepository(LearnoveDbContext learnoveDbContext)
        {
            _learnoveDbContext = learnoveDbContext;
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await _learnoveDbContext.Categories.AddAsync(category);
            await _learnoveDbContext.SaveChangesAsync();

        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            await _learnoveDbContext.Categories.Where(c => c.CategoryId == categoryId)
                          .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int page, int pageSize)
        {
            return await _learnoveDbContext.Categories.AsNoTracking()
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {

            return await _learnoveDbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {

            return await _learnoveDbContext.Categories
               .AsNoTracking()
               .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var existingCategory = await _learnoveDbContext.Categories
          .FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);

            await _learnoveDbContext.SaveChangesAsync();
            return existingCategory;
        }
    }
}
