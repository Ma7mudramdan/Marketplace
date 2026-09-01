using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Infrastructure.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        

        public ShoppingCartRepository(AppDbContext context) : base(context) { }
        
       

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);
            
            if(cart == null)
            {
                cart = new ShoppingCart()
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                };

                await _context.AddAsync(cart);
                await _context.SaveChangesAsync();

            }

            var existingItem =  _context.CartItems
                                    .FirstOrDefault(ci => ci.ProductId == productId && ci.ShoppingCartId == cart.Id);

            if(existingItem != null)
            {
                existingItem.Quantity += quantity;
                _context.CartItems.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem()
                {
                    ProductId = productId,
                    ShoppingCartId = cart.Id,
                    Quantity = quantity,
                    AddedAt = DateTime.UtcNow,
                    IsActive = true,
                }; 
                await _context.AddAsync(cartItem);

               
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await GetCartByUserIdAsync(userId);

            if(cart != null)
            {
                var items = _context.CartItems
                                    .Where(ci => ci.ShoppingCartId == cart.Id);

                _context.CartItems.RemoveRange(items);
                await _context.SaveChangesAsync();
                                    
            }
        }


        public async Task<ShoppingCart?> GetCartByUserIdAsync(int userId)
        {
           return  await _context.Carts
                                 .FirstOrDefaultAsync(c => c.UserId == userId);
                                      
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if(cart  == null) return 0;

            return await _context.CartItems
                                 .Where(ci => ci.ShoppingCartId == cart.Id)
                                 .SumAsync(ci => ci.Quantity);
                                 
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            var cart = await GetCartWithItemsAsync(userId);
            if (cart == null) return 0;

            return cart.Items.Sum(ci => ci.Product.Price * ci.Quantity);
        }

        public async Task<ShoppingCart?> GetCartWithItemsAsync(int userId)
        {
            return await _context.Carts.Include(c => c.Items)
                                          .ThenInclude(ci => ci.Product)
                                            .ThenInclude(p =>p.Images)
                                        .FirstOrDefaultAsync(c => c.UserId == userId);
        }

    

        public async Task RemoveFromCartAsync(int cartItemId)
        {
             var cartItem = await _context.CartItems.FindAsync(cartItemId);
             if (cartItem != null)
             {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
             }
        }

     

        public async Task UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem != null)
            {
                if (quantity <= 0)
                    _context.CartItems.Remove(cartItem);
                else
                {
                    cartItem.Quantity = quantity;
                    _context.CartItems.Update(cartItem);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
