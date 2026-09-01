
namespace Marketplace.Application.DTOs.Statistics
{
    public class ProductStatisticsDto
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int FeaturedProducts { get; set; }
        public int PendingApproval { get; set; }
        public int OutOfStock { get; set; }
        public int LowStock { get; set; }
        public int Categories { get; set; }
    }
}