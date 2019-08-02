using System;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PriceViewableEntity : ViewableEntity
    {
        public string PricelistName { get; set; }
        public string PricelistId { get; set; }
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string Currency { get; set; }
        public decimal? Sale { get; set; }
        public decimal List { get; set; }
        public int MinQuantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OuterId { get; set; }
    }
}
