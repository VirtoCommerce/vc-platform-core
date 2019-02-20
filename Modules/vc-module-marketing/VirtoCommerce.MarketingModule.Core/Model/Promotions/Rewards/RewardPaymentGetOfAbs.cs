using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //Get $[] off payment method []
    public class RewardPaymentGetOfAbs : BaseCondition, IRewardExpression
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
