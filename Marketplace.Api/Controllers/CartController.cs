using Marketplace.Application.DTOs.ShoppingCart;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Infrastructure.Data;
using Marketplace.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : BaseApiController
    {
        private readonly IShoppingCartService _cartService;

        public CartController(
            IShoppingCartService cartService,
            AppDbContext context,
            ILogger<CartController> logger)
            : base(context, logger)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }

                var cart = await _cartService.GetCartByUserIdAsync(userId);
                return Ok(cart ?? new ShoppingCartDto { Items = new List<CartItemDto>() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Count")]
        public async Task<IActionResult> GetCartItemCount()
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }
                var count = await _cartService.GetCartItemCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart item count");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Total")]
        public async Task<IActionResult> GetCartTotal()
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }
                var total = await _cartService.GetCartTotalAsync(userId);
                return Ok(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart total");
                return StatusCode(500, "An error occurred while retrieving the cart total");
            }
        }

        [HttpPost("Items")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddToCartDto addItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }
                await _cartService.AddToCartAsync(userId, addItemDto);

                var count = await _cartService.GetCartItemCountAsync(userId);

                return Ok( new {messege = $"item added to cart" , cartCount = count});
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return StatusCode(500, "An error occurred while adding to cart");
            }
        }

        [HttpPut("Items/{itemId}/updateQuantity")]
        public async Task<IActionResult> UpdateCartItem(int itemId, [FromBody] int quantity)
        {
            try
            {
                await _cartService.UpdateCartItemQuantityAsync(itemId, quantity);
                return Ok(new { message = "Cart item updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item");
                return StatusCode(500, "An error occurred while updating the cart item");
            }
        }

        [HttpDelete("Items/{itemId}")]
        public async Task<IActionResult> RemoveItemFromCart(int itemId)
        {
            try
            {
                await _cartService.RemoveFromCartAsync(itemId);
                return Ok(new { message = "Cart item removed successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item");
                return StatusCode(500, "An error occurred while removing the cart item");
            }
        }

        [HttpDelete("Clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }
                await _cartService.ClearCartAsync(userId);
                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return StatusCode(500, "An error occurred while clearing the cart");
            }
        }
       
    }
}
