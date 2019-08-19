using System;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PricingModule.Data.ExportImport.Converters
{
    public class TabularPriceDataConverter : ITabularDataConverter
    {
        public IExportable ToTabular(IExportable obj)
        {
            var source = obj as ExportablePrice ?? throw new ArgumentException(nameof(obj));
            var result = AbstractTypeFactory<TabularPrice>.TryCreateInstance();

            result.Currency = source.Currency;
            result.Id = source.Id;
            result.List = source.List;
            result.MinQuantity = source.MinQuantity;
            result.PricelistId = source.PricelistId;
            result.ProductId = source.ProductId;
            result.Sale = source.Sale;

            return result;
        }
    }
}
