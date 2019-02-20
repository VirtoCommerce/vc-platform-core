using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //Get [] % off shipping [] not to exceed $ [ 500 ]
    public class RewardShippingGetOfRelShippingMethod : BaseCondition, IRewardExpression
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
