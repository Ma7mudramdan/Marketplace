
using System.ComponentModel.DataAnnotations;
using Marketplace.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace Marketplace.Application.DTOs.Products
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999,999.99")]
        public decimal Price { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Discounted price must be between 0 and 999,999.99")]
        public decimal? DiscountedPrice { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Condition is required")]
        public ProductCondition Condition { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public bool IsFeatured { get; set; }

        // Image Upload
        public List<IFormFile>? Images { get; set; }
    }
}