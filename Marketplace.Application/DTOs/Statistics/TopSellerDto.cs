
namespace Marketplace.Application.DTOs.Statistics
{
    public class TopSellerDto
    {
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public int TotalProducts { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRating { get; set; }
    }
}