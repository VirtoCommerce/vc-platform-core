using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;

namespace VirtoCommerce.MarketingModule.Core.Search
{
    public interface IPromotionSearchService
    {
        Task<PromotionSearchResult> SearchPromotionsAsync(PromotionSearchCriteria criteria);
    }
}
