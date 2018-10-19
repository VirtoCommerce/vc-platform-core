using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards
{
	//Get [] free items of Product []
	public class RewardItemGetFreeNumItemOfProduct : DynamicExpression, IRewardExpression
	{
		public string ProductId { get; set; }
		public string ProductName { get; set; }
		public int NumItem { get; set; }
		
		#region IRewardExpression Members

		public PromotionReward[] GetRewards()
		{
			var retVal = new CatalogItemAmountReward
			{
				Amount = 100,
				AmountType = RewardAmountType.Relative,
				Quantity = NumItem,
				ProductId = ProductId
			};
			return new PromotionReward[] { retVal };
		}

		#endregion
	}
}
