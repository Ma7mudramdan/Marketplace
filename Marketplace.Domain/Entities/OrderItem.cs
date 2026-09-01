// Marketplace.Domain/Entities/OrderItem.cs
namespace Marketplace.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? DiscountApplied { get; set; }

       
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}