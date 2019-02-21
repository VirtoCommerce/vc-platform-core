using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //Get []% off [] items of entry [] not to exceed $ [ 500 ]
    public class RewardItemGetOfRelForNum : ConditionRewardTree, IReward
    {
        public decimal Amount { get; set; }
        public string ProductId { get; set; }
        public int NumItem { get; set; }
        public string ProductName { get; set; }
        public decimal MaxLimit { get; set; }
        #region IRewardExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = new CatalogItemAmountReward
            {
                Amount = Amount,
                AmountType = RewardAmountType.Relative,
                Quantity = NumItem,
                ProductId = ProductId,
                MaxLimit = MaxLimit
            };
            return new PromotionReward[] { retVal };
        }

        #endregion
    }
}
