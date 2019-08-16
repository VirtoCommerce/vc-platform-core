using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Product code contains []
    public class ConditionCodeContains : ConditionTree
    {
        public string Keyword { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).IsItemCodeContains(Keyword)
        /// </summary>
        public override bool IsSatisfiedBy(IEvaluationContext context)
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
