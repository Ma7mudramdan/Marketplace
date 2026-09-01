using Marketplace.Domain.Entities;


namespace Marketplace.Domain.Interfaces.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetBySellerAsync(int sellerId);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count);
        Task<IEnumerable<Product>> SearchProductsAsync(
           string? searchTerm,
           int? categoryId,
           decimal? minPrice,
           decimal? maxPrice,
           string? condition);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task UpdateStockAsync(int productId, int quantity);
        Task<bool> IsProductInStockAsync(int productId, int quantity);
        Task<Product> GetProductWithImagesAsync(int id);
    }
}
