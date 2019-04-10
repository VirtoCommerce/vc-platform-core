namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class CartSubtotalReward : AmountBasedReward
    {
        public CartSubtotalReward()
        {
        }
        //Copy constructor
        protected CartSubtotalReward(CartSubtotalReward other)
            : base(other)
        {
            Amount = other.Amount;
        }


    }
}
