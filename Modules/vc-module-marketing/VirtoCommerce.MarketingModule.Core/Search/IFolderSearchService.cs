using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent.Search;

namespace VirtoCommerce.MarketingModule.Core.Search
{
    public interface IFolderSearchService
    {
        Task<DynamicContentFolderSearchResult> SearchFoldersAsync(DynamicContentFolderSearchCriteria criteria);
    }
}
