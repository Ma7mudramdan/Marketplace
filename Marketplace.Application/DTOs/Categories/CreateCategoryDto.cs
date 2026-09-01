
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Application.DTOs.Categories
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Icon { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public int? ParentCategoryId { get; set; }
    }
}