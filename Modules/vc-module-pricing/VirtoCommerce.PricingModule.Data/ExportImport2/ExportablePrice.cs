using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePrice : Price, IExportable
    {
        #region IExportable properties

        public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }

        #endregion IExportable properties

        #region Properties specific to universal viewer

        public string PricelistName { get; set; }
        public string ProductName { get; set; }

        #endregion Properties specific to universal viewer

        public ExportablePrice FromModel(Price source)
        {
            Type = nameof(Price);
            Currency = source.Currency;
            Id = source.Id;
            List = source.List;
            MinQuantity = source.MinQuantity;
            PricelistId = source.PricelistId;
            ProductId = source.ProductId;
            Sale = source.Sale;
            OuterId = source.OuterId;

            return this;
        }
    }
}
