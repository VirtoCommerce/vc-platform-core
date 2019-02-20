using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //Get [] $ off cart subtotal
    public class RewardCartGetOfAbsSubtotal : BaseCondition, IRewardExpression
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
