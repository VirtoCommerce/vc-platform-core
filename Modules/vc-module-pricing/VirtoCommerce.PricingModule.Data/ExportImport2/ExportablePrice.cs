using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePrice : Price, IExportable, IExportViewable, ITabularConvertible
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

        public virtual ExportablePrice FromModel(Price source)
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

        #region ITabularConvertible implementation

        public virtual IExportable ToTabular()
        {
            var result = AbstractTypeFactory<TabularPrice>.TryCreateInstance();

            result.Currency = Currency;
            result.Id = Id;
            result.List = List;
            result.MinQuantity = MinQuantity;
            result.PricelistId = PricelistId;
            result.PricelistName = PricelistName;
            result.ProductId = ProductId;
            result.ProductName = ProductName;
            result.Sale = Sale;

            return result;
        }

        #endregion ITabularConvertible implementation
    }
}
