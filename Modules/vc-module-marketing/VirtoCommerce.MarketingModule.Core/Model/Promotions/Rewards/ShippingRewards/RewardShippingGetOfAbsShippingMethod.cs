using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //Get $[] off shipping []
    public class RewardShippingGetOfAbsShippingMethod : ConditionTree, IReward
    {
        public decimal Amount { get; set; }
        public string ShippingMethod { get; set; }

        #region IRewardExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = new ShipmentReward
            {
                Amount = Amount,
                AmountType = RewardAmountType.Absolute,
                ShippingMethod = ShippingMethod
            };
            return new PromotionReward[] { retVal };
        }

        #endregion
    }
}
