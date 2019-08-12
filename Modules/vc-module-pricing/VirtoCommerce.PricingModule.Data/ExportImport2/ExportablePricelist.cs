using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePricelist : Pricelist, IExportable
    {
        #region IExportable properties

        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }

        #endregion IExportable properties

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
