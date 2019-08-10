using System.Collections.Generic;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePricelist : ExportableEntity<ExportablePricelist>
    {
        #region Pricelist properties
        public string Description { get; set; }
        public string Currency { get; set; }
        public string OuterId { get; set; }
        public ICollection<Price> Prices { get; set; }
        public ICollection<PricelistAssignment> Assignments { get; set; }
        #endregion

        public ExportablePricelist FromModel(Pricelist source)
        {
            Type = nameof(Pricelist);
            Description = source.Description;
            Currency = source.Currency;
            Id = source.Id;
            OuterId = source.OuterId;
            Prices = source.Prices;
            Assignments = source.Assignments;
            Name = source.Name;
            Code = null;
            ImageUrl = null;
            Parent = null;

            return this;
        }
    }
}
