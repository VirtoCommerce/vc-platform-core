using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions
{
    //Category is []
    public class ConditionCategoryIs : ConditionTree
    {
        public ICollection<string> ExcludingCategoryIds { get; set; } = new List<string>();
        public ICollection<string> ExcludingProductIds { get; set; } = new List<string>();

        public string CategoryId { get; set; }
        public string CategoryName { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).IsItemInCategory(CategoryId, ExcludingCategoryIds, ExcludingProductIds)
        /// </summary>
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = promotionEvaluationContext.IsItemInCategory(CategoryId, ExcludingCategoryIds.ToArray(), ExcludingProductIds.ToArray());
            }

            return result;
        }
    }
}
