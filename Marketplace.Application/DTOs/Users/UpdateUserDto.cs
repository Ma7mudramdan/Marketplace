
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Application.DTOs.Users
{
    public class UpdateUserDto
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }

        [Phone]
        [RegularExpression(@"^(010|011|012|015)[0-9]{8}$",
            ErrorMessage = "Please enter valild Number ")]
        public string? PhoneNumber { get; set; }
    }
}