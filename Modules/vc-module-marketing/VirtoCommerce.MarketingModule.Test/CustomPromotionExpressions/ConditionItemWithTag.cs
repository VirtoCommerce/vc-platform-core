using System;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Test.CustomPromotionExpressions
{
    //items with [] tag
    public class ConditionItemWithTag : ConditionTree
    {
        public string[] Tags { get; set; }

        /// <summary>
        /// ((PromotionEvaluationContext)x).CheckItemTags() > NumItem
        /// </summary>
        /// <returns></returns>
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                return Tags.Any(x => promotionEvaluationContext.PromoEntry.Attributes.ContainsKey("tag") && string.Equals(promotionEvaluationContext.PromoEntry.Attributes["tag"], x, StringComparison.InvariantCultureIgnoreCase));
            }

            return false;
        }
    }
}
