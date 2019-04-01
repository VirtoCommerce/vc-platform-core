using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IPropertyService
    {
        Task<IEnumerable<Property>> GetByIdsAsync(IEnumerable<string> ids);
        Task<IEnumerable<Property>> GetAllCatalogPropertiesAsync(string catalogId);
        Task SaveChangesAsync(IEnumerable<Property> properties);
        Task DeleteAsync(IEnumerable<string> ids, bool doDeleteValues = false);
    }
}
