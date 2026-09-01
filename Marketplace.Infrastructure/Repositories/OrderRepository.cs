using Marketplace.Domain.Entities;
using Marketplace.Domain.Entities.Enums;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {

        public OrderRepository(AppDbContext context ) : base( context ) { }
       
        public async Task<int> GetOrderCountAsync(int? sellerId = null)
        {
            var query = _dbSet
                .Where(o => o.Status == OrderStatus.Delivered);

            if(sellerId.HasValue)
            {
                query = query
                    .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == sellerId.Value));
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Include(o => o.OrderItems)
                               .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                               .OrderByDescending(o => o.OrderDate)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _dbSet.Include(o => o.OrderItems)
                               .Where(o => o.Status.ToString() == status)
                               .OrderByDescending(o => o.OrderDate)
                               .ToListAsync();
        }

        public async Task<Order?> GetOrderWithItemsAsync(int orderId)
        {
            var order = await _dbSet.Include(o => o.OrderItems)
                                 .ThenInclude(oi => oi.Product)
                              .FirstOrDefaultAsync(o => o.Id == orderId);
            return order;
        }

        public async Task<decimal> GetTotalSalesAsync(int? sellerId = null)
        {
            var query = _dbSet
                 .Where(o => o.Status == OrderStatus.Delivered);

            if (sellerId.HasValue)
            {
                query = query
                    .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == sellerId.Value));
            }

            return await query.SumAsync(o => o.TotalAmount);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            return  await _context.Orders.Include(o => o.OrderItems)
                                  .ThenInclude(oi => oi.Product)
                                .Where(o => o.CustomerId == userId)
                                .OrderByDescending(o => o.OrderDate)
                                .ToArrayAsync();

        }
    }
}
