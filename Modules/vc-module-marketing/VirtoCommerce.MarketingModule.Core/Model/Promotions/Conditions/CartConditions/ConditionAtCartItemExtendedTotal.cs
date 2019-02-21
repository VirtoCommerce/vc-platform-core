using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Line item subtotal is []
    public class ConditionAtCartItemExtendedTotal : Condition
    {
        public decimal LineItemTotal { get; set; }
        public decimal LineItemTotalSecond { get; set; }

        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();

        /// <summary>
        /// ((PromotionEvaluationContext)x).IsAnyLineItemTotal(LineItemTotal, LineItemTotalSecond, CompareCondition,  ExcludingCategoryIds, ExcludingProductIds)
        /// </summary>
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = promotionEvaluationContext.IsAnyLineItemExtendedTotalNew(LineItemTotal
                    , LineItemTotalSecond
                    , ModuleConstants.ConditionOperation.AtLeast
                    , ExcludingCategoryIds.ToArray()
                    , ExcludingProductIds.ToArray());
            }

            return result;
        }
    }
}
