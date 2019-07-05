using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class ShipmentDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual ShipmentEntity Shipment { get; set; }
    }
}
