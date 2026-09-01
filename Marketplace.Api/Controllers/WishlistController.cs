using Marketplace.Application.DTOs.Products;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Infrastructure.Data;
using Marketplace.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : BaseApiController
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService, AppDbContext context, ILogger<WishlistController> logger) : base(context, logger)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetWishlist()
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();

                if (userId == 0)
                    return Unauthorized();

                var wishlist = await _wishlistService.GetWishlistAsync(userId);

                return Ok(wishlist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wishlist");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<object>> GetWishlistCount()
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }

                var count = await _wishlistService.GetWishlistCountAsync(userId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wishlist count");
                return StatusCode(500, "An error occurred");
            }
        }

        
        [HttpGet("check/{productId}")]
        public async Task<ActionResult<object>> CheckInWishlist(int productId)
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }

                var exists = await _wishlistService.IsInWishlistAsync(userId, productId);
                return Ok(new { inWishlist = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking wishlist");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();

                if(userId == 0)
                     return Unauthorized();

                var result = await _wishlistService.AddToWishlistAsync(userId,productId);

                if (!result)
                    return BadRequest("Product already in wishlist or not found");

                var count = await _wishlistService.GetWishlistCountAsync(userId);
                return Ok(new { success = true, message = "Added to wishlist", wishlistCount = count });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to wishlist");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }

                var result = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
                if (!result)
                {
                    return NotFound("Product not in wishlist");
                }

                var count = await _wishlistService.GetWishlistCountAsync(userId);
                return Ok(new { success = true, message = "Removed from wishlist", wishlistCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from wishlist");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }

                await _wishlistService.ClearWishlistAsync(userId);
                return Ok(new { success = true, message = "Wishlist cleared" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing wishlist");
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
