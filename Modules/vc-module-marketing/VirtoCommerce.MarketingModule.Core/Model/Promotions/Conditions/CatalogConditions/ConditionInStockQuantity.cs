using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //InStock quantity is []
    public class ConditionInStockQuantity : CompareConditionBase
    {
        public int Quantity { get; set; }

        public int QuantitySecond { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).IsItemsInStockQuantity(Exactly, Quantity, QuantitySecond)
        /// </summary>
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = promotionEvaluationContext.IsItemsInStockQuantityNew(CompareCondition, Quantity, QuantitySecond);
            }

            return result;
        }
    }
}
