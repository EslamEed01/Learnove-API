using Learnova.Business.DTOs.CateDTO;

namespace Learnova.Business.Services.Interfaces
{
    public interface ICategoryService
    {

        Task<List<CategoryDTO>> GetAllCategoriesAsync(int page, int pageSize);
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(CategoryDTO dto);
        Task UpdateCategoryAsync(int id, CategoryDTO dto);
        Task DeleteCategoryAsync(int id);
    }
}
