using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            result.Prices = Prices?.Select(x => x.Clone()).OfType<Price>().ToList();
            result.Assignments = Assignments?.Select(x => x.Clone()).OfType<PricelistAssignment>().ToList();

            return result;
        } 
        #endregion
    }
}
