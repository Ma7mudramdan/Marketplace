
using Marketplace.Application.DTOs.Users;

namespace Marketplace.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public DateTime TokenExpiry { get; set; }
        public string? RefreshToken { get; set; }
        public UserDto? User { get; set; }

        public List<string> Errors { get; set; }
    }
}