
using AutoMapper;
using FluentAssertions;
using Marketplace.Application.DTOs.Orders;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Application.Services;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Entities.Enums;
using Marketplace.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.ContentModel;

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

        [Fact]
        public async Task GetOrdersByStatusAsync_Should_Return_MappedOrders()
        {
            //Arrange
            var orders = new List<Order>
            {
                new Order{ Id = 1,Status = OrderStatus.Pending },
                new Order{Id = 2,Status = OrderStatus.Pending },
                new Order{Id=3,Status = OrderStatus.Processing }
            };

            var expectedOrders = new List<OrderDto>
            {
                new OrderDto{ Id = 1,Status = OrderStatus.Pending.ToString() },
                new OrderDto{Id = 2,Status = OrderStatus.Pending.ToString() },
            };

            _orderRepoMock.Setup(repo => repo.GetOrdersByStatusAsync("Pending"))
                .ReturnsAsync(orders);

            _mapperMock.Setup(m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()))
                .Returns(expectedOrders);

            //Act 
            var res = await _orderService.GetOrdersByStatusAsync("Pending");


            //Assert

            res.Should().NotBeNull();
            Assert.Equal(2, res.Count());
            _mapperMock.Verify(
                m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()),
                Times.Once
                );

            Assert.All(res, x =>
                 Assert.Equal(OrderStatus.Pending.ToString(), x.Status));
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_Should_Throw_When_RepositoryThrows()
        {
            // Arrange
            _orderRepoMock
                .Setup(x => x.GetOrdersByStatusAsync("Pending"))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _orderService.GetOrdersByStatusAsync("Pending"));
        }

        [Fact]
        public async Task CreateOrderAsync_Should_Throw_When_CartIsEmpty()
        {
            //Arrange
            var transaction = new Mock<IDisposable>();

            _orderRepoMock.Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(transaction.Object);

            _cartRepoMock.Setup(repo => repo.GetCartWithItemsAsync(10))
                .ReturnsAsync((ShoppingCart?)null);


            var dto = new CreateOrderDto();

            // Act & Assert

            await Assert.ThrowsAsync<InvalidOperationException>
                (() => _orderService.CreateOrderAsync(dto, 10));

            _orderRepoMock.Verify(
                x => x.RollbackTransactionAsync(),
                Times.Once
                );

            _orderRepoMock.Verify(
                x => x.CommitTransactionAsync(),
                Times.Never
                );
        }

        [Fact]
        public async Task CreateOrderAsync_Should_Throw_When_CartHasNoItems()
        {
            //Arrange
            var transaction = new Mock<IDisposable>();

            _orderRepoMock.Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(transaction.Object);

            _cartRepoMock.Setup(x => x.GetCartItemCountAsync(10))
                .ReturnsAsync(0);

            //Act & Assert

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.CreateOrderAsync(new CreateOrderDto(), 10));

            _orderRepoMock.Verify(x => x.RollbackTransactionAsync(), Times.Once);
            _orderRepoMock.Verify(x => x.CommitTransactionAsync(), Times.Never);


        }

        [Fact]
        public async Task CreateOrderAsync_Should_Throw_When_ProductDoesNotExist()
        {
            //Arrange
            var transaction = new Mock<IDisposable>();

            _orderRepoMock.Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(transaction.Object);

            var cart = new ShoppingCart {
                Items = new List<CartItem> {new CartItem { ProductId = 1, Quantity = 2 } }
                };

            _cartRepoMock
                .Setup(x => x.GetCartWithItemsAsync(10))
                .ReturnsAsync(cart);

            _productRepoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _orderService.CreateOrderAsync(new CreateOrderDto(), 10));

            _orderRepoMock.Verify(
                x => x.RollbackTransactionAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_Should_Throw_When_StockIsInsufficient()
        {
            // Arrange
            var transaction = new Mock<IDisposable>();

            _orderRepoMock.Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(transaction.Object);

            var cart = new ShoppingCart
            {
                Items = new List<CartItem> { new CartItem { ProductId = 1, Quantity = 10 } }
            };

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 100,
                StockQuantity = 5
            };

            _cartRepoMock
                .Setup(x => x.GetCartWithItemsAsync(10))
                .ReturnsAsync(cart);

            _productRepoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.CreateOrderAsync(new CreateOrderDto(), 10));

            _orderRepoMock.Verify(
                x => x.RollbackTransactionAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_Should_Create_Order_And_Update_Stock()
        {
            // Arrange
            var transaction = new Mock<IDisposable>();

            _orderRepoMock.Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(transaction.Object);

            var cart = new ShoppingCart
            {
                Items = new List<CartItem> { new CartItem { ProductId = 1, Quantity = 2 } }
            };

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 50,
                StockQuantity = 10,
                SoldQuantity = 0
            };

            var orderDto = new OrderDto
            {
                Id = 1,
                CustomerId = 10,
                Status = OrderStatus.Pending.ToString()
            };

            _cartRepoMock
                .Setup(x => x.GetCartWithItemsAsync(10))
                .ReturnsAsync(cart);

            _productRepoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            _orderRepoMock
                .Setup(x => x.AddAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(x => x.Map<OrderDto>(It.IsAny<Order>()))
                .Returns(orderDto);

            // Act
            var result = await _orderService.CreateOrderAsync(
                new CreateOrderDto(),
                10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);

            Assert.Equal(8, product.StockQuantity);
            Assert.Equal(2, product.SoldQuantity);

            _productRepoMock.Verify(
                x => x.Update(product),
                Times.Once);

            _orderRepoMock.Verify(
                x => x.AddAsync(It.Is<Order>(o =>
                    o.CustomerId == 10 &&
                    o.Status == OrderStatus.Pending &&
                    o.PaymentStatus == PaymentStatus.Pending &&
                    o.Subtotal == 100 &&
                    o.TaxAmount == 14 &&
                    o.ShippingCost == 0 &&
                    o.TotalAmount == 114)),
                Times.Once);

            _cartRepoMock.Verify(
                x => x.ClearCartAsync(10),
                Times.Once);

            _orderRepoMock.Verify(
                x => x.CommitTransactionAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_Should_Use_DiscountedPrice_When_Available()
        {
            // Arrange
            var transaction = new Mock<IDisposable>();

            _orderRepoMock.Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(transaction.Object);

            var cart = new ShoppingCart
            {
                Items = new List<CartItem> { new CartItem { ProductId = 1, Quantity = 2 } }
            };

            var product = new Product
            {
                Id = 1,
                Name = "Phone",
                Price = 100,
                DiscountedPrice = 80,
                StockQuantity = 10,
                SoldQuantity = 0
            };

            _cartRepoMock
                .Setup(x => x.GetCartWithItemsAsync(10))
                .ReturnsAsync(cart);

            _productRepoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            _orderRepoMock
                .Setup(x => x.AddAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(x => x.Map<OrderDto>(It.IsAny<Order>()))
                .Returns(new OrderDto());

            // Act
            await _orderService.CreateOrderAsync(
                new CreateOrderDto(),
                10);

            // Assert
            _orderRepoMock.Verify(
                x => x.AddAsync(It.Is<Order>(o =>
                    o.Subtotal == 160 &&
                    o.DiscountAmount == 40 &&
                    o.TaxAmount == 22.4m &&
                    o.ShippingCost == 0 &&
                    o.TotalAmount == 182.4m)),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_Should_Use_10_Shipping_When_Subtotal_Is_Less_Than_100()
        {
            // Arrange
            var transaction = new Mock<IDisposable>();

            _orderRepoMock.Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(transaction.Object);

            var cart = new ShoppingCart
            {
                Items = new List<CartItem> { new CartItem { ProductId = 1, Quantity = 1 } }
            };

            var product = new Product
            {
                Id = 1,
                Name = "Mouse",
                Price = 50,
                StockQuantity = 10
            };

            _cartRepoMock
                .Setup(x => x.GetCartWithItemsAsync(10))
                .ReturnsAsync(cart);

            _productRepoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            _orderRepoMock
                .Setup(x => x.AddAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(x => x.Map<OrderDto>(It.IsAny<Order>()))
                .Returns(new OrderDto());

            // Act
            await _orderService.CreateOrderAsync(
                new CreateOrderDto(),
                10);

            // Assert
            _orderRepoMock.Verify(
                x => x.AddAsync(It.Is<Order>(o =>
                    o.Subtotal == 50 &&
                    o.TaxAmount == 7 &&
                    o.ShippingCost == 10 &&
                    o.TotalAmount == 67)),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_Should_Rollback_When_ExceptionOccurs()
        {
            // Arrange
            var transaction = new Mock<IDisposable>();

            _orderRepoMock.Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(transaction.Object);

            _cartRepoMock
                .Setup(x => x.GetCartWithItemsAsync(10))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _orderService.CreateOrderAsync(
                    new CreateOrderDto(),
                    10));

            _orderRepoMock.Verify(
                x => x.RollbackTransactionAsync(),
                Times.Once);

            _orderRepoMock.Verify(
                x => x.CommitTransactionAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_Should_Throw_When_OrderDoesNotExist()
        {
            // Arrange
            _orderRepoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Order?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _orderService.UpdateOrderStatusAsync(
                    1,
                    "Processing"));
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_Should_Throw_When_StatusTransitionIsInvalid()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                Status = OrderStatus.Pending
            };

            _orderRepoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(order);

            // Pending -> Delivered is invalid
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.UpdateOrderStatusAsync(
                    1,
                    "Delivered"));
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_Should_Update_Pending_To_Processing()
        {
            //Arrange 
            var order = new Order { Id = 1, Status = OrderStatus.Pending };
            var orderDto = new OrderDto { Id = 1, Status = OrderStatus.Processing.ToString() };

            _orderRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(order);

            _mapperMock.Setup(m => m.Map<OrderDto>(order))
                .Returns(orderDto);

            //Act 
            var res = await _orderService.UpdateOrderStatusAsync(1, "Processing");

            //Assert
            res.Should().NotBeNull();
            Assert.Equal("Processing",res.Status);
            Assert.Equal(OrderStatus.Processing, order.Status);
            _orderRepoMock.Verify(x => x.Update(order),Times.Once());
            
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_Should_Set_ShippedAt_When_Shipped()
        {
            //Arrange 
            var order = new Order { Id = 1, Status = OrderStatus.Processing };
            var orderDto = new OrderDto { Id = 1, Status = OrderStatus.Shipped.ToString() };

            _orderRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(order);

            _mapperMock.Setup(m => m.Map<OrderDto>(order))
                .Returns(orderDto);

            //Act 
            var res = await _orderService.UpdateOrderStatusAsync(1, "Shipped");

            //Assert 
            res.Should().NotBeNull();
            Assert.NotNull(order.ShippedAt);
            Assert.Null(order.DeliveredAt);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_Should_Set_DeliveredAt_When_Delivered()
        {
            //Arrange 
            var order = new Order { Id = 1, Status = OrderStatus.Shipped };
            var orderDto = new OrderDto { Id = 1, Status = OrderStatus.Delivered.ToString() };

            _orderRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(order);

            _mapperMock.Setup(m => m.Map<OrderDto>(order))
                .Returns(orderDto);

            //Act 
            var res = await _orderService.UpdateOrderStatusAsync(1, OrderStatus.Delivered.ToString());

            //Assert 
            res.Should().NotBeNull();
            Assert.NotNull(order.DeliveredAt);
            Assert.Equal(OrderStatus.Delivered, order.Status);

        }

        [Fact]
        public async Task UpdateOrderStatusAsync_Should_Restore_Stock_When_Refunded()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                Status = OrderStatus.Shipped,
                OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 5,
                    Quantity = 2
                }
            }
            };

            var product = new Product
            {
                Id = 5,
                StockQuantity = 3,
                SoldQuantity = 5
            };

            _orderRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(order);

            _productRepoMock.Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(product);

            _mapperMock.Setup(m => m.Map<OrderDto>(order))
                .Returns(new OrderDto());

            //Act 
            var res = await _orderService.UpdateOrderStatusAsync(1, OrderStatus.Refunded.ToString());

            //Assert
            Assert.Equal(5, product.StockQuantity);
            Assert.Equal(3, product.SoldQuantity);
        }

        [Fact]
        public async Task CancelOrderAsync_Should_Throw_When_OrderDoesNotExist()
        {
            // Arrange
            _orderRepoMock
                .Setup(x => x.GetOrderWithItemsAsync(1))
                .ReturnsAsync((Order?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _orderService.CancelOrderAsync(1, 10));
        }

        [Fact]
        public async Task CancelOrderAsync_Should_Throw_When_OrderIsDelivered()
        {
            //Arrange
            var order = new Order { Id = 1, CustomerId = 5, Status = OrderStatus.Delivered };

            _orderRepoMock.Setup(x => x.GetOrderWithItemsAsync(1))
                .ReturnsAsync(order);

            //Act & Assert 
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.CancelOrderAsync(1, 5));
        }

        [Fact]
        public async Task CancelOrderAsync_Should_Throw_When_OrderAlreadyCancelled()
        {
            //Arrange
            var order = new Order { Id = 1, CustomerId = 5, Status = OrderStatus.Cancelled };

            _orderRepoMock.Setup(x => x.GetOrderWithItemsAsync(1))
                .ReturnsAsync(order);

            //Act & Assert 
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.CancelOrderAsync(1, 5));
        }

        [Fact]
        public async Task CancelOrderAsync_Should_Throw_When_UserIsNotOrderOwner()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                CustomerId = 10,
                Status = OrderStatus.Pending
            };

            _orderRepoMock
                .Setup(x => x.GetOrderWithItemsAsync(1))
                .ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _orderService.CancelOrderAsync(1, 11));
        }

        [Fact]
        public async Task CancelOrderAsync_Should_RestoreStock_And_CancelOrder()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                CustomerId = 10,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 5,
                    Quantity = 2
                }
            }
            };

            var product = new Product
            {
                Id = 5,
                StockQuantity = 3,
                SoldQuantity = 5
            };

            _orderRepoMock
                .Setup(x => x.GetOrderWithItemsAsync(1))
                .ReturnsAsync(order);

            _productRepoMock
                .Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(product);

            // Act
            await _orderService.CancelOrderAsync(1, 10);

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);

            Assert.Equal(5, product.StockQuantity);
            Assert.Equal(3, product.SoldQuantity);

            _productRepoMock.Verify(
                x => x.Update(product),
                Times.Once);

            _orderRepoMock.Verify(
                x => x.Update(order),
                Times.Once);
        }

        [Fact]
        public async Task OrderExistsAsync_Should_Return_True_When_OrderExists()
        {
            // Arrange
            _orderRepoMock
                .Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _orderService.OrderExistsAsync(1);

            // Assert
            Assert.True(result);

            _orderRepoMock.Verify(
                x => x.ExistsAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task OrderExistsAsync_Should_Return_False_When_OrderDoesNotExist()
        {
            // Arrange
            _orderRepoMock
                .Setup(x => x.ExistsAsync(1))
                .ReturnsAsync(false);

            // Act
            var result = await _orderService.OrderExistsAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetTotalSalesAsync_Should_Return_TotalSales()
        {
            //Arrange 
            _orderRepoMock.Setup(x => x.GetTotalSalesAsync(1))
                .ReturnsAsync(5000m);

            //Act 
            var res = await _orderService.GetTotalSalesAsync(1);

            //Assert 
            Assert.Equal(5000, res);
        }

        [Fact]
        public async Task GetOrderCountAsync_Should_Return_OrderCount()
        {
            // Arrange
            _orderRepoMock
                .Setup(x => x.GetOrderCountAsync(null))
                .ReturnsAsync(20);

            // Act
            var result = await _orderService.GetOrderCountAsync();

            // Assert
            Assert.Equal(20, result);
        }

        [Fact]
        public async Task GetOrderCountAsync_Should_Pass_SellerId_To_Repository()
        {
            // Arrange
            _orderRepoMock
                .Setup(x => x.GetOrderCountAsync(5))
                .ReturnsAsync(7);

            // Act
            var result = await _orderService.GetOrderCountAsync(5);

            // Assert
            Assert.Equal(7, result);

            _orderRepoMock.Verify(
                x => x.GetOrderCountAsync(5),
                Times.Once);
        }
    }
}
