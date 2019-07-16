using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PricingModule.Core.Model
{
    public class Pricelist : AuditableEntity, ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public string OuterId { get; set; }
        public ICollection<Price> Prices { get; set; }
        public ICollection<PricelistAssignment> Assignments { get; set; }

        #region ICloneable members
        public virtual object Clone()
        {
            var result = MemberwiseClone() as Pricelist;
            if (Prices != null)
            {
                result.Prices = new List<Price>(Prices.Select(x => x.Clone() as Price));
            }
            if (Assignments != null)
            {
                result.Assignments = new List<PricelistAssignment>(Assignments.Select(x => x.Clone() as PricelistAssignment));
            }
            return result;
        } 
        #endregion
    }
}
