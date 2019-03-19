using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IPropertySearchService
    {
        GenericSearchResult<Property> SearchProperties(PropertySearchCriteria criteria);
        GenericSearchResult<PropertyDictionaryValue> SearchPropertyDictionaryValues(PropertyDictionaryValueSearchCriteria criteria);
    }
}
