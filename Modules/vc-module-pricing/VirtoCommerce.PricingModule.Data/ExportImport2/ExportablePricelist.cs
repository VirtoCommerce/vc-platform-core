using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePricelist : Pricelist, IExportable, IExportViewable, ITabularConvertible
    {
        #region IExportable properties

        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }

        #endregion IExportable properties

        public virtual ExportablePricelist FromModel(Pricelist source)
        {
            Type = nameof(Pricelist);
            Description = source.Description;
            Currency = source.Currency;
            Id = source.Id;
            OuterId = source.OuterId;
            Name = source.Name;
            Code = null;
            ImageUrl = null;
            Parent = null;

            Assignments = source.Assignments?.Select(x => x.Clone() as PricelistAssignment).ToList();
            Prices = source.Prices?.Select(x => x.Clone() as Price).ToList();

            return this;
        }

        #region ITabularConvertible implementation

        public virtual IExportable ToTabular()
        {
            var result = AbstractTypeFactory<TabularPricelist>.TryCreateInstance();

            result.Currency = Currency;
            result.Description = Description;
            result.Id = Id;
            result.Name = Name;

            return result;
        }

        #endregion ITabularConvertible implementation

    }
}
