using System;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class PromotionConditionAndRewardTree : BlockConditionAndOr, IReward
    {
        public PromotionConditionAndRewardTree()
        {
            All = true;
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
    }
}
