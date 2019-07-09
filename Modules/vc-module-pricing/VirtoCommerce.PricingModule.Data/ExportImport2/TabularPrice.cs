using System;

namespace VirtoCommerce.PricingModule.Data.ExportImport2
{
    public class TabularPrice
    {
        public string Id { get; set; }
        public string PricelistId { get; set; }
        public string Currency { get; set; }
        public string ProductId { get; set; }
        public decimal? Sale { get; set; }
        public decimal List { get; set; }
        public int MinQuantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
