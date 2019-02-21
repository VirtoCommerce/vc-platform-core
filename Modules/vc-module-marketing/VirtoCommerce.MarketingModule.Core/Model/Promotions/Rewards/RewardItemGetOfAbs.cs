using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //Get $[] off 
    public class RewardItemGetOfAbs : ConditionRewardTree, IReward
    {
        public decimal Amount { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        #region IRewardExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = new CatalogItemAmountReward
            {
                Amount = Amount,
                AmountType = RewardAmountType.Absolute,
                ProductId = ProductId
            };
            return new PromotionReward[] { retVal };
        }

        #endregion
    }
}
