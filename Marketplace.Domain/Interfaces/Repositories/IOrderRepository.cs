using Marketplace.Domain.Entities;


namespace Marketplace.Domain.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetUserOrdersAsync (int  userId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Order?> GetOrderWithItemsAsync(int orderId);
        Task<decimal> GetTotalSalesAsync(int? sellerId = null);
        Task<int> GetOrderCountAsync(int? sellerId = null);
    }
}
