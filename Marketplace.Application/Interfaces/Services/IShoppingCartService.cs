// Marketplace.Application/Interfaces/Services/IShoppingCartService.cs
using Marketplace.Application.DTOs.ShoppingCart;

namespace Marketplace.Application.Interfaces.Services
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto?> GetCartByUserIdAsync(int userId);
        Task AddToCartAsync(int userId, AddToCartDto addToCartDto);
        Task RemoveFromCartAsync(int cartItemId);
        Task UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task ClearCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId);
    }
}