
using Marketplace.Api.Services;
using Marketplace.Domain.Entities;
using Marketplace.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Marketplace.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly AppDbContext _context;
        protected readonly ILogger _logger;

        protected BaseApiController(AppDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        
        protected string? GetIdentityUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        
        protected async Task<int> GetBusinessUserIdAsync()
        {
            var identityUserId = GetIdentityUserId();
            if (string.IsNullOrEmpty(identityUserId))
            {
                return 0;
            }

            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

            return user?.Id ?? 0;
        }

       
        protected async Task<User?> GetBusinessUserAsync()
        {
            var identityUserId = GetIdentityUserId();
            if (string.IsNullOrEmpty(identityUserId))
            {
                return null;
            }

            return await _context.Users
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
        }

       
        protected bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }

        
        protected List<string> GetUserRoles()
        {
            return User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }
    }
}