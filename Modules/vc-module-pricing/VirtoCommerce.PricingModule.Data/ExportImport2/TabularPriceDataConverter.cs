using System;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class TabularPriceDataConverter : ITabularDataConverter
    {
        public object ToTabular(object obj)
        {
            var source = obj as Price ?? throw new ArgumentException(nameof(obj));
            var result = AbstractTypeFactory<TabularPrice>.TryCreateInstance();

            result.Currency = source.Currency;
            result.EndDate = source.EndDate;
            result.Id = source.Id;
            result.List = source.List;
            result.MinQuantity = source.MinQuantity;
            result.PricelistId = source.PricelistId;
            result.ProductId = source.ProductId;
            result.Sale = source.Sale;
            result.StartDate = source.StartDate;

            return result;
        }
    }
}
