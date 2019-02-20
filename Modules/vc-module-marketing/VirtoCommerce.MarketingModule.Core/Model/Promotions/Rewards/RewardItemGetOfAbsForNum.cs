using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //Get $[] off [] items of entry []
    public class RewardItemGetOfAbsForNum : BaseCondition, IRewardExpression
    {
        public decimal Amount { get; set; }
        public string ProductId { get; set; }
        public int NumItem { get; set; }
        public string ProductName { get; set; }

        #region IRewardExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = new CatalogItemAmountReward
            {
                Amount = Amount,
                AmountType = RewardAmountType.Absolute,
                Quantity = NumItem,
                ProductId = ProductId
            };
            return new PromotionReward[] { retVal };
        }

        #endregion
    }
}
