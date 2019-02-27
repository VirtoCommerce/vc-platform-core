using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    /// <summary>
    /// Represent abstraction for search property dictionary items  
    /// </summary>
    public interface IProperyDictionaryItemSearchService
    {
        Task<GenericSearchResult<PropertyDictionaryItem>> SearchAsync(PropertyDictionaryItemSearchCriteria searchCriteria);
    }
}
