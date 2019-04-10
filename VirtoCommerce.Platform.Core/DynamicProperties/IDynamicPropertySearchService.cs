using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.DynamicProperties
{
    public interface IDynamicPropertySearchService
    {
        Task<DynamicPropertySearchResult> SearchDynamicPropertiesAsync(DynamicPropertySearchCriteria criteria);
        Task<DynamicPropertyDictionaryItemSearchResult> SearchDictionaryItemsAsync(DynamicPropertyDictionaryItemSearchCriteria criteria);
    }
}
