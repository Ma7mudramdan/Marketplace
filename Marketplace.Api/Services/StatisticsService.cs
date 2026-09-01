using Marketplace.Api.Services.Interfaces;
using Marketplace.Application.DTOs.Statistics;
using Marketplace.Domain.Entities;
using Marketplace.Domain.Entities.Enums;
using Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StatisticsService> _logger;

        public StatisticsService(
            AppDbContext context,
            ILogger<StatisticsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<DailySalesDto>> GetDailySalesAsync(int days)
        {
            try
            {
                var startDate = DateTime.UtcNow.Date.AddDays(-days);
                var orders = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered && o.OrderDate >= startDate)
                    .ToListAsync();

                var dailySales = new List<DailySalesDto>();
                for (int i = 0; i < days; i++)
                {
                    var date = startDate.AddDays(i);
                    var dayOrders = orders.Where(o => o.OrderDate.Date == date);
                    dailySales.Add(new DailySalesDto
                    {
                        Date = date,
                        Revenue = dayOrders.Sum(o => o.TotalAmount),
                        Orders = dayOrders.Count(),
                        ItemsSold = dayOrders.Sum(o => o.OrderItems.Sum(i => i.Quantity))
                    });
                }

                return dailySales;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily sales");
                throw;
            }
        }

        public async Task<ProductStatisticsDto> GetProductStatisticsAsync()
        {
            try
            {
                var totalProducts = await _context.Products.CountAsync();
                var activeProducts = await _context.Products.CountAsync(p => p.IsActive);
                var inactiveProducts = totalProducts - activeProducts;
                var featuredProducts = await _context.Products.CountAsync(p => p.IsFeatured);
                var pendingApproval = await _context.Products.CountAsync(p => !p.IsApproved && p.IsActive);
                var outOfStock = await _context.Products.CountAsync(p => p.StockQuantity == 0 && p.IsActive);
                var lowStock = await _context.Products.CountAsync(p => p.StockQuantity > 0 && p.StockQuantity <= 10 && p.IsActive);
                var categories = await _context.Categories.CountAsync(c => c.IsActive);

                return new ProductStatisticsDto
                {
                    TotalProducts = totalProducts,
                    ActiveProducts = activeProducts,
                    InactiveProducts = inactiveProducts,
                    FeaturedProducts = featuredProducts,
                    PendingApproval = pendingApproval,
                    OutOfStock = outOfStock,
                    LowStock = lowStock,
                    Categories = categories
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product statistics");
                throw;
            }
        }

        public async Task<SalesStatisticsDto> GetSalesStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var query = _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered);

                if(startDate.HasValue)
                    query = query.Where(o => o.OrderDate >= startDate);

                if(endDate.HasValue)
                    query = query.Where(o => o.OrderDate <= endDate);

                var orders = await query.ToListAsync();
                var totalRevenue = orders.Sum(o => o.TotalAmount);
                var totalOrders = orders.Count();
                var totalItems = orders.Sum(o => o.OrderItems.Sum(oi => oi.Quantity));
                var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

                return new SalesStatisticsDto
                {
                    TotalOrders = totalOrders,
                    TotalRevenue = totalRevenue,
                    AverageOrderValue = averageOrderValue,
                    TotalItemsSold = totalItems,
                    RevenueGrowth = CalculateRevenueGrowth(),
                    OrdersGrowth = CalculateOrdersGrowth()
                };



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales statistics");
                throw;
            }
        }


        public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync(int count)
        {
            try
            {
                var topProducts = await _context.OrderItems
                    .Include(oi => oi.Product)
                        .ThenInclude(p => p.Reviews)
                    .Where(oi => oi.Order.Status == OrderStatus.Delivered)
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalSold = g.Sum(oi => oi.Quantity),
                        TotalRevenue = g.Sum(oi => oi.TotalPrice)
                    })
                    .OrderBy(g => g.TotalSold)
                    .Take(count)
                    .ToListAsync();

                var result = new List<TopProductDto>();
                foreach(var item in topProducts)
                {
                    var product = await _context.Products
                        .Include(p => p.Reviews)
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        result.Add(new TopProductDto
                        {
                            ProductId = item.ProductId,
                            ProductName = product.Name,
                            TotalSold = item.TotalSold,
                            TotalRevenue = item.TotalRevenue,
                            AverageRating =(decimal) product.AverageRating,
                            ReviewCount = product.ReviewCount
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top products");
                throw;
            }
        }

        public async Task<IEnumerable<TopSellerDto>> GetTopSellersAsync(int count)
        {
            try
            {
                var topSellers = await _context.Products
                    .Where(p => p.IsActive)
                    .GroupBy(p => p.SellerId)
                    .Select(g => new
                    {
                        SellerId = g.Key,
                        TotalProducts = g.Count(),
                        TotalSold = g.Sum(p => p.SoldQuantity),
                        TotalRevenue = g.Sum(p => p.Price * p.SoldQuantity)
                    })
                    .OrderByDescending(g => g.TotalSold)
                    .Take(count)
                    .ToListAsync();

                var result = new List<TopSellerDto>();
                foreach (var item in topSellers)
                {
                    var seller = await _context.Users
                        .Include(u => u.Products)
                        .ThenInclude(p => p.Reviews)
                        .FirstOrDefaultAsync(u => u.Id == item.SellerId);

                    if (seller != null)
                    {
                        var avgRating = seller.Products
                            .SelectMany(p => p.Reviews)
                            .Where(r => r.IsApproved)
                            .Average(r => (double?)r.Rating) ?? 0;

                        result.Add(new TopSellerDto
                        {
                            SellerId = item.SellerId,
                            SellerName = $"{seller.FirstName} {seller.LastName}",
                            TotalProducts = item.TotalProducts,
                            TotalSold = item.TotalSold,
                            TotalRevenue = item.TotalRevenue,
                            AverageRating = (decimal) avgRating
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top sellers");
                throw;
            }
        }

        public async Task<UserStatisticsDto> GetUserStatisticsAsync()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
                var inactiveUsers = totalUsers - activeUsers;
                var sellers = await _context.Users.Where(u => u.Products.Any()).CountAsync();
                var customers = await _context.Users.Where(u => u.Orders.Any()).CountAsync();

                var today = DateTime.UtcNow.Date;
                var weekStart = today.AddDays(-7);
                var monthStart = today.AddDays(-30);

                var newUsersToday = await _context.Users
                    .CountAsync(u => u.CreatedAt >= today);

                var newUsersThisWeek = await _context.Users
                    .CountAsync(u => u.CreatedAt >= weekStart);

                var newUsersThisMonth = await _context.Users
                    .CountAsync(u => u.CreatedAt >= monthStart);

                return new UserStatisticsDto
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    InactiveUsers = inactiveUsers,
                    Sellers = sellers,
                    Customers = customers,
                    Admins = 0, // Will get from Identity
                    NewUsersToday = newUsersToday,
                    NewUsersThisWeek = newUsersThisWeek,
                    NewUsersThisMonth = newUsersThisMonth
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user statistics");
                throw;
            }
        }
        private int CalculateOrdersGrowth()
        {
            return 0;
        }

        private decimal CalculateRevenueGrowth()
        {
            return 0;
        }
    }
}
