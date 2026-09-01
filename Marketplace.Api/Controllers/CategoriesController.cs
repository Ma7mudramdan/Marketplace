using Marketplace.Application.DTOs.Categories;
using Marketplace.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryservice;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryservice, ILogger<CategoriesController> logger)
        {
            _categoryservice = categoryservice;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryservice.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Active")]
        public async Task<IActionResult> GetActiveCategories()
        {
            try
            {
                var categories = await _categoryservice.GetActiveCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetCategoryHierarchy()
        {
            try
            {
                var categories = await _categoryservice.GetCategoryHierarchyAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category hierarchy");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryservice.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving category with ID {id}");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("{id}/SubCategories")]
        public async Task<IActionResult> GetSubCategoriesByCategoryId(int id)
        {
            try
            {
                var subCategories = await _categoryservice.GetSubCategoriesAsync(id);
                return Ok(subCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving subcategories for category with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            try
            {
                var createdCategory = await _categoryservice.CreateCategoryAsync(createCategoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            try
            {
                if(! ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var updatedCategory = await _categoryservice.UpdateCategoryAsync(id, updateCategoryDto);
                if (updatedCategory == null)
                {
                    return NotFound();
                }
                return Ok(updatedCategory);
            }
            catch(ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Category with ID {id} not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating category with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryservice.DeleteCategoryAsync(id);
                return NoContent();
            }
            catch(ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Category with ID {id} not found");
                return NotFound(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Cannot delete category with ID {id} because it has associated products");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}