using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards
{
    //Get [] % off shipping [] not to exceed $ [ 500 ]
    public class RewardShippingGetOfRelShippingMethod : DynamicExpression, IRewardExpression
	{
		public decimal Amount { get; set; }
		public string ShippingMethod { get; set; }
	    public decimal MaxLimit { get; set; }

        #region IRewardExpression Members

        public PromotionReward[] GetRewards()
		{
			var retVal = new ShipmentReward
			{
				Amount = Amount,
				AmountType = RewardAmountType.Relative,
				ShippingMethod = ShippingMethod,
                MaxLimit = MaxLimit
			};
			return new PromotionReward[] { retVal };
		}

		#endregion
	}
}
