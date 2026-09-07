
using AutoMapper;
using FluentAssertions;
using Marketplace.Application.DTOs.Orders;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Application.Services;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Entities.Enums;
using Marketplace.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marketplace.Tests
{
    public class OrderServiceTest
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IShoppingCartRepository> _cartRepoMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;

        private readonly OrderService _orderService;

        public OrderServiceTest()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _cartRepoMock = new Mock<IShoppingCartRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrderService>>();

            _orderService = new OrderService(
                _orderRepoMock.Object,
                _productRepoMock.Object,
                _cartRepoMock.Object,
                _emailServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_Return_OrderDto_When_OrderExists()
        {
            //Arrange
            var order = new Order
            {
                Id = 1,
                CustomerId = 3,
                Status = OrderStatus.Pending
            };

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status.ToString()
            };

            _orderRepoMock.Setup(repo => repo.GetOrderWithItemsAsync(1))
                .ReturnsAsync(order);

            _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>()))
                .Returns(orderDto);

            //Act 
            var result = await _orderService.GetOrderByIdAsync(1);


            //Assert 

            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.Equal(order.CustomerId, result.CustomerId);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_Return_Null_When_OrderDoesNotExist()
        {
            //Arrange
            _orderRepoMock.Setup(repo => repo.GetOrderWithItemsAsync(1))
                  .ReturnsAsync((Order?)null);


            //Act 
            var result = await _orderService.GetOrderByIdAsync(1);

            //Assert
            Assert.Null(result);
            _mapperMock.Verify(
                m => m.Map<OrderDto>(It.IsAny<Order>()),
                Times.Never());

        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_Throw_When_RepositoryThrows()
        {
            //Arrange
            _orderRepoMock.Setup(repo => repo.GetOrderWithItemsAsync(1))
                .ThrowsAsync(new ArgumentException("Order not found"));

            //Assert 
            await Assert.ThrowsAsync<ArgumentException>(() => _orderService.GetOrderByIdAsync(1));
        }

        [Fact]
        public async Task GetUserOrdersAsync_Should_Return_NonCancelledOrders()
        {
            //Arrange
            var orders = new List<Order>
            {
                new Order{ Id = 1, CustomerId = 2, Status = OrderStatus.Pending },
                new Order{ Id = 2, CustomerId = 2, Status = OrderStatus.Pending },
                new Order{ Id = 3, CustomerId = 2, Status = OrderStatus.Cancelled },
            };

            var expectedDtos = new List<OrderDto>
            {
                new OrderDto{ Id = 1, CustomerId = 2, Status = OrderStatus.Pending.ToString() },
                new OrderDto{ Id = 2, CustomerId = 2, Status = OrderStatus.Pending.ToString()},
            };

            _orderRepoMock.Setup(repo => repo.GetUserOrdersAsync(2))
                .ReturnsAsync(orders);

            _mapperMock.Setup(m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()))
                .Returns(expectedDtos);

            //Act 
            var result = (await _orderService.GetUserOrdersAsync(2)).ToList();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.DoesNotContain(result, o => o.Status == OrderStatus.Cancelled.ToString());

        }

        [Fact]
        public async Task GetUserOrdersAsync_Should_Throw_When_RepositoryThrows()
        {
            // Arrange
            _orderRepoMock
                .Setup(x => x.GetUserOrdersAsync(10))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _orderService.GetUserOrdersAsync(10));
        }
    }
}
