using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICategoryService
    {
        Task<Category[]> GetByIdsAsync(string[] categoryIds, CategoryResponseGroup responseGroup, string catalogId = null);
        Task<Category> GetByIdAsync(string categoryId, CategoryResponseGroup responseGroup, string catalogId = null);
        Task CreateAsync(Category[] categories);
        Task<Category> CreateAsync(Category category);
        Task UpdateAsync(Category[] categories);
        Task DeleteAsync(string[] categoryIds);
    }
}
