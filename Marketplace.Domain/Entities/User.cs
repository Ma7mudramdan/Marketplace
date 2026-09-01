using Marketplace.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime LastLoginAt { get; set; }
        public bool IsEmailConfirmed { get; set; }

        public string? IdentityUserId { get; set; }
        public virtual IIdentityUser? IdentityUser { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ShoppingCart? ShoppingCart { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
