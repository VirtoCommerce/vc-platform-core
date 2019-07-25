using System;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport.Converters
{
    public class TabularProductDataConverter : ITabularDataConverter
    {
        public object ToTabular(object obj)
        {
            var source = obj as CatalogProduct ?? throw new ArgumentException(nameof(obj));
            var result = AbstractTypeFactory<TabularProduct>.TryCreateInstance();

            result.Id = source.Id;
            result.Name = source.Name;
            result.Code = source.Code;
            result.Gtin = source.Gtin;
            result.OuterId = source.OuterId;

            return result;
        }
    }
}
