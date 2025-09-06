using Learnova.Business.DTOs.CateDTO;
using Learnova.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        /// <summary>
        /// get all categories with pagination  
        /// </summary>


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int Page = 1, int PageSize = 20)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(Page, PageSize);
            return Ok(categories);
        }


        /// <summary>
        /// get a specific category by id  
        /// </summary>


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(category);
        }

        /// <summary>
        /// create a new category  
        /// </summary>

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryDTO dto)
        {
            await _categoryService.AddCategoryAsync(dto);
            return StatusCode(201);
        }



        /// <summary>
        /// update a specific category by id  
        /// </summary>

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDTO dto)
        {
            await _categoryService.UpdateCategoryAsync(id, dto);
            return NoContent();
        }


        /// <summary>
        /// delete a specific category by id  
        /// </summary>

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }



    }
}
