using System.Collections.Generic;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePricelist : Entity, IExportViewable
    {
        #region Pricelist properties
        public string Description { get; set; }
        public string Currency { get; set; }
        public string OuterId { get; set; }
        public ICollection<Price> Prices { get; set; }
        public ICollection<PricelistAssignment> Assignments { get; set; }
        #endregion

        #region IExportViewable implementation
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }
        #endregion

        public static ExportablePricelist FromModel(Pricelist source)
        {
            var result = AbstractTypeFactory<ExportablePricelist>.TryCreateInstance();

            result.Type = nameof(Pricelist);
            result.Description = source.Description;
            result.Currency = source.Currency;
            result.Id = source.Id;
            result.OuterId = source.OuterId;
            result.Prices = source.Prices;
            result.Assignments = source.Assignments;
            result.Name = source.Name;
            result.Code = null;
            result.ImageUrl = null;
            result.Parent = null;

            return result;
        }
    }
}
