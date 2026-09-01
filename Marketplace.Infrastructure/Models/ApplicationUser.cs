using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Infrastructure.Models
{
    public class ApplicationUser : IdentityUser, IIdentityUser
    {
        // Personal Information
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Account Info
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsEmailConfirmed { get; set; }

        // 🔗 Link to Business User (Domain Entity)
        public int? BusinessUserId { get; set; }
        public virtual User? UserProfile { get; set; }

        // Computed Property    
        public string FullName => $"{FirstName} {LastName}";

        string IIdentityUser.Id => Id;

        string? IIdentityUser.Email => Email;

        string? IIdentityUser.UserName => UserName;
    }
}
