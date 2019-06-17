using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IPromotionSearchService
    {
        Task<PromotionSearchResult> SearchPromotionsAsync(PromotionSearchCriteria criteria);
    }
}
