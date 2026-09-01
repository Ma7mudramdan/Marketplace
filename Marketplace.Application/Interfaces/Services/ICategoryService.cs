
using Marketplace.Application.DTOs.Categories;

namespace Marketplace.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
        Task<IEnumerable<CategoryDto>> GetCategoryHierarchyAsync();
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
        Task<CategoryDto> UpdateCategoryAsync(int id, CreateCategoryDto updateDto);
        Task DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
        Task<bool> HasSubCategoriesAsync(int categoryId);
        Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentId);
    }
}