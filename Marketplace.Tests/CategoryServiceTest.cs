
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
                , Times.Once );
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
                , Times.Once );

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
                () =>  _categoryService.CreateCategoryAsync(dto));
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
    }
}
