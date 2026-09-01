
namespace Marketplace.Application.DTOs.Statistics
{
    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
        public int ItemsSold { get; set; }
    }
}