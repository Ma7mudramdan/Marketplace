

using AutoMapper;
using FluentAssertions;
using Marketplace.Application.DTOs.Reviews;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Application.Services;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marketplace.Tests
{
    public class ReviewServiceTest
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ReviewService>> _loggerMock;
        private readonly ReviewService _reviewService;

        public ReviewServiceTest()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ReviewService>>();

            _reviewService = new ReviewService(
                _reviewRepositoryMock.Object,
                _productRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }


        [Fact]
        public async Task GetReviewByIdAsync_Should_ReturnMappedReview()
        {
            var review = new Review { Id = 1, ProductId = 1 };
            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(review);

            _mapperMock.Setup(m => m.Map<ReviewDto>(It.IsAny<Review>()))
                .Returns(new ReviewDto());

            var res = await _reviewService.GetReviewByIdAsync(1);

            Assert.NotNull(res);
            _mapperMock.Verify(
                m => m.Map<ReviewDto>(It.IsAny<Review>())
                , Times.Once);

        }

        [Fact]
        public async Task GetReviewByIdAsync_Should_ReturnNull_WhenNotFound()
        {
            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Review?)null);

            var res = await _reviewService.GetReviewByIdAsync(1);

            res.Should().Be(null);
        }

        [Fact]
        public async Task GetProductReviewsAsync_Should_Throw_WhenProductDoesNotExist()
        {
            _productRepositoryMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _reviewService.GetProductReviewsAsync(1));
        }

        [Fact]
        public async Task GetProductReviewsAsync_Should_ReturnReviews()
        {
            var reviews = new List<Review>();
            var expected = new List<ReviewDto>();

            _productRepositoryMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            _reviewRepositoryMock.Setup(x => x.GetProductReviewsAsync(1))
                .ReturnsAsync(reviews);

            _mapperMock.Setup(m => m.Map<IEnumerable<ReviewDto>>(reviews))
                .Returns(expected);

            var result = await _reviewService.GetProductReviewsAsync(1);

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task GetUserReviewsAsync_Should_ReturnReviews()
        {
            var reviews = new List<Review>();
            var expected = new List<ReviewDto>();

            _reviewRepositoryMock
                .Setup(x => x.GetUserReviewsAsync(7))
                .ReturnsAsync(reviews);

            _mapperMock
                .Setup(x => x.Map<IEnumerable<ReviewDto>>(reviews))
                .Returns(expected);

            var result = await _reviewService.GetUserReviewsAsync(7);

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task CreateReviewAsync_Should_Throw_WhenProductDoesNotExist()
        {
            var createDto = new CreateReviewDto
            {
                ProductId = 1,
                Rating = 5,
                Comment = "this is comment"
            };

            _productRepositoryMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(false);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _reviewService.CreateReviewAsync(createDto, 2));

            Assert.Equal("Product not found", exception.Message);
            _reviewRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Review>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateReviewAsync_Should_Throw_WhenUserAlreadyReviewedProduct()
        {
            var createDto = new CreateReviewDto
            {
                ProductId = 1,
                Comment = "Great",
                Rating = 5
            };

            _productRepositoryMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            _reviewRepositoryMock.Setup(x => x.GetUserProductReviewAsync(3, 1))
                .ReturnsAsync(new Review());

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _reviewService.CreateReviewAsync(createDto,3));
        }

        [Fact]
        public async Task CreateReviewAsync_Should_CreateUnapprovedReview()
        {
            var createDto = new CreateReviewDto
            {
                ProductId = 1,
                Comment = "Great",
                Rating = 5
            };

            var createdReview = new Review();

            _mapperMock.Setup(m => m.Map<ReviewDto>(It.IsAny<Review>()))
                .Returns(new ReviewDto());

            _reviewRepositoryMock
            .Setup(x => x.GetUserProductReviewAsync(3,1))
            .ReturnsAsync((Review?)null);

            _reviewRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Review>()))
                .Callback<Review>(review => createdReview = review)
                .Returns(Task.CompletedTask);

            _productRepositoryMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);


            await _reviewService.CreateReviewAsync(createDto,3);

            Assert.NotNull(createdReview);
            Assert.Equal(1, createdReview.ProductId);
            Assert.Equal(3, createdReview.UserId);
            Assert.False(createdReview.IsApproved);
            Assert.True(createdReview.IsActive);
        }

        [Fact]
        public async Task UpdateReviewAsync_Should_Throw_WhenNotFound()
        {
            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Review?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _reviewService.UpdateReviewAsync(1, new CreateReviewDto()));
        }

        [Fact]
        public async Task UpdateReviewAsync_Should_Throw_WhenReviewApproved()
        {
            var review = new Review { Id = 1, IsApproved = true };

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(review);


            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _reviewService.UpdateReviewAsync(1,
                new CreateReviewDto()));

            Assert.Equal("Cannot update an approved review", exception.Message);

            _reviewRepositoryMock.Verify(
                x => x.Update(It.IsAny<Review>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateReviewAsync_Should_UpdateUnapprovedReview()
        {
            var review = new Review
            {
                Rating = 2,
                Comment = "Bad",
                IsApproved = false
            };

            var dto = new CreateReviewDto
            {
                Rating = 5,
                Comment = "Good",
                ProductId = 10
            };

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(1))
                 .ReturnsAsync(review);

            _mapperMock.Setup(m => m.Map<ReviewDto>(It.IsAny<Review>()))
                .Returns(new ReviewDto());

            await  _reviewService.UpdateReviewAsync(1, dto);

            Assert.Equal(5, review.Rating);
            Assert.Equal("Good", review.Comment);

            _reviewRepositoryMock.Verify(
                x => x.Update(It.IsAny<Review>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_Should_SoftDeleteReview()
        {
            var review = new Review { IsActive = true };

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(review);

            await _reviewService.DeleteReviewAsync(1);

            Assert.False(review.IsActive);
        }

        [Fact]
        public async Task DeleteReviewAsync_Should_Throw_WhenNotFound()
        {
            _reviewRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Review?)null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _reviewService.DeleteReviewAsync(1));

            Assert.Equal("Review not found", exception.Message);
        }

        [Fact]
        public async Task ApproveReviewAsync_Should_ApproveReview()
        {
            var review = new Review { IsApproved = false };

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(review);

            await _reviewService.ApproveReviewAsync(1);

            Assert.True(review.IsApproved);
            _reviewRepositoryMock.Verify(
                x => x.Update(It.IsAny<Review>()),
                Times.Once);

        }

        [Fact]
        public async Task ApproveReviewAsync_Should_Throw_WhenNotFound()
        {
            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Review?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _reviewService.ApproveReviewAsync(1));
        }

        [Fact]
        public async Task GetProductAverageRatingAsync_Should_ReturnAverage()
        {
            _productRepositoryMock
                .Setup(x => x.ExistsAsync(10))
                .ReturnsAsync(true);

            _reviewRepositoryMock
                .Setup(x => x.GetProductAverageRatingAsync(10))
                .ReturnsAsync(4.5);

            var result =
                await _reviewService.GetProductAverageRatingAsync(10);

            Assert.Equal(4.5, result);
        }

        [Fact]
        public async Task GetProductAverageRatingAsync_Should_Throw_WhenProductNotFound()
        {
            _productRepositoryMock
                .Setup(x => x.ExistsAsync(10))
                .ReturnsAsync(false);

            var exception =
                await Assert.ThrowsAsync<ArgumentException>(
                    () => _reviewService.GetProductAverageRatingAsync(10));

            Assert.Equal("Product not found", exception.Message);
        }

        [Fact]
        public async Task GetReviewCountAsync_Should_ReturnCount()
        {
            _productRepositoryMock
                .Setup(x => x.ExistsAsync(10))
                .ReturnsAsync(true);

            _reviewRepositoryMock
                .Setup(x => x.GetReviewCountAsync(10))
                .ReturnsAsync(8);

            var result =
                await _reviewService.GetReviewCountAsync(10);

            Assert.Equal(8, result);
        }

        [Fact]
        public async Task GetReviewCountAsync_Should_Throw_WhenProductNotFound()
        {
            _productRepositoryMock
                .Setup(x => x.ExistsAsync(10))
                .ReturnsAsync(false);

            var exception =
                await Assert.ThrowsAsync<ArgumentException>(
                    () => _reviewService.GetReviewCountAsync(10));

            Assert.Equal("Product not found", exception.Message);
        }

        [Fact]
        public async Task HasUserReviewedProductAsync_Should_ReturnTrue_WhenReviewExists()
        {
            _reviewRepositoryMock
                .Setup(x => x.GetUserProductReviewAsync(7, 10))
                .ReturnsAsync(new Review());

            var result =
                await _reviewService.HasUserReviewedProductAsync(7, 10);

            Assert.True(result);
        }

        [Fact]
        public async Task HasUserReviewedProductAsync_Should_ReturnFalse_WhenReviewDoesNotExist()
        {
            _reviewRepositoryMock
                .Setup(x => x.GetUserProductReviewAsync(7, 10))
                .ReturnsAsync((Review?)null);

            var result =
                await _reviewService.HasUserReviewedProductAsync(7, 10);

            Assert.False(result);
        }
    }
}
