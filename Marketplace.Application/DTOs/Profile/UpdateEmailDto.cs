
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Application.DTOs.Profile
{
    public class UpdateEmailDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        
    }

    public class UpdatePhoneDto
    {
       

        [Required]
        [RegularExpression(@"^(010|011|012|015)[0-9]{8}$",
            ErrorMessage = "Please enter valild Number ")]
        public string? PhoneNumber { get; set; } 
    }
    public class UploadPictureDto
    {
        [FromForm]
        public IFormFile? File { get; set; }
    }
}
