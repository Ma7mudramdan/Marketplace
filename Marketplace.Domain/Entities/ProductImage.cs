namespace Marketplace.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public long FileSize { get; set; }
        public string? FileName { get; set; }

        // Foreign Keys
        public int ProductId { get; set; }

        // Navigation Properties
        public virtual Product Product { get; set; } = null!;
    }
}