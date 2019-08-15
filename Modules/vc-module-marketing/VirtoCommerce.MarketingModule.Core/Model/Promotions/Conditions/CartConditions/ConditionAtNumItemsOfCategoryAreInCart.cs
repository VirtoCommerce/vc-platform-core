using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //[] [] items of category are in shopping cart
    public class ConditionAtNumItemsInCategoryAreInCart : CompareConditionBase
    {
        public int NumItem { get; set; }
        public int NumItemSecond { get; set; }

        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();

        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).GetCartItemsOfCategoryQuantity(CategoryId, ExcludingCategoryIds, ExcludingProductIds) > NumItem
        /// </summary>
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                var quantity = promotionEvaluationContext.GetCartItemsOfCategoryQuantity(CategoryId, ExcludingCategoryIds.ToArray(), ExcludingProductIds.ToArray());
                result = UseCompareCondition(quantity, NumItem, NumItemSecond);
            }

            return result;
        }
    }
}
