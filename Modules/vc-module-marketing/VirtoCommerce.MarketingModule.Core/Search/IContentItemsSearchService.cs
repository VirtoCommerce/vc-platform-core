using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent.Search;

namespace VirtoCommerce.MarketingModule.Core.Search
{
    public interface IContentItemsSearchService
    {
        Task<DynamicContentItemSearchResult> SearchContentItemsAsync(DynamicContentItemSearchCriteria criteria);
    }
}
