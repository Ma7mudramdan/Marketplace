
using AutoMapper;
using Marketplace.Application.DTOs.Categories;
using Marketplace.Application.Services;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marketplace.Tests
{
    public class CategoryServiceTest
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CategoryService>> _loggerMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTest()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CategoryService>>();

            _categoryService = new CategoryService(
                _categoryRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CategoryExistsAsync_Should_ReturnTrue_WhenCategoryExists()
        {
            //Arrange 
            _categoryRepositoryMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            //Act 
            var res = await _categoryService.CategoryExistsAsync(1);

            Assert.True(res);
        }

        [Fact]
        public async Task CategoryExistsAsync_Should_ReturnFalse_WhenCategoryDoesNotExist()
        {
            _categoryRepositoryMock
                .Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(false);

            var result = await _categoryService.CategoryExistsAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task CreateCategoryAsync_Should_CreateCategory_WhenNoParent()
        {
            var dto = new CreateCategoryDto { Name = "Phones" };

            var category = new Category();

            var resultDto = new CategoryDto();

            _mapperMock.Setup(m => m.Map<Category>(dto))
                .Returns(category);

            _mapperMock.Setup(m => m.Map<CategoryDto>(category))
                .Returns(resultDto);


            var res = await _categoryService.CreateCategoryAsync(dto);

            Assert.Same(resultDto, res);
            Assert.True(category.IsActive);
            _categoryRepositoryMock.Verify(
                x => x.AddAsync(category)
                , Times.Once);
        }

        [Fact]
        public async Task CreateCategoryAsync_Should_CreateCategory_WhenValidParentExists()
        {
            var dto = new CreateCategoryDto { Name = "Phones", ParentCategoryId = 1 };

            var category = new Category();

            _categoryRepositoryMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            _mapperMock.Setup(m => m.Map<Category>(dto))
                .Returns(category);

            _mapperMock.Setup(x => x.Map<CategoryDto>(category))
                .Returns(new CategoryDto());


            var res = await _categoryService.CreateCategoryAsync(dto);


            Assert.True(category.IsActive);

            _categoryRepositoryMock.Verify(
                x => x.AddAsync(category)
                , Times.Once);

            _categoryRepositoryMock.Verify(
                x => x.AddAsync(category)
                , Times.Once);
        }

        [Fact]
        public async Task CreateCategoryAsync_Should_Throw_WhenParentDoesNotExist()
        {
            var dto = new CreateCategoryDto { Name = "Phones", ParentCategoryId = 1 };

            _categoryRepositoryMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _categoryService.CreateCategoryAsync(dto));
        }

        [Fact]
        public async Task DeleteCategoryAsync_Should_Throw_WhenCategoryNotExist()
        {
            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Category?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _categoryService.DeleteCategoryAsync(1));
        }

        [Fact]
        public async Task DeleteCategoryAsync_WhenCategoryExist_SoftDelete()
        {
            var category = new Category { Id = 1, IsActive = true };

            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(category);

            await _categoryService.DeleteCategoryAsync(1);

            Assert.False(category.IsActive);
            Assert.NotEqual(default, category.UpdatedAt);

            _categoryRepositoryMock.Verify(
                x => x.Update(category),
                Times.Once);
        }

        [Fact]
        public async Task GetActiveCategoriesAsync_Should_ReturnMappedCategories()
        {
            var categories = new List<Category>
            {
                new Category(),
                new Category()
            };

            var expectedDtos = new List<CategoryDto>
            {
                new CategoryDto(),
                new CategoryDto()
            };

            _categoryRepositoryMock.Setup(x => x.GetActiveCategoriesAsync())
                .ReturnsAsync(categories);

            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories))
                .Returns(expectedDtos);

            var result = await _categoryService.GetActiveCategoriesAsync();

            Assert.Same(expectedDtos, result);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_Should_ReturnMappedCategories()
        {
            var categories = new List<Category>();
            var expected = new List<CategoryDto>();

            _categoryRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(categories);

            _mapperMock
                .Setup(x => x.Map<IEnumerable<CategoryDto>>(categories))
                .Returns(expected);

            var result = await _categoryService.GetAllCategoriesAsync();

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_Should_ReturnMappedCategory_WhenFound()
        {
            var category = new Category();
            var expected = new CategoryDto();

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(category);

            _mapperMock
                .Setup(x => x.Map<CategoryDto>(category))
                .Returns(expected);

            var result = await _categoryService.GetCategoryByIdAsync(1);

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_Should_ReturnNull_WhenNotFound()
        {
            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Category?)null);

            var result = await _categoryService.GetCategoryByIdAsync(1);

            Assert.Null(result);
        }


        [Fact]
        public async Task GetCategoryHierarchyAsync_Should_ReturnMappedCategories()
        {
            var categories = new List<Category>();
            var expected = new List<CategoryDto>();

            _categoryRepositoryMock
                .Setup(x => x.GetCategoryHierarchyAsync())
                .ReturnsAsync(categories);

            _mapperMock
                .Setup(x => x.Map<IEnumerable<CategoryDto>>(categories))
                .Returns(expected);

            var result = await _categoryService.GetCategoryHierarchyAsync();

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task GetSubCategoriesAsync_Should_ReturnMappedCategories()
        {
            var categories = new List<Category>();
            var expected = new List<CategoryDto>();

            _categoryRepositoryMock
                .Setup(x => x.GetSubCategoriesAsync(1))
                .ReturnsAsync(categories);

            _mapperMock
                .Setup(x => x.Map<IEnumerable<CategoryDto>>(categories))
                .Returns(expected);

            var result = await _categoryService.GetSubCategoriesAsync(1);

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task HasSubCategoriesAsync_Should_ReturnTrue()
        {
            _categoryRepositoryMock
                .Setup(x => x.HasSubCategoriesAsync(1))
                .ReturnsAsync(true);

            var result = await _categoryService.HasSubCategoriesAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task HasSubCategoriesAsync_Should_ReturnFalse()
        {
            _categoryRepositoryMock
                .Setup(x => x.HasSubCategoriesAsync(1))
                .ReturnsAsync(false);

            var result = await _categoryService.HasSubCategoriesAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateCategoryAsync_Should_UpdateCategory()
        {
            var dto = new CreateCategoryDto
            {
                Name = "Updated Category"
            };

            var category = new Category();
            var expected = new CategoryDto();

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(category);

            _mapperMock
                .Setup(x => x.Map(dto, category))
                .Returns(category);

            _mapperMock
                .Setup(x => x.Map<CategoryDto>(category))
                .Returns(expected);

            var result = await _categoryService.UpdateCategoryAsync(1, dto);

            Assert.Same(expected, result);
            Assert.NotEqual(default, category.UpdatedAt);

            _categoryRepositoryMock.Verify(
                x => x.Update(category),
                Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_Should_Throw_WhenCategoryNotFound()
        {
            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Category?)null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _categoryService.UpdateCategoryAsync(1, new CreateCategoryDto()));

            Assert.Equal("Category not found", exception.Message);
        }

        [Fact]
        public async Task UpdateCategoryAsync_Should_Throw_WhenParentDoesNotExist()
        {
            var category = new Category();

            var dto = new CreateCategoryDto
            {
                ParentCategoryId = 99
            };

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(category);

            _categoryRepositoryMock
                .Setup(x => x.ExistsAsync(99))
                .ReturnsAsync(false);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _categoryService.UpdateCategoryAsync(1, dto));

            Assert.Equal("Parent category does not exist", exception.Message);

            _categoryRepositoryMock.Verify(
                x => x.Update(It.IsAny<Category>()),
                Times.Never);
        }
    }
}
