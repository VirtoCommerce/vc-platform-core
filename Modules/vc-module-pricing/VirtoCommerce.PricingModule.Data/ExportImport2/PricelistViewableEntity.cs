using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistViewableEntity : ViewableEntity
    {
        public string Description { get; set; }
        public string Currency { get; set; }
        public string OuterId { get; set; }
    }
}
