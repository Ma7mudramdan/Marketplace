
using Marketplace.Application.DTOs.Orders;
using Marketplace.Domain.Entities;

namespace Marketplace.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, int userId);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, string status);
        Task CancelOrderAsync(int orderId , int userId);
        Task<bool> OrderExistsAsync(int id);
        Task<decimal> GetTotalSalesAsync(int? sellerId = null);
        Task<int> GetOrderCountAsync(int? sellerId = null);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);

       
    }
}