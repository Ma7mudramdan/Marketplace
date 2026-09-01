
using Marketplace.Application.DTOs.Products;
using Marketplace.Application.DTOs.Pagination;

namespace Marketplace.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int count);
        Task<PaginatedResultDto<ProductDto>> SearchProductsAsync(ProductSearchDto searchDto);
        Task<ProductDto> CreateProductAsync(CreateProductDto createDto, int sellerId);
        Task<ProductDto> UpdateProductAsync(UpdateProductDto updateDto);
        Task DeleteProductAsync(int id);
        Task<bool> ProductExistsAsync(int id);
        Task UpdateStockAsync(int productId, int quantity);
        Task<bool> IsProductInStockAsync(int productId, int quantity);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<ProductDto>> GetProductsBySellerAsync(int sellerId);
        Task<int> GetProductCountAsync();
        Task <ProductDto> GetProductWithImagesAsync(int id);
    }
}