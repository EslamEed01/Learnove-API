using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(int page, int pageSize);
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task<Category> GetCategoryByNameAsync(string name);
        Task CreateCategoryAsync(Category category);

        Task<Category> UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int categoryId);

    }
}
