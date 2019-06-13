using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Tests
{
    public class Pricelist : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public ICollection<Price> Prices { get; set; }
        public ICollection<PricelistAssignment> Assignments { get; set; }
    }
}
