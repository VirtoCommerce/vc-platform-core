using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards
{
    //Get [] % off cart subtotal not to exceed $ [ 500 ]
    public class RewardCartGetOfRelSubtotal : DynamicExpression, IRewardExpression
    {
        public decimal Amount { get; set; }
        public decimal MaxLimit { get; set; }

        #region IRewardsExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = new CartSubtotalReward
            {
                Amount = Amount,
                AmountType = RewardAmountType.Relative,
                MaxLimit = MaxLimit
            };
            return new PromotionReward[] { retVal };
        }

        #endregion
    }
}
