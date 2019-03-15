using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //First time buyers
    public class ConditionIsFirstTimeBuyer : Condition
    {
        /// <summary>
        ///  ((PromotionEvaluationContext)x).IsFirstTimeBuyer
        /// </summary>
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = promotionEvaluationContext.IsFirstTimeBuyer;
            }

            return result;
        }
    }
}
