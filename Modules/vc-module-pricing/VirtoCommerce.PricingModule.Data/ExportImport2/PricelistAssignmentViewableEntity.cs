using System;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistAssignmentViewableEntity : ViewableEntity
    {
        public string Catalog { get; set; }
        public string Pricelist { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
