using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Cart subtotal is []
    public class ConditionCartSubtotalLeast : BaseCondition
    {
        public decimal SubTotal { get; set; }

        public decimal SubTotalSecond { get; set; }


        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();

        /// <summary>
        /// ((PromotionEvaluationContext)x).GetCartTotalWithExcludings(ExcludingCategoryIds, ExcludingProductIds) > SubTotal
        /// </summary>
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                var quantity = promotionEvaluationContext.GetCartTotalWithExcludings(ExcludingCategoryIds.ToArray(), ExcludingProductIds.ToArray());
                result = quantity > SubTotal || quantity > SubTotalSecond;
            }

            return result;
        }
    }
}
