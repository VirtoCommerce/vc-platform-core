using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;
namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetByIds(IEnumerable<string> categoryIds, string responseGroup = null, string catalogId = null);
        void SaveChanges(IEnumerable<Category> categories);
		void Delete(IEnumerable<string> categoryIds);
    }
}
