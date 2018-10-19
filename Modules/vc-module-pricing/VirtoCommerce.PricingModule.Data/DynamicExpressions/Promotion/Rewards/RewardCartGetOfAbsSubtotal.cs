using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Promotion.Rewards
{
    //Get [] $ off cart subtotal
    public class RewardCartGetOfAbsSubtotal : DynamicExpression, IRewardExpression
    {
        public decimal Amount { get; set; }

        #region IRewardsExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = new CartSubtotalReward()
            {
                Amount = Amount,
                AmountType = RewardAmountType.Absolute
            };
            return new PromotionReward[] { retVal };
        }

        #endregion
    }
}
