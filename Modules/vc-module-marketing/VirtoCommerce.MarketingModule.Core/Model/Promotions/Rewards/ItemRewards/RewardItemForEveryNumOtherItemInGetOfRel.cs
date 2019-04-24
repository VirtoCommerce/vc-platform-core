namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    //For [] items of entry [] in every [] items of entry [] get [] % off no more than [] not to exceed $ []
    public class RewardItemForEveryNumOtherItemInGetOfRel : RewardItemForEveryNumInGetOfRel
    {
        public ProductContainer ConditionalProduct { get; set; } = new ProductContainer();

        #region IRewardExpression Members

        public override PromotionReward[] GetRewards()
        {
            var result = base.GetRewards();
            foreach (var reward in result)
            {
                var amountReward = reward as CatalogItemAmountReward;
                if (amountReward != null)
                {
                    amountReward.ConditionalProductId = ConditionalProduct?.ProductId;
                }
            }
            return result;
        }

        #endregion
    }
}
