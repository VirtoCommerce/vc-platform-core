using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePricingFull : IExportable
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }

        public ICollection<Pricelist> Pricelists { get; set; }
        public ICollection<Price> Prices { get; set; }
        public ICollection<PricelistAssignment> Assignments { get; set; }


        public object Clone()
        {
            var result = MemberwiseClone() as ExportablePricingFull;
            result.Pricelists = Pricelists?.Select(x => x.Clone() as Pricelist).ToList();
            result.Prices = Prices?.Select(x => x.Clone() as Price).ToList();
            result.Assignments = Assignments?.Select(x => x.Clone() as PricelistAssignment).ToList();

            return result;
        }
    }
}
