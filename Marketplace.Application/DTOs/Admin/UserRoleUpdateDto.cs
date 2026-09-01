
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Application.DTOs.Admin
{
    public class UserRoleUpdateDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}