using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePricelistAssignment : PricelistAssignment, IExportable
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

        public ExportablePricelistAssignment FromModel(PricelistAssignment source)
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
    }
}
