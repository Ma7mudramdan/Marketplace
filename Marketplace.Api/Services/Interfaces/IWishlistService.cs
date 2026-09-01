
using Marketplace.Application.DTOs.Products;

namespace Marketplace.Application.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<IEnumerable<ProductDto>> GetWishlistAsync(int userId);
        Task<bool> AddToWishlistAsync(int userId, int productId);
        Task<bool> RemoveFromWishlistAsync(int userId, int productId);
        Task<bool> IsInWishlistAsync(int userId, int productId);
        Task<int> GetWishlistCountAsync(int userId);
        Task<bool> ClearWishlistAsync(int userId);
    }
}