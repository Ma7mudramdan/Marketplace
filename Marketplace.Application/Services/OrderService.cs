// Marketplace.Application/Services/OrderService.cs
using AutoMapper;
using Microsoft.Extensions.Logging;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Entities.Enums;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Application.DTOs.Orders;
using Marketplace.Application.Interfaces.Services;

namespace Marketplace.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly IEmailService _emailService;
        

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IShoppingCartRepository shoppingCartRepository,
            IEmailService emailService,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetOrderWithItemsAsync(id);
                return order != null ? _mapper.Map<OrderDto>(order) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order by id: {OrderId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
        {
            try
            {
                var orders = await _orderRepository.GetUserOrdersAsync(userId);
                orders = orders.Where(o => o.Status != OrderStatus.Cancelled);
                return _mapper.Map<IEnumerable<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByStatusAsync(status);
                return _mapper.Map<IEnumerable<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders by status: {Status}", status);
                throw;
            }
        }

        
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, int userId)
        {
            // Begin transaction (without using)
            var transaction = await _orderRepository.BeginTransactionAsync();

            try
            {
                // 1. Get user's shopping cart
                var cart = await _shoppingCartRepository.GetCartWithItemsAsync(userId);
                if (cart == null || !cart.Items.Any())
                {
                    throw new InvalidOperationException("Shopping cart is empty");
                }

                // 2. Validate stock availability
                foreach (var cartItem in cart.Items)
                {
                    var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                    if (product == null)
                    {
                        throw new ArgumentException($"Product {cartItem.ProductId} not found");
                    }

                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product: {product.Name}");
                    }
                }

                // 3. Create order
                var order = new Order
                {
                    CustomerId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    PaymentStatus = PaymentStatus.Pending,
                    ShippingAddress = createDto.ShippingAddress,
                    ShippingCity = createDto.ShippingCity,
                    ShippingCountry = createDto.ShippingCountry,
                    PaymentMethod = createDto.PaymentMethod,
                    Notes = createDto.Notes,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                // 4. Create order items and calculate totals
                decimal subtotal = 0;
                decimal discountAmount = 0;

                foreach (var cartItem in cart.Items)
                {
                    var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                    if (product == null) continue;

                    var finalPrice = product.DiscountedPrice ?? product.Price;
                    var itemTotal = finalPrice * cartItem.Quantity;

                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = finalPrice,
                        TotalPrice = itemTotal,
                        DiscountApplied = product.DiscountedPrice.HasValue
                            ? (product.Price - product.DiscountedPrice.Value) * cartItem.Quantity
                            : 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    order.OrderItems.Add(orderItem);
                    subtotal += itemTotal;
                    discountAmount += orderItem.DiscountApplied ?? 0;
                    
                    // 5. Update stock
                    product.StockQuantity -= cartItem.Quantity;
                    product.SoldQuantity += cartItem.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                    _productRepository.Update(product);
                }

                // 6. Calculate totals
                order.Subtotal = subtotal;
                order.DiscountAmount = discountAmount;
                order.TaxAmount = subtotal * 0.14m;
                order.ShippingCost = CalculateShippingCost(order);
                order.TotalAmount = order.Subtotal + order.TaxAmount + order.ShippingCost - order.DiscountAmount;

                // 7. Save order
                await _orderRepository.AddAsync(order);

                // 8. Clear shopping cart
                await _shoppingCartRepository.ClearCartAsync(userId);

                // 9. Commit transaction
                await _orderRepository.CommitTransactionAsync();

                _logger.LogInformation("Order {OrderId} created successfully for user {UserId}", order.Id, userId);

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                // 10. Rollback on error
                await _orderRepository.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating order for user: {UserId}", userId);
                throw;
            }
        }
        private decimal CalculateShippingCost(Order order)
        {
            // Simple shipping calculation based on total amount
            // Free shipping for orders over $100
            if (order.Subtotal >= 100)
                return 0;

            // Base shipping cost
            return 10.00m;
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                // Validate status transition
                if (!IsValidStatusTransition(order.Status.ToString(), status))
                {
                    throw new InvalidOperationException($"Invalid status transition from {order.Status} to {status}");
                }

                var newStatus = Enum.Parse<OrderStatus>(status);
                order.Status = newStatus;
                order.UpdatedAt = DateTime.UtcNow;

                // Handle specific status changes
                switch (newStatus)
                {
                    case OrderStatus.Shipped:
                        order.ShippedAt = DateTime.UtcNow;
                        break;
                    case OrderStatus.Delivered:
                        order.DeliveredAt = DateTime.UtcNow;
                        break;
                    case OrderStatus.Cancelled:
                        // Restore stock when order is cancelled
                        await RestoreStockForOrderAsync(order);
                        break;
                    case OrderStatus.Refunded:
                        // Restore stock when order is refunded
                        await RestoreStockForOrderAsync(order);
                        break;
                }

                _orderRepository.Update(order);

                _logger.LogInformation("Order {OrderId} status updated to {Status}", orderId, status);

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status: {OrderId}", orderId);
                throw;
            }
        }

        private async Task RestoreStockForOrderAsync(Order order)
        {
            try
            {
                if (order.Status == OrderStatus.Cancelled)
                {
                    _logger.LogWarning("No order items to restore stock for order {OrderId}", order?.Id);
                    return;
                }

                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
                    if (product != null)
                    {
                        // Restore stock
                        product.StockQuantity += orderItem.Quantity;
                        product.UpdatedAt = DateTime.UtcNow;
                        product.SoldQuantity -= orderItem.Quantity;

                        _productRepository.Update(product);
                        _logger.LogInformation("Restored {Quantity} stock for product {ProductId} (Order {OrderId})",
                            orderItem.Quantity, product.Id, order.Id);
                    }
                    else
                    {
                        _logger.LogWarning("Product {ProductId} not found when restoring stock for order {OrderId}",
                            orderItem.ProductId, order.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring stock for order {OrderId}", order?.Id);
                throw;
            }
        }

        private bool IsValidStatusTransition(string currentStatus, string newStatus)
        {
            var validTransitions = new Dictionary<string, string[]>
            {
                ["Pending"] = new[] { "Processing", "Cancelled" },
                ["Processing"] = new[] { "Shipped", "Cancelled" },
                ["Shipped"] = new[] { "Delivered" },
                ["Delivered"] = new[] {""},
                ["Cancelled"] = new[] {""} ,
                ["Refunded"] = new[] {""} ,
            };

            return validTransitions.ContainsKey(currentStatus) &&
                   validTransitions[currentStatus].Contains(newStatus);
        }

        public async Task CancelOrderAsync(int orderId , int userId)
        {
            try
            {
                var order = await _orderRepository.GetOrderWithItemsAsync(orderId);
                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                {
                    throw new InvalidOperationException($"Order cannot be cancelled when status is {order.Status}");
                }

                if (order.CustomerId != userId)
                    throw new ArgumentException();
                // Restore stock
               await RestoreStockForOrderAsync(order);

                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;
                

                _orderRepository.Update(order);

                _logger.LogInformation("Order {OrderId} cancelled", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> OrderExistsAsync(int id)
        {
            return await _orderRepository.ExistsAsync(id);
        }

        public async Task<decimal> GetTotalSalesAsync(int? sellerId = null)
        {
            try
            {
                return await _orderRepository.GetTotalSalesAsync(sellerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total sales for seller: {SellerId}", sellerId);
                throw;
            }
        }

        public async Task<int> GetOrderCountAsync(int? sellerId = null)
        {
            try
            {
                return await _orderRepository.GetOrderCountAsync(sellerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order count for seller: {SellerId}", sellerId);
                throw;
            }
        }

        
    }
}