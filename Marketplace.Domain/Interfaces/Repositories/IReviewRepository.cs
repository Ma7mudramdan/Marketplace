using Marketplace.Domain.Entities;


namespace Marketplace.Domain.Interfaces.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
        Task<IEnumerable<Review>> GetUserReviewsAsync(int userId);
        Task<Review?> GetUserProductReviewAsync(int userId, int productId);
        Task<double> GetProductAverageRatingAsync(int productId);
        Task<int> GetReviewCountAsync(int productId);
        Task<IEnumerable<Review>> GetPendingReviewsAsync();
        Task ApproveReviewAsync(int reviewId);
    }
}
