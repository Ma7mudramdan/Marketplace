using Marketplace.Domain.Entities;
using Marketplace.Domain.Interfaces.Repositories;
using Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Marketplace.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }
        
        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
           return await _context.Categories
                                .Where(c => c.IsActive && c.ParentCategoryId == null )
                                .Include(c => c.SubCategories.Where(sc => sc.IsActive))
                                .OrderByDescending(c => c.DisplayOrder)
                                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync()
        {
            return await _dbSet.Where(c => c.IsActive)
                               .Include(c => c.Products.Where(sc => sc.IsActive))
                               .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoryHierarchyAsync()
        {
            var categories = await _dbSet.Where(c => c.IsActive && c.ParentCategoryId == null)
                                         .Include(c => c.SubCategories)
                                         .ToListAsync();
            return categories;
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId)
        {
            return await _dbSet.Where(c => c.ParentCategoryId == parentId && c.IsActive).ToListAsync();
        }

        public async Task<bool> HasSubCategoriesAsync(int categoryId)
        {
            return await _dbSet
                         .AnyAsync(c => c.ParentCategoryId == categoryId && c.IsActive);
        }
    }
}
