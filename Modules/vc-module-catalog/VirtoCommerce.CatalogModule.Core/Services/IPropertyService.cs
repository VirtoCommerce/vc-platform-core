using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IPropertyService
    {

        Property GetById(string propertyId);
        Property[] GetByIds(string[] propertyIds);
        Property Create(Property property);
        void Update(Property[] properties);
        void Delete(string[] propertyIds);
        Property[] GetAllCatalogProperties(string catalogId);
        Property[] GetAllProperties();
    }
}
