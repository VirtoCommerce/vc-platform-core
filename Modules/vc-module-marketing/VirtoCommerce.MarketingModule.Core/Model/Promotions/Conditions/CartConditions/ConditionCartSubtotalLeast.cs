using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Cart subtotal is []
    public class ConditionCartSubtotalLeast : CompareConditionBase
    {
        public decimal SubTotal { get; set; }

        public decimal SubTotalSecond { get; set; }


        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();

        /// <summary>
        /// ((PromotionEvaluationContext)x).GetCartTotalWithExcludings(ExcludingCategoryIds, ExcludingProductIds) > SubTotal
        /// </summary>
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                var quantity = promotionEvaluationContext.GetCartTotalWithExcludings(ExcludingCategoryIds.ToArray(), ExcludingProductIds.ToArray());
                result = UseCompareCondition(quantity, SubTotal, SubTotalSecond);
            }

            return result;
        }
    }
}
