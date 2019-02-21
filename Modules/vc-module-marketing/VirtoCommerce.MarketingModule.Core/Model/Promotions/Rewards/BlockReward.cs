using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class BlockReward : ConditionRewardTree, IReward
    {
        #region IRewardsExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = new PromotionReward[] { };
            if (Children != null)
            {
                retVal = Children.OfType<IReward>().SelectMany(x => x.GetRewards()).ToArray();
            }
            return retVal;
        }

        #endregion
    }
}
