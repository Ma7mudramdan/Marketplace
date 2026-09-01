
namespace Marketplace.Domain.Entities
{
    public class ShoppingCart : BaseEntity
    {
   
        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();

       
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
        public int ItemsCount => Items.Sum(i => i.Quantity);
    }
}