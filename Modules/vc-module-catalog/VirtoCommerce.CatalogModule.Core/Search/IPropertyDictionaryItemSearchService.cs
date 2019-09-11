using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Core.Search
{
    /// <summary>
    /// Represent abstraction for search property dictionary items  
    /// </summary>
    public interface IPropertyDictionaryItemSearchService
    {
        Task<PropertyDictionaryItemSearchResult> SearchAsync(PropertyDictionaryItemSearchCriteria searchCriteria);
    }
}
