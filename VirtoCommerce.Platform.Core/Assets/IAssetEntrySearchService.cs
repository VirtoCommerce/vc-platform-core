using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Assets
{
    public interface IAssetEntrySearchService
    {
        Task<GenericSearchResult<AssetEntry>> SearchAssetEntriesAsync(AssetEntrySearchCriteria criteria);
    }
}
