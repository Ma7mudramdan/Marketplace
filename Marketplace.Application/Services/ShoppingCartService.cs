
using AutoMapper;
using Microsoft.Extensions.Logging;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Application.DTOs.ShoppingCart;
using Marketplace.Application.Interfaces.Services;

namespace Marketplace.Application.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ShoppingCartService> _logger;

        public ShoppingCartService(
            IShoppingCartRepository shoppingCartRepository,
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<ShoppingCartService> logger)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ShoppingCartDto?> GetCartByUserIdAsync(int userId)
        {
            try
            {
                var cart = await _shoppingCartRepository.GetCartWithItemsAsync(userId);
                return cart != null ? _mapper.Map<ShoppingCartDto>(cart) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart for user: {UserId}", userId);
                throw;
            }
        }

        public async Task AddToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            try
            {
                
                // 1. Verify product exists
                if (!await _productRepository.ExistsAsync(addToCartDto.ProductId))
                {
                    throw new ArgumentException("Product not found");
                }

                // 2. Check stock availability
                var product = await _productRepository.GetByIdAsync(addToCartDto.ProductId);
                if (product == null)
                {
                    throw new ArgumentException("Product not found");
                }

                // 3. Get current cart to check existing quantities
                var cart = await _shoppingCartRepository.GetCartWithItemsAsync(userId);
                var existingItem = cart?.Items?.FirstOrDefault(i => i.ProductId == addToCartDto.ProductId);

                var requestedQuantity = addToCartDto.Quantity + (existingItem?.Quantity ?? 0);

                if (product.StockQuantity < requestedQuantity)
                {
                    throw new InvalidOperationException($"Insufficient stock. Available: {product.StockQuantity}");
                }

                // 4. Add to cart
                await _shoppingCartRepository.AddToCartAsync(userId, addToCartDto.ProductId, addToCartDto.Quantity);

                _logger.LogInformation("Product {ProductId} added to cart for user {UserId}",
                    addToCartDto.ProductId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart for user {UserId}",
                    addToCartDto.ProductId, userId);
                throw;
            }
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            try
            {
                await _shoppingCartRepository.RemoveFromCartAsync(cartItemId);
                _logger.LogInformation("Cart item {CartItemId} removed", cartItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item: {CartItemId}", cartItemId);
                throw;
            }
        }

        public async Task UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            try
            {
                if (quantity < 0)
                {
                    throw new ArgumentException("Quantity cannot be negative");
                }

                await _shoppingCartRepository.UpdateCartItemQuantityAsync(cartItemId, quantity);

                _logger.LogInformation("Cart item {CartItemId} quantity updated to {Quantity}",
                    cartItemId, quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId} quantity", cartItemId);
                throw;
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            try
            {
                await _shoppingCartRepository.ClearCartAsync(userId);
                _logger.LogInformation("Cart cleared for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            try
            {
                return await _shoppingCartRepository.GetCartItemCountAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart item count for user {UserId}", userId);
                throw;
            }
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            try
            {
                return await _shoppingCartRepository.GetCartTotalAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart total for user {UserId}", userId);
                throw;
            }
        }
    }
}