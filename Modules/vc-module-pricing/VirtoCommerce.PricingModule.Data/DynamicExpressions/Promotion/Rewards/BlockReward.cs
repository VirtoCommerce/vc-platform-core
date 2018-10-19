using System.Linq;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards
{
	public class RewardBlock : DynamicExpression, IRewardExpression
	{
		#region IRewardsExpression Members

		public PromotionReward[] GetRewards()
		{
			var retVal = new PromotionReward[] { };
			if (Children != null)
			{
				 retVal = Children.OfType<IRewardExpression>().SelectMany(x => x.GetRewards()).ToArray();
			}
			return retVal;
		}

		#endregion
	}
}
