using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IMarketingExtensionManager
    {
        IConditionTree PromotionCondition { get; set; }
        IConditionTree ContentCondition { get; set; }

        void RegisterPromotion(Promotion promotion);
        IEnumerable<Promotion> Promotions { get; }
    }
}
