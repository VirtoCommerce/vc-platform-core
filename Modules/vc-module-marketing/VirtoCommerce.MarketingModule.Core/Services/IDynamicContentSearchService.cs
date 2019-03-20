using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IDynamicContentSearchService
    {
        Task<GenericSearchResult<DynamicContentFolder>> SearchFoldersAsync(DynamicContentFolderSearchCriteria criteria);
        Task<GenericSearchResult<DynamicContentItem>> SearchContentItemsAsync(DynamicContentItemSearchCriteria criteria);
        Task<GenericSearchResult<DynamicContentPlace>> SearchContentPlacesAsync(DynamicContentPlaceSearchCriteria criteria);
        Task<GenericSearchResult<DynamicContentPublication>> SearchContentPublicationsAsync(DynamicContentPublicationSearchCriteria criteria);
    }
}
