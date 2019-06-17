using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IPromotionService
    {
        Task<Promotion[]> GetPromotionsByIdsAsync(string[] ids);
        Task SavePromotionsAsync(Promotion[] promotions);
        Task DeletePromotionsAsync(string[] ids);
    }
}
