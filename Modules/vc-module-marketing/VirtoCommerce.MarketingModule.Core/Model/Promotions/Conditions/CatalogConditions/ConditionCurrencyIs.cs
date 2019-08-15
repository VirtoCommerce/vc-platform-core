using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Currency is []
    public class ConditionCurrencyIs : ConditionTree
    {
        public string Currency { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).Currency == Currency
        /// </summary>
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = promotionEvaluationContext.Currency.Equals(Currency);
            }

            return result;
        }
    }
}
