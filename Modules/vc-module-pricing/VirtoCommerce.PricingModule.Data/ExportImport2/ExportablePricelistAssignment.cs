using System;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePricelistAssignment : ExportableEntity<ExportablePricelistAssignment>
    {
        #region PricelistAssignment properties
        public string CatalogId { get; set; }
        public string PricelistId { get; set; }
        public Pricelist Pricelist { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OuterId { get; set; }
        #endregion

        #region Properties specific to universal viewer
        public string CatalogName { get; set; }
        public string PricelistName { get; set; }
        #endregion

        public static ExportablePricelistAssignment FromModel(PricelistAssignment source)
        {
            var result = AbstractTypeFactory<ExportablePricelistAssignment>.TryCreateInstance();

            result.Type = nameof(PricelistAssignment);
            result.CatalogId = source.CatalogId;
            result.EndDate = source.EndDate;
            result.Id = source.Id;
            result.PricelistId = source.PricelistId;
            result.Description = source.Description;
            result.Pricelist = source.Pricelist;
            result.Priority = source.Priority;
            result.StartDate = source.StartDate;
            result.OuterId = source.OuterId;
            result.Name = source.Name;
            result.Code = null;
            result.ImageUrl = null;
            result.Parent = null;

            return result;
        }
    }
}
