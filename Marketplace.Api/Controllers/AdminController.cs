using Marketplace.Api.Services.Interfaces;
using Marketplace.Application.DTOs.Admin;
using Marketplace.Application.DTOs.Pagination;
using Marketplace.Application.DTOs.Products;
using Marketplace.Application.DTOs.Statistics;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Infrastructure.Data;
using Marketplace.Infrastructure.Models;
using Marketplace.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseApiController
    {
        private readonly IAdminService _adminService;
        private readonly IStatisticsService _statisticsService;
        
        public AdminController(
            IAdminService adminService,
            IStatisticsService statisticsService,
            ILogger<AdminController> logger,
            AppDbContext context)
            : base(context, logger)
        {
            _adminService = adminService;
            _statisticsService = statisticsService;
        }

        [HttpGet("dashboard/stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashBoardStats()
        {

            try
            {
                var stats = await _adminService.GetDashboardStatsAsync();
                return Ok(stats);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return StatusCode(500, "An error occurred");
            }

        }

        [HttpGet("statistics/sales")]
        public async Task<ActionResult<SalesStatisticsDto>> GetSalesStatistics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var statistics = await _statisticsService.GetSalesStatisticsAsync(startDate, endDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales statistics");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpGet("statistics/products")]
        public async Task<ActionResult<ProductStatisticsDto>> GetProductStatistics()
        {
            try
            {
                var pSatatistics = await _statisticsService.GetProductStatisticsAsync();
                return Ok(pSatatistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product statistics");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("statistics/users")]
        public async Task<ActionResult<UserStatisticsDto>> GetUserStatistics()
        {
            try
            {
                var stats = await _statisticsService.GetUserStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user statistics");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("statistics/top-products")]
        public async Task<ActionResult<IEnumerable<TopProductDto>>> GetTopProducts([FromQuery] int count = 10)
        {
            try
            {
                var topProducts = await _statisticsService.GetTopProductsAsync(count);
                return Ok(topProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top products");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("statistics/top-sellers")]
        public async Task<ActionResult<IEnumerable<TopSellerDto>>> GetTopSellers([FromQuery] int count = 10)
        {
            try
            {
                var topSellers = await _statisticsService.GetTopSellersAsync(count);
                return Ok(topSellers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top sellers");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("statistics/daily-sales")]
        public async Task<ActionResult<IEnumerable<DailySalesDto>>> GetDailySales([FromQuery] int days = 30)
        {
            try
            {
                var dailySales = await _statisticsService.GetDailySalesAsync(days);
                return Ok(dailySales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily sales");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<PaginatedResultDto<UserListDto>>> GetUsers(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] string? search = null,
           [FromQuery] string? role = null)
        {
            try
            {
                var result = await _adminService.GetUsersAsync(page, pageSize, search, role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, "An error occurred");
            }
        }


        [HttpPut("users/role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserRoleUpdateDto model)
        {
            try
            {
                var result = await _adminService.UpdateUserRoleAsync(model.UserId, model.Role);
                if (!result)
                {
                    return NotFound("User not found");
                }

                return Ok(new { success = true, message = "User role updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut("users/{id}/activate")]
        public async Task<IActionResult> ToggleUserActivation(string id)
        {
            try
            {
                var result = await _adminService.ToggleUserActivationAsync(id);
                if (!result)
                {
                    return NotFound("User not found");
                }

                return Ok(new { success = true, message = "User activation toggled" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user activation");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("products/pending")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetPendingProducts()
        {
            try
            {
                var products = await _adminService.GetPendingProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending products");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut("products/{id}/approve")]
        public async Task<IActionResult> ApproveProduct(int id)
        {
            try
            {
                var result = await _adminService.ApproveProductAsync(id);
                if (!result)
                {
                    return NotFound("Product not found");
                }


                return Ok(new { success = true, message = "Product approved" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving product");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut("products/{id}/feature")]
        public async Task<IActionResult> ToggleProductFeature(int id)
        {
            try
            {
                var result = await _adminService.ToggleProductFeatureAsync(id);
                if (!result)
                {
                    return NotFound("Product not found");
                }

                return Ok(new { success = true, message = "Product feature toggled" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling product feature");
                return StatusCode(500, "An error occurred");
            }
        }

    }

}
