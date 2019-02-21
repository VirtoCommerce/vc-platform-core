using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Currency is []
    public class ConditionCurrencyIs : Condition
    {
        public string Currency { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).Currency == Currency
        /// </summary>
        public override bool Evaluate(IEvaluationContext context)
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
