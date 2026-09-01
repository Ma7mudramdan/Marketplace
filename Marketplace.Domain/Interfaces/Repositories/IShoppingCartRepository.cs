using Marketplace.Domain.Entities;

namespace Marketplace.Domain.Interfaces.Repositories
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<ShoppingCart?> GetCartByUserIdAsync(int userId);
        Task<ShoppingCart?> GetCartWithItemsAsync(int userId);
        Task AddToCartAsync(int userId, int productId, int quantity);
        Task RemoveFromCartAsync(int cartItemId);
        Task UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task ClearCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId);
    }
}
