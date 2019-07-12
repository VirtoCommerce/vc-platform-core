using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Search
{
    public interface IPromotionUsageSearchService
    {
        Task<GenericSearchResult<PromotionUsage>> SearchUsagesAsync(PromotionUsageSearchCriteria criteria);
    }
}
