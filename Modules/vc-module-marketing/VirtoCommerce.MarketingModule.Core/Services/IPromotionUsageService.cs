using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IPromotionUsageService
    {
        Task<PromotionUsage[]> GetByIdsAsync(string[] ids);
        Task SaveUsagesAsync(PromotionUsage[] usages);
        Task DeleteUsagesAsync(string[] ids);
    }
}
