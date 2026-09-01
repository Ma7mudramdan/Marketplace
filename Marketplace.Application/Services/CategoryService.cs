using AutoMapper;
using Marketplace.Application.DTOs.Categories;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Marketplace.Application.Services
{
    public class CategoryService : ICategoryService
    {
       
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _categoryRepository.ExistsAsync(id);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            try
            {
              if(createDto.ParentCategoryId.HasValue)
                {
                    if (!await _categoryRepository.ExistsAsync(createDto.ParentCategoryId.Value))
                        throw new ArgumentException("Parent category not exist");
                }

                var category = _mapper.Map<Category>(createDto);
                category.IsActive = true;
                category.CreatedAt = DateTime.UtcNow;
                

                await _categoryRepository.AddAsync(category);

                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex,$"Error while creating category");
                throw;
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);

                if (category == null)
                    throw new ArgumentException("category not found");

                //soft delete
                category.IsActive = false;
                category.UpdatedAt = DateTime.UtcNow;

                _categoryRepository.Update(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error can get category with Id = {id}");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            try
            {
               var categories =await _categoryRepository.GetActiveCategoriesAsync();
                
                return _mapper.Map<IEnumerable<CategoryDto>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting active categories");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<CategoryDto>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting all categories");
                throw;
            }
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);

                return category != null ? _mapper.Map<CategoryDto>(category) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting category that Id = {id}");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoryHierarchyAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetCategoryHierarchyAsync();
                return _mapper.Map<IEnumerable<CategoryDto>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting category hiererchy");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentId)
        {
            try
            {
                var subCategories = await _categoryRepository.GetSubCategoriesAsync(parentId);
                return _mapper.Map<IEnumerable<CategoryDto>>(subCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subcategories for parent: {ParentId}", parentId);
                throw;
            }
        }

        public async Task<bool> HasSubCategoriesAsync(int categoryId)
        {
            try
            {
                return await _categoryRepository.HasSubCategoriesAsync(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while checking subcategories of parent Id = {categoryId}");
                throw;
            }
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, CreateCategoryDto updateDto)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);

                if (category == null)
                    throw new ArgumentException("Category not found");


                // Validate parent category if provided
                if (updateDto.ParentCategoryId.HasValue)
                {
                    if (!await _categoryRepository.ExistsAsync(updateDto.ParentCategoryId.Value))
                    {
                        throw new ArgumentException("Parent category does not exist");
                    }
                }

                _mapper.Map(updateDto, category);
                category.UpdatedAt = DateTime.UtcNow;

                _categoryRepository.Update(category);

                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during update category that Id = {id}");
                throw;
            }
        }
    }
}
