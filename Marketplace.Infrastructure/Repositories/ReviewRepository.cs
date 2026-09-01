using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Infrastructure.Repositories
{
   
        public class ReviewRepository : Repository<Review>, IReviewRepository
        {
            public ReviewRepository(AppDbContext context) : base(context)
            {
            }

            public async Task ApproveReviewAsync(int reviewId)
            {
                var review = await GetByIdAsync(reviewId);

                if(review != null)
                {
                    review.IsApproved = true;
                    review.ApprovedAt = DateTime.UtcNow;
                    Update(review);

                }
            }

            public async Task<IEnumerable<Review>> GetPendingReviewsAsync()
            {
               return await _context.Reviews
                                    .Include(r => r.User)
                                    .Include(r => r.Product)
                                    .Where(r => !r.IsApproved)
                                    .OrderByDescending( r => r.CreatedAt )
                                    .ToListAsync();
            }

            public async Task<double> GetProductAverageRatingAsync(int productId)
            {
                var reviews = await _context.Reviews
                                            .Where(r => r.ProductId == productId)
                                            .ToListAsync();
                return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            }

            public async Task<IEnumerable<Review>> GetProductReviewsAsync(int productId)
            {
               return await _context.Reviews 
                                    .Include(r => r.User)
                                    .Where(r => r.ProductId == productId && r.IsApproved) 
                                    .OrderByDescending(r => r.CreatedAt)
                                    .ToListAsync();

            }

            public async Task<int> GetReviewCountAsync(int productId)
            {
               return await _context.Reviews
                                    .CountAsync(r => r.ProductId == productId && r.IsApproved);
            }

            public async Task<Review?> GetUserProductReviewAsync(int userId, int productId)
            {
               return await _context.Reviews
                                    .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
            }

            public async Task<IEnumerable<Review>> GetUserReviewsAsync(int userId)
            {
               return await _context.Reviews
                                    .Include(r => r.Product)
                                    .Where(r => r.UserId == userId && r.IsApproved)
                                    .OrderByDescending(r => r.CreatedAt)
                                    .ToListAsync();
            }
        }
}

