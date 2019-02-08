namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Rewards
{
    /// <summary>
    /// Shipment reward
    /// </summary>
    public class ShipmentReward : AmountBasedReward
    {
        public ShipmentReward()
        {
        }
        //Copy constructor
        protected ShipmentReward(ShipmentReward other)
            : base(other)
        {
            ShippingMethod = other.ShippingMethod;
            ShippingMethodOption = other.ShippingMethodOption;

        }
        public string ShippingMethod { get; set; }
        public string ShippingMethodOption { get; set; }

        public override PromotionReward Clone()
        {
            return new ShipmentReward(this);
        }
    }
}
