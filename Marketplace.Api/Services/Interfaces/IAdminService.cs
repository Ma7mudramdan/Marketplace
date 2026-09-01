using Marketplace.Application.DTOs.Admin;
using Marketplace.Application.DTOs.Pagination;
using Marketplace.Application.DTOs.Products;

namespace Marketplace.Api.Services.Interfaces
{
    public interface IAdminService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<PaginatedResultDto<UserListDto>> GetUsersAsync(int page, int pageSize, string? search, string? role);
        Task<bool> UpdateUserRoleAsync(string userId, string role);
        Task<bool> ToggleUserActivationAsync(string userId);
        Task<IEnumerable<ProductDto>> GetPendingProductsAsync();
        Task<bool> ApproveProductAsync(int productId);
        Task<bool> ToggleProductFeatureAsync(int productId);
    }
}
