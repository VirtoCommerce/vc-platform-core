using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IPromotionSearchService
    {
        Task<GenericSearchResult<Promotion>> SearchPromotionsAsync(PromotionSearchCriteria criteria);
    }
}
