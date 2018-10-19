using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards
{
    //Get [] % off payment method [] not to exceed $ [ 500 ]
    public class RewardPaymentGetOfRel : DynamicExpression, IRewardExpression
	{
		public decimal Amount { get; set; }
		public string PaymentMethod { get; set; }
	    public decimal MaxLimit { get; set; }

        #region IRewardExpression Members

        public PromotionReward[] GetRewards()
		{
			var retVal = new PaymentReward
			{
				Amount = Amount,
				AmountType = RewardAmountType.Relative,
                PaymentMethod = PaymentMethod,
                MaxLimit = MaxLimit
            };
			return new PromotionReward[] { retVal };
		}

		#endregion
	}
}
