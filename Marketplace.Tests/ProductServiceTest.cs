

using AutoMapper;
using FluentAssertions;
using Marketplace.Application.DTOs.Pagination;
using Marketplace.Application.DTOs.Products;
using Marketplace.Application.Services;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marketplace.Tests
{
    
    public class ProductServiceTest
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly ProductService _productService;

        public ProductServiceTest()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ProductService>>();

            _productService = new ProductService(
                _productRepoMock.Object,
                _categoryRepoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetProductByIdAsync_IfExist_ShouldReturnProductDto()
        {
            var productId = 1;
            // Arrange
            var product = new Product
            {
                Id = productId,
                Name = "Product1"
            };

            var productDto = new ProductDto
            {
                Id=productId,
                Name = "Product1"
            };
            
            _productRepoMock.Setup(repo => repo.GetByIdAsync(1))
                            .ReturnsAsync(product);

            _mapperMock.Setup(m => m.Map<ProductDto>(product))
                .Returns(productDto);

            // Act

            var result = await _productService.GetProductByIdAsync(1);


            Assert.Equal(productDto, result);
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Product1");

        }

        [Fact]
        public async Task GetProductByIdAsync_IfNotExist_ShouldReturnNull()
        {
            var productId = 1;
            _productRepoMock.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync((Product?)null);

            var result = await _productService.GetProductByIdAsync(productId);

            Assert.Null(result);

        }

        [Fact]
        public async Task CreateProductAsync_IfCategoryExist_CreateProduct()
        {
            var createDto = new CreateProductDto
            {
                Name = "product",
                Price = 100m,
                CategoryId = 1,
                StockQuantity = 3
            };

            _categoryRepoMock
                .Setup(repo => repo.ExistsAsync(createDto.CategoryId))
                .ReturnsAsync(true);

            var product = new Product { Id = 1, Name = createDto.Name };
            _mapperMock.Setup(m => m.Map<Product>(createDto))
                .Returns(product);

            _productRepoMock.Setup(repo => repo.AddAsync(product))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<ProductDto>(product))
                .Returns(new ProductDto { Id = 1, Name = createDto.Name });

            var sellerId = 1; 


            var result = await _productService.CreateProductAsync(createDto, sellerId);


            Assert.NotNull(result);
            result.Name.Should().Be(createDto.Name);
            _productRepoMock.Verify(repo => repo.AddAsync(product),Times.AtLeast(1));
        }

        [Fact]
        public async Task CreateProductAsync_IfCategoryNotExist_CreateProduct()
        {
            var createDto = new CreateProductDto
            {
                Name = "product",
                Price = 100m,
                CategoryId = 1,
                StockQuantity = 3
            };

            var sellerId = 1;


            Func<Task> result = async () => await _productService.CreateProductAsync(createDto, sellerId);


            await result.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task SearchProductsAsync_Should_Return_Matching_Products()
        {
            // Arrange
            var products = new List<Product>
                        {
                            new Product
                            {
                                Id = 1,
                                Name = "iPhone 15",
                                Price = 30000,
                                StockQuantity = 5,
                                IsFeatured = true
                            },
                            new Product
                            {
                                Id = 1,
                                Name = "iPhone 14",
                                Price = 15000,
                                StockQuantity = 2,
                                IsFeatured = true
                            },
                            new Product
                            {
                                Id = 2,
                                Name = "iphone 24",
                                Price = 25000,
                                StockQuantity = 10,
                                IsFeatured = false
                            }
                        };
            
            // when ProductService Method call repository method (SearchProductsAsync)
            // give it this products with no filteration
            _productRepoMock
                .Setup(x => x.SearchProductsAsync(
                    null,
                    null,
                    null,
                    null,
                    null))
                .ReturnsAsync(products);

            _mapperMock
                .Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
                .Returns((List<Product> src) => src
                    .Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price })
                    .ToList());

            var searchDto = new ProductSearchDto
            {
                //SearchTerm = "iPhone",
                Page = 1,
                PageSize = 10,
                IsFeatured = true, // we have 2 product is featured
                InStock = true,
                Ascending = true, // by price
            };

            // Act
            var result = await _productService.SearchProductsAsync(searchDto);

            // Assert
             result.Should().NotBeNull();
             result.TotalCount.Should().Be(2);
             result.Items[0].Price.Should().Be(15000);

        }

    }
}
