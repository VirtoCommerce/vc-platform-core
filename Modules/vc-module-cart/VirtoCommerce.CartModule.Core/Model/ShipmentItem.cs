using System;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Swagger;

namespace VirtoCommerce.CartModule.Core.Model
{
    [SwaggerSchemaId("CartShipmentItem")]
    public class ShipmentItem : AuditableEntity, ICloneable
    {
        public ShipmentItem()
        {
        }

        public ShipmentItem(LineItem lineItem)
        {
            LineItem = lineItem;
            LineItemId = lineItem.Id;

            Quantity = lineItem.Quantity;
        }

        public string LineItemId { get; set; }
        public LineItem LineItem { get; set; }

        public string BarCode { get; set; }

        public int Quantity { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as ShipmentItem;

            if (LineItem != null)
            {
                result.LineItem = LineItem.Clone() as LineItem;
            }

            return result;
        }

        #endregion
    }
}
