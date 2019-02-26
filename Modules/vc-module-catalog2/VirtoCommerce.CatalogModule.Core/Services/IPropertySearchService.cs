using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.CatalogModule.Core2.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core2.Services
{
    public interface IPropertySearchService
    {
        GenericSearchResult<Property> SearchProperties(PropertySearchCriteria criteria);
        GenericSearchResult<PropertyDictionaryValue> SearchPropertyDictionaryValues(PropertyDictionaryValueSearchCriteria criteria);
    }
}
