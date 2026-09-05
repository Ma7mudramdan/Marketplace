using AutoMapper;
using Marketplace.Application.DTOs.Pagination;
using Marketplace.Application.DTOs.Products;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;



namespace Marketplace.Application.Services
{
    public class ProductService : IProductService
    {

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto, int sellerId)
        {
            try
            {
                if (!await _categoryRepository.ExistsAsync(createDto.CategoryId))
                    throw new ArgumentException($"Category with this Id {createDto.CategoryId} does not exist ");

               
                var product = _mapper.Map<Product>(createDto);
                product.SellerId = sellerId;

                await _productRepository.AddAsync(product);

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product for seller: {SellerId}", sellerId);
                throw;
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                    throw new ArgumentException("product not found");

                //soft delete
                product.IsActive = false;
                product.UpdatedAt = DateTime.UtcNow;
                _productRepository.Update(product); 

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting product that Id = {id}");
                throw;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            try
            {
              var products = await  _productRepository.GetAllAsync();
              return _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error getting all products");
                throw;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int count)
        {
            try
            {
                var products = await _productRepository.GetFeaturedProductsAsync(count);

                return _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured product ");
                throw;
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                return product != null ? _mapper.Map<ProductDto>(product) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting product with Id = {id}");
                throw;
            }
        }

        public async Task<int> GetProductCountAsync()
        {
            return await _productRepository.CountAsync(); 
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _productRepository.GetByCategoryAsync(categoryId);
                return _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting products by category ID = {categoryId}");
                throw;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetProductsBySellerAsync(int sellerId)
        {
            try
            {
                var products = await _productRepository.GetBySellerAsync(sellerId);
                return _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting product with seller Id = {sellerId}");
                throw;
            }
        }

        public async Task<ProductDto> GetProductWithImagesAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetProductWithImagesAsync(id);
                return product != null ? _mapper.Map<ProductDto>(product) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting product with Id = {id}");
                throw;
            }
        }

        public async Task<bool> IsProductInStockAsync(int productId, int quantity)
        {
        
                bool isStock = await _productRepository.IsProductInStockAsync(productId, quantity);

                return isStock;
            
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _productRepository.ExistsAsync(id);
        }

        public async Task<PaginatedResultDto<ProductDto>> SearchProductsAsync(ProductSearchDto searchDto)
        {
            try
            {
                if (searchDto.PageSize > 50) searchDto.PageSize = 50;
                if (searchDto.Page < 1) searchDto.Page = 1;

                var products = await _productRepository.SearchProductsAsync(
                                                            searchDto.SearchTerm,
                                                            searchDto.CategoryId,
                                                            searchDto.MinPrice,
                                                            searchDto.MaxPrice,
                                                            searchDto.Condition);

                if(searchDto.IsFeatured.HasValue)
                {
                    products = products.Where(p => p.IsFeatured ==  searchDto.IsFeatured);
                }

                if (searchDto.InStock.HasValue)
                    products = products.Where(p => p.StockQuantity > 0);

                products = searchDto.SortBy?.ToLower() switch
                {
                    "price" => searchDto.Ascending ? products.OrderBy(p => p.Price) : products.OrderByDescending(p => p.Price),
                    "name" => searchDto.Ascending ? products.OrderBy(p => p.Name) : products.OrderByDescending(p => p.Name),
                    "rating" => searchDto.Ascending ? products.OrderBy(p => p.AverageRating) : products.OrderByDescending(p => p.AverageRating),
                    "popularity" =>  products.OrderByDescending(p => p.Views),
                    _ => products.OrderByDescending(p => p.CreatedAt)
                };

                if(searchDto.Ascending)
                    products = products.OrderBy(p => p.Price);

                var totalCount = products.Count();
                var pagedProducts = products
                                    .Skip((searchDto.Page - 1) * searchDto.PageSize)
                                    .Take(searchDto.PageSize)
                                    .ToList();

                return new PaginatedResultDto<ProductDto>
                {
                    Items = _mapper.Map<List<ProductDto>>(pagedProducts),
                    TotalCount = totalCount,
                    Page = searchDto.Page,
                    PageSize = searchDto.PageSize
                };
                                    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                throw;
            }
        }

        public async Task<ProductDto> UpdateProductAsync(UpdateProductDto updateDto)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(updateDto.Id);

                if (product == null)
                    throw new ArgumentException("Product not found");

                _mapper.Map(updateDto, product);
                product.UpdatedAt = DateTime.UtcNow;


                _productRepository.Update(product);

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during update product that Id = {updateDto.Id}");
                throw;
            }
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            try
            {
                var product = _productRepository.GetByIdAsync(productId);

                if (product == null)
                    throw new ArgumentException("Product not found");

                await _productRepository.UpdateStockAsync(productId, quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product: {ProductId}", productId);
                throw;
            }
        }
    }
}
