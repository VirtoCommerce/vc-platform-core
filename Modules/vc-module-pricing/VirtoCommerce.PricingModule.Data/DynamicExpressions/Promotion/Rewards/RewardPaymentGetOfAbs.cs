using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards
{
	//Get $[] off payment method []
	public class RewardPaymentGetOfAbs : DynamicExpression, IRewardExpression
	{
		public decimal Amount { get; set; }
		public string PaymentMethod { get; set; }

		#region IRewardExpression Members

		public PromotionReward[] GetRewards()
		{
			var retVal = new PaymentReward
			{
				Amount = Amount,
				AmountType = RewardAmountType.Absolute,
				PaymentMethod = PaymentMethod
            };
			return new PromotionReward[] { retVal };
		}

		#endregion
	}
}
