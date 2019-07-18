using System;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    // because we do not have Operation table, we need to store FK for each derived class
    public class OrderDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity, ICloneable
    {
        #region Navigation Properties

        public string CustomerOrderId { get; set; }
        public virtual CustomerOrderEntity CustomerOrder { get; set; }

        public string PaymentInId { get; set; }
        public virtual PaymentInEntity PaymentIn { get; set; }

        public string ShipmentId { get; set; }
        public virtual ShipmentEntity Shipment { get; set; }

        public string LineItemId { get; set; }
        public virtual LineItemEntity LineItem { get; set; }

        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as OrderDynamicPropertyObjectValueEntity;

            if (CustomerOrder != null)
            {
                result.CustomerOrder = CustomerOrder.Clone() as CustomerOrderEntity;
            }

            if (PaymentIn != null)
            {
                result.PaymentIn = PaymentIn.Clone() as PaymentInEntity;
            }

            if (Shipment != null)
            {
                result.Shipment = Shipment.Clone() as ShipmentEntity;
            }

            if (LineItem != null)
            {
                result.LineItem = LineItem.Clone() as LineItemEntity;
            }

            return result;
        }

        #endregion
    }
}
