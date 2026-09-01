
using Marketplace.Application.DTOs.Reviews;

namespace Marketplace.Application.Interfaces.Services
{
    public interface IReviewService
    {
        Task<ReviewDto?> GetReviewByIdAsync(int id);
        Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId);
        Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(int userId);
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto createDto, int userId);
        Task<ReviewDto> UpdateReviewAsync(int id, CreateReviewDto updateDto);
        Task DeleteReviewAsync(int id);
        Task ApproveReviewAsync(int reviewId);
        Task<double> GetProductAverageRatingAsync(int productId);
        Task<int> GetReviewCountAsync(int productId);
        Task<bool> HasUserReviewedProductAsync(int userId, int productId);
    }
}