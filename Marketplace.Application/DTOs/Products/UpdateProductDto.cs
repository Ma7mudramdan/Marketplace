
using System.ComponentModel.DataAnnotations;
using Marketplace.Domain.Entities.Enums;

namespace Marketplace.Application.DTOs.Products
{
    public class UpdateProductDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [Range(0, 999999.99)]
        public decimal? DiscountedPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required]
        public ProductCondition Condition { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
    }
}