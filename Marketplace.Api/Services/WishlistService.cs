using AutoMapper;
using Marketplace.Application.DTOs.Products;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Domain.Entities;
using Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<WishlistService> _logger;

        public WishlistService(
            AppDbContext context,
            IMapper mapper,
            ILogger<WishlistService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<bool> AddToWishlistAsync(int userId, int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                    throw new ArgumentException("Product not found");


                var existing = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

                if (existing != null)
                    return false;

                var favorite = new Favorite
                {
                    UserId = userId,
                    ProductId = productId,
                    CreatedAt = DateTime.UtcNow,
                    AddedAt = DateTime.UtcNow,
                    IsActive = true,
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} added to wishlist for user {UserId}", productId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to wishlist for user {UserId}", productId, userId);
                return false;
            }
        }

        public async Task<bool> ClearWishlistAsync(int userId)
        {
            try
            {
                var favorites = await _context.Favorites
                    .Where(f => f.UserId == userId && f.IsActive)
                    .ToListAsync();

                foreach(var favorite in favorites)
                {
                    favorite.IsActive = false;
                    favorite.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Wishlist cleared for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing wishlist for user {UserId}", userId);
                return false;
            }
        }

        public async Task<IEnumerable<ProductDto>> GetWishlistAsync(int userId)
        {
            try
            {
                var favorites = await _context.Favorites
                    .Include(f => f.Product)
                       .ThenInclude(p => p.Images)
                    .Include(f => f.Product)
                       .ThenInclude(p => p.Category)
                    .Where(f => f.UserId == userId && f.IsActive)
                    .OrderByDescending(f => f.AddedAt)
                    .Select(f => f.Product)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<ProductDto>>(favorites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wishlist for user {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetWishlistCountAsync(int userId)
        {
            try
            {
                return await _context.Favorites
                    .CountAsync(f => f.UserId == userId && f.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wishlist count for user {UserId}", userId);
                return 0;
            }
        }

        public async Task<bool> IsInWishlistAsync(int userId, int productId)
        {
            try
            {
                return await _context.Favorites
                    .AnyAsync(f => f.UserId == userId && f.ProductId == productId && f.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking wishlist for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(int userId, int productId)
        {
            try
            {
                var favorite =  await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

                if (favorite == null)
                    return false;

                favorite.IsActive = false;
                favorite.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} removed from wishlist for user {UserId}", productId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product {ProductId} from wishlist for user {UserId}", productId, userId);
                return false;
            }
        }
    }
}
