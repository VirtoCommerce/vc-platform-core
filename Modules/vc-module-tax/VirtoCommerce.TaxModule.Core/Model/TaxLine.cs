using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaxModule.Core.Model
{
    /// <summary>
    /// Represent one abstract position for tax calculation
    /// </summary>
    public class TaxLine : Entity, ICloneable
    {
        public string Code { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Item type name (shipment, lineItem, car etc)
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// Total sum  
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Price per item
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Tax category/type
        /// </summary>
        public string TaxType { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as TaxLine;
        }

        #endregion
    }
}
