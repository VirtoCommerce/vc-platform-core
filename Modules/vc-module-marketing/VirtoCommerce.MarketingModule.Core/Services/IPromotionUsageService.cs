using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IPromotionUsageService
    {
        Task<GenericSearchResult<PromotionUsage>> SearchUsagesAsync(PromotionUsageSearchCriteria criteria);

        Task<PromotionUsage[]> GetByIdsAsync(string[] ids);
        Task SaveUsagesAsync(PromotionUsage[] usages);
        Task DeleteUsagesAsync(string[] ids);
    }
}
