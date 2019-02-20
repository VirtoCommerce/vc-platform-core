using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IMarketingExtensionManager
    {
        //TODO
        ICondition PromotionCondition { get; set; }
        ICondition ContentCondition { get; set; }

        void RegisterPromotion(Promotion promotion);
        IEnumerable<Promotion> Promotions { get; }
    }
}
