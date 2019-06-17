using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent.Search;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IDynamicContentSearchService
    {
        Task<DynamicContentFolderSearchResult> SearchFoldersAsync(DynamicContentFolderSearchCriteria criteria);
        Task<DynamicContentItemSearchResult> SearchContentItemsAsync(DynamicContentItemSearchCriteria criteria);
        Task<DynamicContentPlaceSearchResult> SearchContentPlacesAsync(DynamicContentPlaceSearchCriteria criteria);
        Task<DynamicContentPublicationSearchResult> SearchContentPublicationsAsync(DynamicContentPublicationSearchCriteria criteria);
    }
}
