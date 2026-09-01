
using Marketplace.Domain.Entities.Enums;

namespace Marketplace.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; } = 0;
        public ProductCondition Condition { get; set; } = ProductCondition.New;
        public string? Location { get; set; }
        public int Views { get; set; } = 0;
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        
        // Concurrency Token
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        
        // Foreign Keys
        public int CategoryId { get; set; }
        public int SellerId { get; set; }
        
        // Navigation Properties
        public virtual Category Category { get; set; } = null!;
        public virtual User Seller { get; set; } = null!;
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        
        // Computed Properties
        public decimal FinalPrice => DiscountedPrice ?? Price;
        public bool IsInStock => StockQuantity > 0;
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
        public int ReviewCount => Reviews.Count;
    }
}