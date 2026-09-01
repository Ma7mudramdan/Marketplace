
namespace Marketplace.Application.DTOs.Statistics
{
    public class SalesStatisticsDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal RevenueGrowth { get; set; }
        public int OrdersGrowth { get; set; }
    }
}