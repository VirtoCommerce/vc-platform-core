using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class TabularPricelist : IExportable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }

        public object Clone()
        {
            return MemberwiseClone() as TabularPricelist;
        }
    }
}
