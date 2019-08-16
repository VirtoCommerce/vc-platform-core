using System;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PricingModule.Data.ExportImport.Converters
{
    public class TabularPricelistDataConverter : ITabularDataConverter
    {
        public virtual object ToTabular(object obj)
        {
            var source = obj as ExportablePricelist ?? throw new ArgumentException(nameof(obj));
            var result = AbstractTypeFactory<TabularPricelist>.TryCreateInstance();

            result.Currency = source.Currency;
            result.Description = source.Description;
            result.Id = source.Id;
            result.Name = source.Name;

            return result;
        }
    }
}
