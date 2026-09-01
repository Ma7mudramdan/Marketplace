
namespace Marketplace.Application.DTOs.Products
{
    public class ProductSearchDto
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Condition { get; set; }
        public string? SortBy { get; set; } = "created";
        public bool Ascending { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsFeatured { get; set; }
        public bool? InStock { get; set; }
    }
}