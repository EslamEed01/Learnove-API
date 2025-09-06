using AutoMapper;
using Learnova.Business.DTOs.CateDTO;
using Learnova.Business.Services.Interfaces;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Interfaces;

namespace Learnova.Business.Implementations
{
    public class CategoryService : ICategoryService
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public Task AddCategoryAsync(CategoryDTO dto)
        {
            var category = _mapper.Map<Category>(dto);
            return _categoryRepository.CreateCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _categoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<List<CategoryDTO>> GetAllCategoriesAsync(int page, int pageSize)
        {
            var category = await _categoryRepository.GetAllCategoriesAsync(page, pageSize);
            return _mapper.Map<List<CategoryDTO>>(category);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task UpdateCategoryAsync(int id, CategoryDTO dto)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found");

            _mapper.Map(dto, category);
            await _categoryRepository.UpdateCategoryAsync(category);
        }
    }
}
