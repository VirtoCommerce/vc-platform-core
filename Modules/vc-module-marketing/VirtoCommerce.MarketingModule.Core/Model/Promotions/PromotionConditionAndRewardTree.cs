using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Conditions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class PromotionConditionAndRewardTree : ConditionTree, IReward
    {
        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is PromotionEvaluationContext promotionEvaluationContext)
            {
                result = Children.All(c => c.IsSatisfiedBy(promotionEvaluationContext));
            }
            return result;
        }

        public virtual PromotionReward[] GetRewards()
        {
            var result = Array.Empty<PromotionReward>();
            if (Children != null)
            {
                result = Children.OfType<IReward>().SelectMany(x => x.GetRewards()).ToArray();
            }
            return result;
        }

        public override IEnumerable<IConditionTree> AvailableChildren
        {
            get
            {
                yield return AbstractTypeFactory<BlockCustomerCondition>.TryCreateInstance();
                yield return AbstractTypeFactory<BlockCatalogCondition>.TryCreateInstance();
                yield return AbstractTypeFactory<BlockCartCondition>.TryCreateInstance();
                yield return AbstractTypeFactory<BlockReward>.TryCreateInstance();
            }
        }      
    }
}
