using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Tests
{
    public class Pricelist : AuditableEntity, IExportable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public ICollection<Price> Prices { get; set; }
        public ICollection<PricelistAssignment> Assignments { get; set; }

        /// <summary>
        /// Added for testing purposes as 1 to 1 related entity, does not exists in original Pricelist object
        /// </summary>
        public PricelistAssignment ActiveAssignment { get; set; }

        /// <summary>
        /// Added for testing purposes to check same type properties mapping inside one entity for CSV export (tabular)
        /// </summary>
        public PricelistAssignment InactiveAssignment { get; set; }

        public object Clone()
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

            result.ActiveAssignment = ActiveAssignment?.Clone() as PricelistAssignment;
            result.InactiveAssignment = InactiveAssignment?.Clone() as PricelistAssignment;

            return result;
        }
    }
}
