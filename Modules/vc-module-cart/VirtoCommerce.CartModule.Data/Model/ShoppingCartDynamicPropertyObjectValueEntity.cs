using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class ShoppingCartDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }
        public string ShipmentId { get; set; }
        public virtual ShipmentEntity Shipment { get; set; }
        public string PaymentId { get; set; }
        public virtual PaymentEntity Payment { get; set; }
    }
}
