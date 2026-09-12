

using AutoMapper;
using Marketplace.Application.DTOs.ShoppingCart;
using Marketplace.Application.Services;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marketplace.Tests
{
    public class ShoppingCartServiceTest
    {
        private readonly Mock<IShoppingCartRepository> _cartRepositoryMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ShoppingCartService>> _loggerMock;
        private readonly ShoppingCartService _shoppingCartService;

        public ShoppingCartServiceTest()
        {
            _cartRepositoryMock = new Mock<IShoppingCartRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ShoppingCartService>>();

            _shoppingCartService = new ShoppingCartService(
                _cartRepositoryMock.Object,
                _productRepoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetCartByUserIdAsync_Should_ReturnMappedCart()
        {
            var cart = new ShoppingCart();
            var expected = new ShoppingCartDto();

            _cartRepositoryMock
                .Setup(x => x.GetCartWithItemsAsync(1))
                .ReturnsAsync(cart);

            _mapperMock
                .Setup(x => x.Map<ShoppingCartDto>(cart))
                .Returns(expected);

            var result = await _shoppingCartService.GetCartByUserIdAsync(1);

            Assert.Same(expected, result);
        }

        [Fact]
        public async Task GetCartByUserIdAsync_Should_ReturnNull_WhenCartNotFound()
        {
            _cartRepositoryMock
                .Setup(x => x.GetCartWithItemsAsync(1))
                .ReturnsAsync((ShoppingCart?)null);

            var result = await _shoppingCartService.GetCartByUserIdAsync(1);

            Assert.Null(result);
        }


        [Fact]
        public async Task AddToCartAsync_Should_Throw_WhenProductDoesNotExist()
        {
            _productRepoMock
                .Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(false);

            var addToCartDto = new AddToCartDto { ProductId = 1, Quantity = 1 };

            await Assert.ThrowsAsync<ArgumentException>(
                () => _shoppingCartService.AddToCartAsync(1, addToCartDto));
        }

        [Fact]
        public async Task AddToCartAsync_Should_Throw_WhenProductLookupReturnsNull()
        {

            _productRepoMock.Setup(x => x.ExistsAsync(1))
                 .ReturnsAsync(true);

            _productRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Product?)null);

            var addToCartDto = new AddToCartDto { ProductId = 1, Quantity = 1 };

            await Assert.ThrowsAsync<ArgumentException>(
                () => _shoppingCartService.AddToCartAsync(1, addToCartDto));
        }

        [Fact]
        public async Task AddToCartAsync_Should_AddProduct_WhenEnoughStock()
        {
            var product = new Product { Id = 1, StockQuantity = 10 };
            var addToCartDto = new AddToCartDto { ProductId = 1, Quantity = 2 };

            _productRepoMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            _productRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            _cartRepositoryMock.Setup(x => x.GetCartWithItemsAsync(1))
                .ReturnsAsync((ShoppingCart?)null);


            await _shoppingCartService.AddToCartAsync(1, addToCartDto);

            _cartRepositoryMock.Verify(x => x.AddToCartAsync(1, 1, 2), Times.Once);

        }

        [Fact]
        public async Task AddToCartAsync_Should_Throw_WhenStockIsInsufficient()
        {
            var product = new Product { Id = 1, StockQuantity = 5 };
            var addToCartDto = new AddToCartDto { ProductId = 1, Quantity = 6 };

            _productRepoMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            _productRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            _cartRepositoryMock.Setup(x => x.GetCartWithItemsAsync(1))
                .ReturnsAsync((ShoppingCart?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _shoppingCartService.AddToCartAsync(1, addToCartDto));

        }

        [Fact]
        public async Task AddToCartAsync_Should_ConsiderExistingCartQuantity()
        {
            var product = new Product { Id = 1, StockQuantity = 5 };
            var addToCartDto = new AddToCartDto { ProductId = 1, Quantity = 3 };

            var cart = new ShoppingCart
            {
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Quantity = 4 }
                }
            };

            _productRepoMock.Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            _productRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            _cartRepositoryMock.Setup(x => x.GetCartWithItemsAsync(1))
                .ReturnsAsync(cart);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _shoppingCartService.AddToCartAsync(1, addToCartDto));

            Assert.Equal("Insufficient stock. Available: 5", exception.Message);
        }

        [Fact]
        public async Task RemoveFromCartAsync_Should_CallRepository()
        {
            await _shoppingCartService.RemoveFromCartAsync(5);

            _cartRepositoryMock.Verify(
                x => x.RemoveFromCartAsync(5),
                Times.Once);
        }


        [Fact]
        public async Task UpdateCartItemQuantityAsync_Should_Update_WhenQuantityIsValid()
        {
            await _shoppingCartService.UpdateCartItemQuantityAsync(5, 3);

            _cartRepositoryMock.Verify(
                x => x.UpdateCartItemQuantityAsync(5, 3),
                Times.Once);
        }

        [Fact]
        public async Task UpdateCartItemQuantityAsync_Should_Throw_WhenQuantityIsNegative()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _shoppingCartService.UpdateCartItemQuantityAsync(5, -1));

            Assert.Equal("Quantity cannot be negative", exception.Message);

            _cartRepositoryMock.Verify(
                x => x.UpdateCartItemQuantityAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task ClearCartAsync_Should_CallRepository()
        {
            await  _shoppingCartService.ClearCartAsync(7);

            _cartRepositoryMock.Verify(
                x => x.ClearCartAsync(7),
                Times.Once);
        }

        [Fact]
        public async Task GetCartItemCountAsync_Should_ReturnCount()
        {
            _cartRepositoryMock
                .Setup(x => x.GetCartItemCountAsync(7))
                .ReturnsAsync(5);

            var result = await _shoppingCartService.GetCartItemCountAsync(7);

            Assert.Equal(5, result);
        }

        [Fact]
        public async Task GetCartTotalAsync_Should_ReturnTotal()
        {
            _cartRepositoryMock
                .Setup(x => x.GetCartTotalAsync(7))
                .ReturnsAsync(250m);

            var result = await _shoppingCartService.GetCartTotalAsync(7);

            Assert.Equal(250m, result);
        }
    }
}
