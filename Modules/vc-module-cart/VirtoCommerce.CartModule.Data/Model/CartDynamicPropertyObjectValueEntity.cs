using System;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class CartDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity, ICloneable
    {
        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }

        public string ShipmentId { get; set; }
        public virtual ShipmentEntity Shipment { get; set; }

        public string PaymentId { get; set; }
        public virtual PaymentEntity Payment { get; set; }

        public string LineItemId { get; set; }
        public virtual LineItemEntity LineItem { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as CartDynamicPropertyObjectValueEntity;

            if (ShoppingCart != null)
            {
                result.ShoppingCart = ShoppingCart.Clone() as ShoppingCartEntity;
            }

            if (Shipment != null)
            {
                result.Shipment = Shipment.Clone() as ShipmentEntity;
            }

            if (Payment != null)
            {
                result.Payment = Payment.Clone() as PaymentEntity;
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
