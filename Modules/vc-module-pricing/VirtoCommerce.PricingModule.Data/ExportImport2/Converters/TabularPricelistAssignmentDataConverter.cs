using System;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport.Converters
{
    public class TabularPricelistAssignmentDataConverter : ITabularDataConverter
    {
        public virtual object ToTabular(object obj)
        {
            var source = obj as PricelistAssignment ?? throw new ArgumentException(nameof(obj));
            var result = AbstractTypeFactory<TabularPricelistAssignment>.TryCreateInstance();

            result.CatalogId = source.CatalogId;
            result.Description = source.Description;
            result.EndDate = source.EndDate;
            result.Id = source.Id;
            result.Name = source.Name;
            result.PricelistId = source.PricelistId;
            result.Priority = source.Priority;
            result.StartDate = source.StartDate;

            return result;
        }
    }
}
