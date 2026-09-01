using Marketplace.Application.DTOs.Reviews;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Infrastructure.Data;
using Marketplace.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : BaseApiController
    {
        private readonly IReviewService _reviewService;
        private readonly IProductService _productService;

        public ReviewsController(
            IReviewService reviewService,
            IProductService productService,
            AppDbContext context,
            ILogger<ReviewsController> logger)
            : base(context, logger)
        {
            _reviewService = reviewService;
            _productService = productService;
        }

        [HttpGet("Product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetProductReviews(int productId)
        {
            try
            {
                var reviews = await _reviewService.GetProductReviewsAsync(productId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product reviews product Id {productId}");
                return StatusCode(500, "An error occurred while retrieving reviews");

            }
        }


        [HttpGet("Product/{productId}/Rating")]
        
        public async Task<ActionResult<object>> GetProductRating(int productId)
        {
            try
            {
                var averageRating = await _reviewService.GetProductAverageRatingAsync(productId);
                var reviewCount = await _reviewService.GetReviewCountAsync(productId);

                return Ok(new {productId, averageRating, reviewCount});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rating for product {ProductId}", productId);
                return StatusCode(500, "An error occurred while retrieving product rating");

            }
        }


        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetMyReviews()
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }
                var reviews = await _reviewService.GetUserReviewsAsync(userId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my reviews");
                return StatusCode(500, "An error occurred while retrieving reviews");
            }
        }

        [HttpPost]
        [Authorize]

        public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto reviewDto)
        {
            try
            {
                if(! ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }
                var review = await _reviewService.CreateReviewAsync(reviewDto, userId);
                return CreatedAtAction(nameof(GetProductReviews), new { productId = review.ProductId }, review);
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
                _logger.LogError(ex, "Error creating review");
                return StatusCode(500, "An error occurred while creating review");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> UpdateReview(int id, [FromBody] CreateReviewDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var review = await _reviewService.UpdateReviewAsync(id, updateDto);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {ReviewId}", id);
                return StatusCode(500, "An error occurred while updating review");
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", id);
                return StatusCode(500, "An error occurred while deleting review");
            }
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveReview(int id)
        {
            try
            {
                await _reviewService.ApproveReviewAsync(id);
                return Ok(new { message = "Review approved successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving review {ReviewId}", id);
                return StatusCode(500, "An error occurred while approving review");
            }
        }

        
    }
}
