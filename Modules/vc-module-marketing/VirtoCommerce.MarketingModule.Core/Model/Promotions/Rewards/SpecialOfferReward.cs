namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    /// <summary>
    /// Special offer reward
    /// </summary>
    public class SpecialOfferReward : PromotionReward
    {
        public SpecialOfferReward()
        {
        }
        //Copy constructor
        protected SpecialOfferReward(SpecialOfferReward other)
            : base(other)
        {
        }

    }
}
