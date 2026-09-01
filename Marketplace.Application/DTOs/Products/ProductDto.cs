
using Marketplace.Domain.Entities.Enums;

namespace Marketplace.Application.DTOs.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public ProductCondition Condition { get; set; }
        public string? Location { get; set; }
        public int Views { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Related Data
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public List<ProductImageDto> Images { get; set; } = new();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsInStock => StockQuantity > 0;
    }
}
