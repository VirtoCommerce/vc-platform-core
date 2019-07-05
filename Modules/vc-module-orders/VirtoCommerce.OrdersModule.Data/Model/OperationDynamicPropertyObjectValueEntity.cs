using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    // because we do not have Operation table, we need to store FK for each derived class
    public class OperationDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual PaymentInEntity PaymentIn { get; set; }

        public virtual ShipmentEntity Shipment { get; set; }

        public virtual CustomerOrderEntity CustomerOrder { get; set; }
    }
}
