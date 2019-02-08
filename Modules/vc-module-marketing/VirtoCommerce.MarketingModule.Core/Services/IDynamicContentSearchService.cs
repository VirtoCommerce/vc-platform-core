using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IDynamicContentSearchService
    {
        GenericSearchResult<DynamicContentFolder> SearchFolders(DynamicContentFolderSearchCriteria criteria);
        GenericSearchResult<DynamicContentItem> SearchContentItems(DynamicContentItemSearchCriteria criteria);
        GenericSearchResult<DynamicContentPlace> SearchContentPlaces(DynamicContentPlaceSearchCriteria criteria);
        GenericSearchResult<DynamicContentPublication> SearchContentPublications(DynamicContentPublicationSearchCriteria criteria);
    }
}
