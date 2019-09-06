using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.InventoryModule.Core.Model
{
    public class InventoryInfo : AuditableEntity, ICloneable
    {
        public string FulfillmentCenterId { get; set; }
        public FulfillmentCenter FulfillmentCenter { get; set; }

        public string ProductId { get; set; }

        public long InStockQuantity { get; set; }
        public long ReservedQuantity { get; set; }
        public long ReorderMinQuantity { get; set; }
        public long PreorderQuantity { get; set; }
        public long BackorderQuantity { get; set; }
        public bool AllowBackorder { get; set; }
        public bool AllowPreorder { get; set; }
        public long InTransit { get; set; }
        public DateTime? PreorderAvailabilityDate { get; set; }
        public DateTime? BackorderAvailabilityDate { get; set; }
        public InventoryStatus Status { get; set; }
        public string OuterId { get; set; }


        public virtual bool IsAvailableOn(DateTime date)
        {
            var result = AllowPreorder && (PreorderAvailabilityDate ?? DateTime.MinValue) <= date && PreorderQuantity > 0;
            if (!result)
            {
                result = AllowBackorder && (BackorderAvailabilityDate ?? DateTime.MaxValue) >= date && BackorderQuantity > 0;
            }
            if (!result)
            {
                result = Math.Max(0, InStockQuantity - ReservedQuantity) > 0;
            }
            return result;
        }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as InventoryInfo;

            if (FulfillmentCenter != null)
            {
                result.FulfillmentCenter = FulfillmentCenter.Clone() as FulfillmentCenter;
            }

            return result;
        }

        #endregion
    }
}
