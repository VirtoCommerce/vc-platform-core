using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePricelistAssignment : PricelistAssignment, IExportable, IExportViewable, ITabularConvertible
    {
        #region IExportable properties

        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }

        #endregion IExportable properties

        #region Properties specific to universal viewer

        public string CatalogName { get; set; }
        public string PricelistName { get; set; }

        #endregion

        public virtual ExportablePricelistAssignment FromModel(PricelistAssignment source)
        {
            Type = nameof(PricelistAssignment);
            CatalogId = source.CatalogId;
            EndDate = source.EndDate;
            Id = source.Id;
            PricelistId = source.PricelistId;
            Description = source.Description;
            Priority = source.Priority;
            StartDate = source.StartDate;
            OuterId = source.OuterId;
            Name = source.Name;
            Code = null;
            ImageUrl = null;
            Parent = null;

            return this;
        }

        #region ITabularConvertible implementation

        public virtual IExportable ToTabular()
        {
            var result = AbstractTypeFactory<TabularPricelistAssignment>.TryCreateInstance();

            result.CatalogId = CatalogId;
            result.CatalogName = CatalogName;
            result.Description = Description;
            result.EndDate = EndDate;
            result.Id = Id;
            result.Name = Name;
            result.PricelistId = PricelistId;
            result.PricelistName = PricelistName;
            result.Priority = Priority;
            result.StartDate = StartDate;

            return result;
        }

        #endregion ITabularConvertible implementation
    }
}
