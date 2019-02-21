using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //Get [] free items of Product []
    public class RewardItemGetFreeNumItemOfProduct : ConditionRewardTree, IReward
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int NumItem { get; set; }

        #region IRewardExpression Members

        public PromotionReward[] GetRewards()
        {
            var retVal = new CatalogItemAmountReward
            {
                Amount = 100,
                AmountType = RewardAmountType.Relative,
                Quantity = NumItem,
                ProductId = ProductId
            };
            return new PromotionReward[] { retVal };
        }

        #endregion
    }
}
