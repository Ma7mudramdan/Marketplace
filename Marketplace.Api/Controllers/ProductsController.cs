using Marketplace.Application.DTOs.Products;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Infrastructure.Data;
using Marketplace.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseApiController
    {
        
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(
            IProductService productService,
            ICategoryService categoryService,
            AppDbContext context,
            ILogger<ProductsController> logger)
            : base(context, logger)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet("Search")]

        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] ProductSearchDto searchDto)
        {
            try
            {
                var products = await _productService.SearchProductsAsync(searchDto);
                return Ok(products);
            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex, $"An error occurred while retrieving products {searchDto}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving products.");
            }

        }

        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving products.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving product with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the product.");
            }



        }

        [HttpGet("WithImages/{id}")]
        public async Task<ActionResult<ProductDto>> GetProductWithImages(int id)
        {
            try
            {
                var product = await _productService.GetProductWithImagesAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving product with images for ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the product with images.");
            }
        }

        [HttpGet("Category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving products for category ID {categoryId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving products for the specified category.");
            }
        }

        [HttpGet("Featured")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts([FromQuery] int count = 5)
        {
            try
            {
                var products = await _productService.GetFeaturedProductsAsync(count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving featured products.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving featured products.");
            }
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get user ID from token (will be implemented with Identity)
                var userId = await GetBusinessUserIdAsync();

                if(userId == 0)
                    return Unauthorized("User not found");
                var product = await _productService.CreateProductAsync(createDto, userId);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "An error occurred while creating the product");
            }
        }


        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Seller,Admin")]

        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
        {
            try
            {
                
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound("Product not found");
                }

                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }

                if (product.SellerId != userId && !IsInRole("Admin"))
                {
                    return Forbid("You don't have permission to delete this product");
                }

                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return StatusCode(500, "An error occurred while deleting the product");
            }
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return Ok(new { message = "Product deleted successfully" });
            }
            catch(ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID {id}");
                return StatusCode(500, "An error occurred while deleting the product");
            }
        }

        [HttpGet("{id}/check-stock")]
        public async Task<ActionResult<bool>> CheckProductStock(int id, [FromQuery] int quantity)
        {
            try
            {
                var isInStock = await _productService.IsProductInStockAsync(id, quantity);
                return Ok(isInStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking stock for product with ID {id}");
                return StatusCode(500, "An error occurred while checking the product stock");
            }
        }

        [HttpGet("seller")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetSellerProducts()
        {
            try
            {
                var sellerId = await GetBusinessUserIdAsync();
                if (sellerId == 0)
                {
                    return Unauthorized("User not found");
                }

                var products = await _productService.GetProductsBySellerAsync(sellerId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seller products");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetProductCount()
        {
            try
            {
                var count = await _productService.GetProductCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product count.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving product count.");
            }
        }

        [HttpPut("UpdateStock/{id}")]
       // [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> UpdateProductStock(int id, [FromQuery] int quantity)
        {
            try
            {
                await _productService.UpdateStockAsync(id, quantity);
                return Ok(new { message = "Product stock updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating stock for product with ID {id}");
                return StatusCode(500, "An error occurred while updating the product stock");
            }
        }

        [HttpGet("Exists/{id}")]
        public async Task<ActionResult<bool>> ProductExists(int id)
        {
            try
            {
                var exists = await _productService.ProductExistsAsync(id);

                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking existence for product with ID {id}");
                return StatusCode(500, "An error occurred while checking the product existence");
            }
        }

    }

}