
using AutoMapper;
using Marketplace.Api.Services.Interfaces;
using Marketplace.Application.DTOs.Admin;
using Marketplace.Application.DTOs.Email;
using Marketplace.Application.DTOs.Pagination;
using Marketplace.Application.DTOs.Products;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Domain.Entities.Enums;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Infrastructure.Data;
using Marketplace.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Marketplace.Api.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;
        private readonly IEmailService _emailService;
        public AdminService(
         AppDbContext context,
         UserManager<ApplicationUser> userManager,
         RoleManager<IdentityRole> roleManager,
         IProductRepository productRepository,
         IOrderRepository orderRepository,
         IEmailService emailService,
         IMapper mapper,
         ILogger<AdminService> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
        }


        public async Task<bool> ApproveProductAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return false;
                }

                product.IsApproved = true;
                product.ApprovedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await _emailService.SendEmailAsync(new EmailDto
                {
                    To = product.Seller.Email,
                    Subject = "Your Product Has Been Approved!",
                    Body = $@"
                            <h1>Congratulations!</h1>
                            <p>Your product <strong>{product.Name}</strong> has been approved and is now live on the marketplace.</p>
                            <p><a href='https://yourdomain.com/products/{product.Id}'>View Product</a></p>",
                             
                    IsHtml = true
                });
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving product");
                throw;
            }
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            try
            {
                var totalUsers = await _userManager.Users.CountAsync();
                var totalSellers = await _userManager.GetUsersInRoleAsync("Seller");
                var totalProducts = await _productRepository.CountAsync();
                var totalOrders = await _orderRepository.GetOrderCountAsync();
                var totalRevenue = await _orderRepository.GetTotalSalesAsync();

                var pendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);

                var pendingReviews = await _context.Reviews
                    .CountAsync(r => !r.IsApproved);

                var featuredProducts = await _context.Products
                    .CountAsync(p => p.IsFeatured && p.IsActive);

                return new DashboardStatsDto
                {
                    TotalUsers = totalUsers,
                    TotalSellers = totalSellers.Count,
                    TotalProducts = totalProducts,
                    TotalOrders = totalOrders,
                    TotalRevenue = totalRevenue,
                    PendingOrders = pendingOrders,
                    PendingReviews = pendingReviews,
                    FeaturedProducts = featuredProducts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                throw;
            }
           
        }

        public async Task<IEnumerable<ProductDto>> GetPendingProductsAsync()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Seller)
                    .Include(p => p.Images)
                    .Where(p => !p.IsApproved && p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending products");
                throw;
            }
        }
        public async Task<PaginatedResultDto<UserListDto>> GetUsersAsync(
                   int page, int pageSize, string? search, string? role)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                // Search
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u =>
                        u.Email.Contains(search) ||
                        u.FirstName.Contains(search) ||
                        u.LastName.Contains(search));
                }

                // Filter by role
                if (!string.IsNullOrEmpty(role))
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                    var userIds = usersInRole.Select(u => u.Id);
                    query = query.Where(u => userIds.Contains(u.Id));
                }

                var totalCount = await query.CountAsync();

                var users = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var userDtos = new List<UserListDto>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var businessUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.IdentityUserId == user.Id);

                    userDtos.Add(new UserListDto
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        Roles = roles.ToList(),
                        IsActive = user.IsActive,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt,
                        ProductCount = businessUser?.Products?.Count ?? 0,
                        OrderCount = businessUser?.Orders?.Count ?? 0
                    });
                }

                return new PaginatedResultDto<UserListDto>
                {
                    Items = userDtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                throw;
            }
        }
        public async Task<bool> ToggleProductFeatureAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return false;
                }

                product.IsFeatured = !product.IsFeatured;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling product feature");
                throw;
            }
        }

        public async Task<bool> ToggleUserActivationAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user activation");
                throw;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(string userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                await _userManager.AddToRoleAsync(user, role);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role");
                throw;
            }
        }
    }
}
