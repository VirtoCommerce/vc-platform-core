using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards
{
	//Get $[] off 
	public class RewardItemGetOfAbs : DynamicExpression, IRewardExpression
	{
		public decimal Amount { get; set; }
		public string ProductId { get; set; }
		public string ProductName { get; set; }

		#region IRewardExpression Members

		public PromotionReward[] GetRewards()
		{
			var retVal = new CatalogItemAmountReward
			{
				Amount = Amount,
				AmountType = RewardAmountType.Absolute,
				ProductId = ProductId
			};
			return new PromotionReward[] { retVal };
		}

		#endregion
	}
}
