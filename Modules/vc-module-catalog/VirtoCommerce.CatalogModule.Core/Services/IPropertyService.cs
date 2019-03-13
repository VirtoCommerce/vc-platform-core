using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IPropertyService
    {

        Task<Property[]> GetByIdsAsync(string[] propertyIds);
        Task<Property[]> GetAllCatalogPropertiesAsync(string catalogId);
        Task<Property[]> GetAllPropertiesAsync();
        Task SaveChangesAsync(Property[] properties);
        Task DeleteAsync(string[] propertyIds);
        Task DeletePropertyValuesByPropertyIdAsync(string propertyId);
    }
}
