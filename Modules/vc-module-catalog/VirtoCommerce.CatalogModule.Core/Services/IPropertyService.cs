using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IPropertyService
    {

        Task<Property> GetByIdAsync(string propertyId);
        Task<Property[]> GetByIdsAsync(string[] propertyIds);
        Task<Property> CreateAsync(Property property);
        Task UpdateAsync(Property[] properties);
        Task DeleteAsync(string[] propertyIds);
        Task<Property[]> GetAllCatalogPropertiesAsync(string catalogId);
        Task<Property[]> GetAllPropertiesAsync();
    }
}
