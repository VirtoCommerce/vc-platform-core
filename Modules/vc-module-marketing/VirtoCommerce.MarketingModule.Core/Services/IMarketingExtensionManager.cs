using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IMarketingExtensionManager
    {
        //TODO
        //PromoDynamicExpressionTree PromotionDynamicExpressionTree { get; set; }
        //ConditionExpressionTree DynamicContentExpressionTree { get; set; }

        void RegisterPromotion(Promotion promotion);
        IEnumerable<Promotion> Promotions { get; }
    }
}
