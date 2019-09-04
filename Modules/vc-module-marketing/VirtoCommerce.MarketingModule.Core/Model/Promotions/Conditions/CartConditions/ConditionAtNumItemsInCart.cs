using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //[] [] items are in shopping cart
    public class ConditionAtNumItemsInCart : CompareConditionBase
    {

        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();

        public int NumItem { get; set; }
        public int NumItemSecond { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).GetCartItemsQuantity(ExcludingCategoryIds, ExcludingProductIds) > NumItem
        /// </summary>
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                var quantity = promotionEvaluationContext.GetCartItemsQuantity(ExcludingCategoryIds.ToArray(), ExcludingProductIds.ToArray());
                result = UseCompareCondition(quantity, NumItem, NumItemSecond);
            }

            return result;
        }
    }
}
