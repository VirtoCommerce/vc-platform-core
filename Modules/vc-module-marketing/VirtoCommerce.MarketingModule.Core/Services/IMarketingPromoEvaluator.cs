using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IMarketingPromoEvaluator
    {
        /// <summary>
        /// Evaluate promotion for specific context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        PromotionResult EvaluatePromotion(IEvaluationContext context);
    }
}
