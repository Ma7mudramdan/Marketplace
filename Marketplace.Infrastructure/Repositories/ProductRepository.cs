using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Infrastructure.Repositories
{
     public class ProductRepository : Repository<Product>, IProductRepository
     {
        public ProductRepository(AppDbContext context) : base(context) {  }
       
        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {

           return  await _context.Products
                                 .Where(p => p.IsActive && p.StockQuantity > 0)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Products
                                 .Where(p => p.CategoryId == categoryId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetBySellerAsync(int sellerId)
        {
           
           return  await _context.Products
                                 .Include(p => p.Images)
                                 .Include(p => p.Category)
                                 .Where(p => p.SellerId == sellerId && p.IsActive)
                                 .OrderByDescending(p => p.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count)
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.Images)
                                 .Where(p => p.IsActive && p.IsFeatured)
                                 .OrderByDescending(p => p.CreatedAt)
                                 .Take(count)
                                 .ToListAsync();
        }

        public async Task<Product> GetProductWithImagesAsync(int id)
        {

            return await _context.Products.Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> IsProductInStockAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);

            return product != null && product.StockQuantity >= quantity;
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, string? condition)
        {
            var query = _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.Images)
                                .Where(p => p.IsActive);

            if(!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                                         p.Description.Contains(searchTerm));
            }

            if(categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if(minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if(maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            if(!string.IsNullOrWhiteSpace(condition))
            {
                query = query.Where(p => p.Condition.ToString() == condition);
            }

            return await query.OrderByDescending(p => p.CreatedAt)
                              .ToListAsync();
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);

            if(product != null)
            {
                product.StockQuantity += quantity;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
