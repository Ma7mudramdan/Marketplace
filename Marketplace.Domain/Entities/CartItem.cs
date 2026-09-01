
namespace Marketplace.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        
        public int ShoppingCartId { get; set; }
        public int ProductId { get; set; }

        
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;

      
        public decimal TotalPrice => Quantity * Product.Price;
    }
}