using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.InventoryModule.Core.Model
{
    public class FulfillmentCenter : AuditableEntity, ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string GeoLocation { get; set; }
        public Address Address { get; set; }
        public string OuterId { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as FulfillmentCenter;

            if (Address != null)
            {
                result.Address = Address.Clone() as Address;
            }

            return result;
        }

        #endregion
    }
}
