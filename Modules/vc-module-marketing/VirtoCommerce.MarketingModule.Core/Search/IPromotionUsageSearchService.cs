using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;

namespace VirtoCommerce.MarketingModule.Core.Search
{
    public interface IPromotionUsageSearchService
    {
        Task<PromotionUsageSearchResult> SearchUsagesAsync(PromotionUsageSearchCriteria criteria);
    }
}
