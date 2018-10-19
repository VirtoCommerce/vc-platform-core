namespace VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards
{
    /// <summary>
    /// Payment reward
    /// </summary>
    public class PaymentReward : AmountBasedReward
    {
        public PaymentReward()
        {
        }

        //Copy constructor
        protected PaymentReward(PaymentReward other)
            : base(other)
        {
            PaymentMethod = other.PaymentMethod;
        }
        public string PaymentMethod { get; set; }

        public override PromotionReward Clone()
        {
            return new PaymentReward(this);
        }
    }
}
