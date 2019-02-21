using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IMarketingExtensionManager
    {
        //TODO
        IConditionRewardTree PromotionCondition { get; set; }
        IConditionRewardTree ContentCondition { get; set; }

        void RegisterPromotion(Promotion promotion);
        IEnumerable<Promotion> Promotions { get; }
    }
}
