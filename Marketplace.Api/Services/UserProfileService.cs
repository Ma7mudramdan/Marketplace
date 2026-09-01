using AutoMapper;
using Marketplace.Application.DTOs.Users;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Infrastructure.Data;
using Marketplace.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Api.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IFileUploadService fileUploadService,
            IMapper mapper,
            ILogger<UserProfileService> logger)
        {
            _context = context;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> DeleteProfilePictureAsync(string identityUserId)
        {
            try
            {
                var identityUser = await _userManager.FindByIdAsync(identityUserId);

                if (identityUser == null)
                    throw new ArgumentException("User not found");

                if(!string.IsNullOrEmpty(identityUser.ProfilePictureUrl))
                 {
                    await _fileUploadService.DeleteFileAsync(identityUser.ProfilePictureUrl);
                    identityUser.ProfilePictureUrl = null;
                    await _userManager.UpdateAsync(identityUser);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile picture for {IdentityUserId}", identityUserId);
                return false;
            }
        }

        public async Task<UserDto> GetUserProfileAsync(string identityUserId)
        {
            try
            {

                var identityUser = await _userManager.FindByIdAsync(identityUserId);

                if (identityUser == null)
                    throw new ArgumentException("User not found");

                var businessUser = await _context.Users
                    .Include(u => u.Products)
                    .Include(u => u.Orders)
                    .Include(u => u.Reviews)
                    .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

                if (businessUser == null)
                    throw new ArgumentException("User business not found");

                return new UserDto
                {
                    Id = businessUser.Id,
                    FirstName = businessUser.FirstName,
                    LastName = businessUser.LastName,
                    Email = businessUser.Email,
                    PhoneNumber = businessUser.PhoneNumber,
                    Bio = businessUser.Bio,
                    Address = businessUser.Address,
                    City = businessUser.City,
                    Country = businessUser.Country,
                    ProfilePictureUrl = identityUser.ProfilePictureUrl,
                    CreatedAt = businessUser.CreatedAt,
                    IsActive = businessUser.IsActive,
                    ProductCount = businessUser.Products.Count,
                    OrderCount = businessUser.Orders.Count,
                    ReviewCount = businessUser.Reviews.Count
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile for {IdentityUserId}", identityUserId);
                throw;
            }
        }

        public async Task<bool> UpdateEmailAsync(string identityUserId, string newEmail)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(identityUserId);

                if (user == null)
                    throw new ArgumentException("User not found");

                user.Email = newEmail;
                user.UserName = newEmail;
                await _userManager.UpdateAsync(user);

                var businessUser = await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

                if (businessUser != null)
                {
                    businessUser.Email = newEmail;
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating email for {IdentityUserId}", identityUserId);
                return false;
            }
        }

        public async Task<bool> UpdatePhoneAsync(string identityUserId, string phoneNumber)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(identityUserId);

                if (user == null)
                    throw new ArgumentException("user not found");

                user.PhoneNumber = phoneNumber;
                await _userManager.UpdateAsync(user);

                var bUser = await _context.Users.FirstOrDefaultAsync(u =>u.IdentityUserId == identityUserId);

                if (bUser != null)
                {
                    bUser.PhoneNumber = phoneNumber;
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating phone for {IdentityUserId}", identityUserId);
                return false;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(string identityUserId, UpdateUserDto updateDto)
        {
            try
            {
                var identityUser = await _userManager.FindByIdAsync(identityUserId);
                if (identityUser == null)
                    return false;

                var businessUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

                if (businessUser == null)
                    return false;

                // Update Identity user

                if (!string.IsNullOrEmpty(updateDto.FirstName))
                    identityUser.FirstName = updateDto.FirstName;

                if (!string.IsNullOrEmpty(updateDto.LastName))
                    identityUser.LastName = updateDto.LastName;

                if (!string.IsNullOrEmpty(updateDto.PhoneNumber))
                    identityUser.PhoneNumber = updateDto.PhoneNumber;

                // Update Business User
                if (!string.IsNullOrEmpty(updateDto.Bio))
                    businessUser.Bio = updateDto.Bio;

                if (!string.IsNullOrEmpty(updateDto.Address))
                    businessUser.Address = updateDto.Address;

                if (!string.IsNullOrEmpty(updateDto.City))
                    businessUser.City = updateDto.City;

                if (!string.IsNullOrEmpty(updateDto.Country))
                    businessUser.Country = updateDto.Country;

                businessUser.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Profile updated for user {IdentityUserId}", identityUserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for {IdentityUserId}", identityUserId);
                return false;
            }
        }

        public async Task<string> UploadProfilePictureAsync(string identityUserId, IFormFile file)
        {
            try
            {
                var identityUser = await _userManager.FindByIdAsync(identityUserId);

                if (identityUser == null)
                    throw new ArgumentException("User not found");

                if (!string.IsNullOrEmpty(identityUser.ProfilePictureUrl))
                    await _fileUploadService.DeleteFileAsync(identityUser.ProfilePictureUrl);

                var filePath = await _fileUploadService.UploadFileAsync(file, "profile");

                identityUser.ProfilePictureUrl = filePath;

                await _userManager.UpdateAsync(identityUser);

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture for {IdentityUserId}", identityUserId);
                throw;
            }
        }
    }
}
