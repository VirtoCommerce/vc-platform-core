using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Everyone
    public class ConditionIsEveryone : BaseCondition
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
