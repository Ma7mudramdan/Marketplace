
namespace Marketplace.Application.DTOs.ShoppingCart
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductFinalPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int MaxStock { get; set; }
        public DateTime AddedAt { get; set; }
    }
}