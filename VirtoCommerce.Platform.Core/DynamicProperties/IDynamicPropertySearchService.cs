using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.DynamicProperties
{
    public interface IDynamicPropertySearchService
    {
        Task<GenericSearchResult<DynamicProperty>> SearchDynamicPropertiesAsync(DynamicPropertySearchCriteria criteria);
        Task<GenericSearchResult<DynamicPropertyDictionaryItem>> SearchDictionaryItemsAsync(DynamicPropertyDictionaryItemSearchCriteria criteria);
    }
}
