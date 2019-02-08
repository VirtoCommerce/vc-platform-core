using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IPromotionUsageService
    {
        GenericSearchResult<PromotionUsage> SearchUsages(PromotionUsageSearchCriteria criteria);

        PromotionUsage[] GetByIds(string[] ids);
        void SaveUsages(PromotionUsage[] usages);
        void DeleteUsages(string[] ids);
    }
}
