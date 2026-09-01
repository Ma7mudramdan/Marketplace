
namespace Marketplace.Domain.Entities
{
    public class Favorite : BaseEntity
    {
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        
        public int UserId { get; set; }
        public int ProductId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}