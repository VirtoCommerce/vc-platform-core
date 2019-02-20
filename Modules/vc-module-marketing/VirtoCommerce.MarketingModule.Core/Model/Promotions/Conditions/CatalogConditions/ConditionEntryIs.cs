using VirtoCommerce.CoreModule.Core.Common;
namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Product is []
    public class ConditionEntryIs : BaseCondition
    {

        public string ProductId { get; set; }
        public string ProductName { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).IsItemInProduct(ProductId)
        /// </summary>
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = promotionEvaluationContext.IsItemInProduct(ProductId);
            }

            return result;
        }
    }
}
