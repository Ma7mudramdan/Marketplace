using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }

        // SELF-REFERENCING PROPERTIES

        // 1. Foreign Key (Nullable because parent categories don't have a parent)
        public int? ParentCategoryId { get; set; }

        // 2. Reference to PARENT (The category this category belongs to)
        public virtual Category? ParentCategory { get; set; }

        // 3. Collection of CHILDREN (Categories that belong to this category)
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
        
  
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
