using Marketplace.Application.DTOs.Orders;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Domain.Entities.Enums;
using Marketplace.Infrastructure.Data;
using Marketplace.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        public OrdersController(
            IOrderService orderService,
            IEmailService emailService,
            AppDbContext context,
            ILogger<OrdersController> logger)
            : base(context, logger)
        {
            _orderService = orderService;
            _emailService = emailService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }
                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
              
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }
                var order = await _orderService.CreateOrderAsync(createOrderDto,userId);
                
                var user = await GetBusinessUserAsync();
                if(user != null)
                {
                    await _emailService.SendOrderConfirmationAsync(user.Email, user.FullName, order.Id);
                }    

                
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/Status")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var orderStatus))
                {
                    return BadRequest("Invalid order status");
                }
                var order = await _orderService.UpdateOrderStatusAsync(id, orderStatus.ToString());
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
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
                _logger.LogError(ex, $"Error updating order status for order with ID {id}");
                return StatusCode(500, "Internal server error");
            }

        }
      
        [HttpDelete("{id}")] 
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                await _orderService.CancelOrderAsync(id,userId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new 
                        { ExceptionMessage =ex.Message ,
                          ErrorMessage = $"You don't have any order with id : {id}" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Stats")]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> GetOrderStats()
        {
            try
            {
                var userId = await GetBusinessUserIdAsync();
                if (userId == 0)
                {
                    return Unauthorized();
                }
                var isAdmin = User.IsInRole("Admin");

                var totalSales = await _orderService.GetTotalSalesAsync(isAdmin ? null : userId);
                var orderCount = await _orderService.GetOrderCountAsync(isAdmin ? null : userId);
               
                return Ok(
                    new
                     {
                     totalSales,
                     orderCount,
                     currency = "EGP"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order stats");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
            try
            {
                if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var orderStatus))
                {
                    return BadRequest("Invalid order status");
                }
                var orders = await _orderService.GetOrdersByStatusAsync(orderStatus.ToString());
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders with status {status}");
                return StatusCode(500, "Internal server error");
            }
        }
       

    }
}
