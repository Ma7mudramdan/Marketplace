using Marketplace.Application.DTOs.Statistics;

namespace Marketplace.Api.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<SalesStatisticsDto> GetSalesStatisticsAsync(DateTime? startDate, DateTime? endDate);
        Task<ProductStatisticsDto> GetProductStatisticsAsync();
        Task<UserStatisticsDto> GetUserStatisticsAsync();
        Task<IEnumerable<DailySalesDto>> GetDailySalesAsync(int days);
        Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int count);
        Task<IEnumerable<TopSellerDto>> GetTopSellersAsync(int count);
    }
}
