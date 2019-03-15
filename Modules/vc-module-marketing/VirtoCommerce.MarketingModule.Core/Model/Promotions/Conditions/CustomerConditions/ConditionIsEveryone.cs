using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Everyone
    public class ConditionIsEveryone : Condition
    {
        /// <summary>
        /// ((PromotionEvaluationContext)x).IsEveryone
        /// </summary>
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = promotionEvaluationContext.IsEveryone;
            }

            return result;
        }
    }
}
