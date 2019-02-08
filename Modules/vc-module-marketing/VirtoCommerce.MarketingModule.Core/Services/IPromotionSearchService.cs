using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IPromotionSearchService
    {
        GenericSearchResult<Promotion> SearchPromotions(PromotionSearchCriteria criteria);
    }
}
