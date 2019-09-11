using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class TabularPrice : IExportable
    {
        public string Id { get; set; }
        public string PricelistId { get; set; }
        public string PricelistName { get; set; }
        public string Currency { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? Sale { get; set; }
        public decimal List { get; set; }
        public int MinQuantity { get; set; }

        public object Clone()
        {
            return MemberwiseClone() as TabularPrice;
        }
    }
}
