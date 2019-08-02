using System;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistAssignmentViewableEntity : ViewableEntity
    {
        public string CatalogName { get; set; }
        public string CatalogId { get; set; }
        public string PricelistName { get; set; }
        public string PricelistId { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
