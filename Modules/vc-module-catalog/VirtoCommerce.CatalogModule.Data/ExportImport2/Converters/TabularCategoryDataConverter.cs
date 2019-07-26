using System;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport.Converters
{
    public class TabularCategoryDataConverter : ITabularDataConverter
    {
        public object ToTabular(object obj)
        {
            var source = obj as Category ?? throw new ArgumentException(nameof(obj));
            var result = AbstractTypeFactory<TabularCategory>.TryCreateInstance();

            result.CatalogId = source.CatalogId;
            result.Catalog = source.Catalog;
            result.ParentId = source.ParentId;
            result.Parent = source.Parent;
            result.Code = source.Code;
            result.Name = source.Name;
            result.Outline = source.Outline;
            result.Path = source.Path;
            result.IsVirtual = source.IsVirtual;
            result.Level = source.Level;
            result.PackageType = source.PackageType;
            result.Priority = source.Priority;
            result.IsActive = source.IsActive;
            result.OuterId = source.OuterId;
            result.TaxType = source.TaxType;
            result.SeoObjectType = source.SeoObjectType;
            result.ImgSrc = source.ImgSrc;
            result.IsInherited = source.IsInherited;

            return result;
        }
    }
}
