using System.Threading.Tasks;
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
        Task<PromotionResult> EvaluatePromotionAsync(IEvaluationContext context);
    }
}
