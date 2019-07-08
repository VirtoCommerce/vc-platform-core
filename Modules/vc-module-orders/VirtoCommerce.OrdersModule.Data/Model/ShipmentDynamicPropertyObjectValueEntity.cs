using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public class ShipmentDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual ShipmentEntity Shipment { get; set; }
    }
}
