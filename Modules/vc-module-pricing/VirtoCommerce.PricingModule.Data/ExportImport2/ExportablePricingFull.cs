using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePricingFull : IExportable
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }

        public ICollection<ExportablePricelist> Pricelists { get; set; }
        public ICollection<ExportablePrice> Prices { get; set; }
        public ICollection<ExportablePricelistAssignment> Assignments { get; set; }


        public object Clone()
        {
            var result = MemberwiseClone() as ExportablePricingFull;
            result.Pricelists = Pricelists?.Select(x => x.Clone() as ExportablePricelist).ToList();
            result.Prices = Prices?.Select(x => x.Clone() as ExportablePrice).ToList();
            result.Assignments = Assignments?.Select(x => x.Clone() as ExportablePricelistAssignment).ToList();

            return result;
        }
    }
}
