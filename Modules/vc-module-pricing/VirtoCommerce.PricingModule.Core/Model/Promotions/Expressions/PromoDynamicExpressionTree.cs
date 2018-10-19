using System.Linq;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions
{
    public class PromoDynamicExpressionTree : ConditionExpressionTree, IRewardExpression
    {
        #region IActionExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = Children.OfType<IRewardExpression>().SelectMany(x => x.GetRewards()).OfType<PromotionReward>().ToArray();

            return retVal;
        }

        #endregion
    }
}
