using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //[] [] items of entry are in shopping cart
    public class ConditionAtNumItemsOfEntryAreInCart : CompareConditionBase
    {

        public int NumItem { get; set; }
        public int NumItemSecond { get; set; }

        public string ProductId { get; set; }
        public string ProductName { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).GetCartItemsOfProductQuantity(ProductId, ExcludingCategoryIds, ExcludingProductIds) > NumItem
        /// </summary>
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                var quantity = promotionEvaluationContext.GetCartItemsOfProductQuantity(ProductId);
                result = UseCompareCondition(quantity, NumItem, NumItemSecond);
            }

            return result;
        }
    }
}
