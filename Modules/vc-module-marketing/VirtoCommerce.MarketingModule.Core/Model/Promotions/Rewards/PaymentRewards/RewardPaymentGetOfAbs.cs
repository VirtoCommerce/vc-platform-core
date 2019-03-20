using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //Get $[] off payment method []
    public class RewardPaymentGetOfAbs : ConditionTree, IReward
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
