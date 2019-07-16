using System;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class TabularPricelistAssignment
    {
        public string Id { get; set; }
        public string CatalogId { get; set; }
        public string PricelistId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
