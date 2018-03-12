using VirtoCommerce.CatalogModule.Core.Model;
namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICategoryService
    {
        Category[] GetByIds(string[] categoryIds, string responseGroup = null, string catalogId = null);
        void SaveChanges(Category[] categories);
		void Delete(string[] categoryIds);
    }
}
