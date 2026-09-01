
using Marketplace.Application.DTOs.Users;
namespace Marketplace.Application.Interfaces.Services
{
    public interface IUserProfileService
    {
        Task<UserDto> GetUserProfileAsync(string identityUserId);
        Task<bool> UpdateUserProfileAsync(string identityUserId, UpdateUserDto updateDto);
        Task<string> UploadProfilePictureAsync(string identityUserId, IFormFile file);
        Task<bool> DeleteProfilePictureAsync(string identityUserId);
        Task<bool> UpdateEmailAsync(string identityUserId, string newEmail);
        Task<bool> UpdatePhoneAsync(string identityUserId, string phoneNumber);
    }
}