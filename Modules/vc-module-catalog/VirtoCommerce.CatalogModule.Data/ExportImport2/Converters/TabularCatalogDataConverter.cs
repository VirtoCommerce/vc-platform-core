using System;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport.Converters
{
    public class TabularCatalogDataConverter : ITabularDataConverter
    {
        public object ToTabular(object obj)
        {
            var source = obj as Catalog ?? throw new ArgumentException(nameof(obj));
            var result = AbstractTypeFactory<TabularCatalog>.TryCreateInstance();

            result.Id = source.Id;
            result.IsVirtual = source.IsVirtual;
            result.Name = source.Name;
            result.OuterId = source.OuterId;

            return result;
        }
    }
}
