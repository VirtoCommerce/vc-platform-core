using System;
using VirtoCommerce.ExportModule.Core.Model;
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

        public ExportablePricelistAssignment FromModel(PricelistAssignment source)
        {
            Type = nameof(PricelistAssignment);
            CatalogId = source.CatalogId;
            EndDate = source.EndDate;
            Id = source.Id;
            PricelistId = source.PricelistId;
            Description = source.Description;
            Pricelist = source.Pricelist;
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
