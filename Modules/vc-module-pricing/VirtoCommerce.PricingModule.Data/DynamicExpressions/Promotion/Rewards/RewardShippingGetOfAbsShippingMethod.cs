using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards
{
	//Get $[] off shipping []
	public class RewardShippingGetOfAbsShippingMethod : DynamicExpression, IRewardExpression
	{
		public decimal Amount { get; set; }
		public string ShippingMethod { get; set; }

		#region IRewardExpression Members

		public PromotionReward[] GetRewards()
		{
			var retVal = new ShipmentReward
			{
				Amount = Amount,
				AmountType = RewardAmountType.Absolute,
				ShippingMethod = ShippingMethod
			};
			return new PromotionReward[] { retVal };
		}

		#endregion
	}
}
