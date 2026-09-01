using Marketplace.Domain.Entities;

namespace Marketplace.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId);
        Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
        Task<bool> HasSubCategoriesAsync(int categoryId);
        Task<IEnumerable<Category>> GetCategoryHierarchyAsync();
    }
}
