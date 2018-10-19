namespace VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards
{
    public class CatalogItemAmountReward : AmountBasedReward
    {
        public CatalogItemAmountReward()
        {
        }

        //Copy constructor
        protected CatalogItemAmountReward(CatalogItemAmountReward other)
            : base(other)
        {
            ProductId = other.ProductId;
        }

        public string ProductId { get; set; }

        public override PromotionReward Clone()
        {
            return new CatalogItemAmountReward(this);
        }
    }
}
