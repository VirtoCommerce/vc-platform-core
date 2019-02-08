using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IPromotionService
    {
        Promotion[] GetPromotionsByIds(string[] ids);
        void SavePromotions(Promotion[] promotions);
        void DeletePromotions(string[] ids);
    }
}
