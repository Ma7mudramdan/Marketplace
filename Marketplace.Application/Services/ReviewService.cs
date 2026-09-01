
using AutoMapper;
using Microsoft.Extensions.Logging;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Application.DTOs.Reviews;
using Marketplace.Application.Interfaces.Services;

namespace Marketplace.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewRepository reviewRepository,
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(id);
                return review != null ? _mapper.Map<ReviewDto>(review) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review by id: {ReviewId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId)
        {
            try
            {
                // Verify product exists
                if (!await _productRepository.ExistsAsync(productId))
                {
                    throw new ArgumentException("Product not found");
                }

                var reviews = await _reviewRepository.GetProductReviewsAsync(productId);
                return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for product: {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(int userId)
        {
            try
            {
                var reviews = await _reviewRepository.GetUserReviewsAsync(userId);
                return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createDto, int userId)
        {
            try
            {
                // 1. Verify product exists
                if (!await _productRepository.ExistsAsync(createDto.ProductId))
                {
                    throw new ArgumentException("Product not found");
                }

                // 2. Check if user already reviewed this product
                var existingReview = await _reviewRepository.GetUserProductReviewAsync(userId, createDto.ProductId);
                if (existingReview != null)
                {
                    throw new InvalidOperationException("You have already reviewed this product");
                }

                // 3. Create review
                var review = new Review
                {
                    ProductId = createDto.ProductId,
                    UserId = userId,
                    Rating = createDto.Rating,
                    Comment = createDto.Comment,
                    IsApproved = false, // Requires admin approval
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _reviewRepository.AddAsync(review);

                _logger.LogInformation("Review created for product {ProductId} by user {UserId}",
                    createDto.ProductId, userId);

                return _mapper.Map<ReviewDto>(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review for product: {ProductId}", createDto.ProductId);
                throw;
            }
        }

        public async Task<ReviewDto> UpdateReviewAsync(int id, CreateReviewDto updateDto)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(id);
                if (review == null)
                {
                    throw new ArgumentException("Review not found");
                }

                // Don't allow updating approved reviews (or have admin override)
                if (review.IsApproved)
                {
                    throw new InvalidOperationException("Cannot update an approved review");
                }

                review.Rating = updateDto.Rating;
                review.Comment = updateDto.Comment;
                review.UpdatedAt = DateTime.UtcNow;

                _reviewRepository.Update(review);

                _logger.LogInformation("Review {ReviewId} updated", id);

                return _mapper.Map<ReviewDto>(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review: {ReviewId}", id);
                throw;
            }
        }

        public async Task DeleteReviewAsync(int id)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(id);
                if (review == null)
                {
                    throw new ArgumentException("Review not found");
                }

                // Soft delete
                review.IsActive = false;
                review.UpdatedAt = DateTime.UtcNow;
                _reviewRepository.Update(review);

                _logger.LogInformation("Review {ReviewId} deleted", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review: {ReviewId}", id);
                throw;
            }
        }

        public async Task ApproveReviewAsync(int reviewId)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId);
                if (review == null)
                {
                    throw new ArgumentException("Review not found");
                }

                review.IsApproved = true;
                review.ApprovedAt = DateTime.UtcNow;
                review.UpdatedAt = DateTime.UtcNow;
                _reviewRepository.Update(review);

                _logger.LogInformation("Review {ReviewId} approved", reviewId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving review: {ReviewId}", reviewId);
                throw;
            }
        }

        public async Task<double> GetProductAverageRatingAsync(int productId)
        {
            try
            {
                if (!await _productRepository.ExistsAsync(productId))
                {
                    throw new ArgumentException("Product not found");
                }

                return await _reviewRepository.GetProductAverageRatingAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting average rating for product: {ProductId}", productId);
                throw;
            }
        }

        public async Task<int> GetReviewCountAsync(int productId)
        {
            try
            {
                if (!await _productRepository.ExistsAsync(productId))
                {
                    throw new ArgumentException("Product not found");
                }

                return await _reviewRepository.GetReviewCountAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review count for product: {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> HasUserReviewedProductAsync(int userId, int productId)
        {
            try
            {
                var review = await _reviewRepository.GetUserProductReviewAsync(userId, productId);
                return review != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} reviewed product {ProductId}", userId, productId);
                throw;
            }
        }
    }
}