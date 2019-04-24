using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //Get []% off [ select product ] not to exceed $ [ 500 ]
    public class RewardItemGetOfRel : ConditionTree, IReward
    {
        public decimal Amount { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal MaxLimit { get; set; }
        #region IRewardExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = new CatalogItemAmountReward
            {
                Amount = Amount,
                AmountType = RewardAmountType.Relative,
                ProductId = ProductId,
                MaxLimit = MaxLimit
            };
            return new PromotionReward[] { retVal };
        }

        #endregion
    }
}
