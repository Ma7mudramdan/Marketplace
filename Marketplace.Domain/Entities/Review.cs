
namespace Marketplace.Domain.Entities
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }

        
        public int ProductId { get; set; }
        public int UserId { get; set; }

        
        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}