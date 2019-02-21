using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Product code contains []
    public class ConditionCodeContains : Condition
    {
        public string Keyword { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).IsItemCodeContains(Keyword)
        /// </summary>
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = promotionEvaluationContext.IsItemCodeContains(Keyword);
            }

            return result;
        }
    }
}
