using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePrice : ExportableEntity<ExportablePrice>
    {
        #region Price properties
        public string PricelistId { get; set; }
        public Pricelist Pricelist { get; set; }
        public string Currency { get; set; }
        public string ProductId { get; set; }
        public decimal? Sale { get; set; }
        public decimal List { get; set; }
        public int MinQuantity { get; set; }
        public decimal EffectiveValue { get; set; }
        public string OuterId { get; set; }
        #endregion

        #region Properties specific to universal viewer
        public string PricelistName { get; set; }
        public string ProductName { get; set; }
        #endregion

        public ExportablePrice FromModel(Price source)
        {
            Type = nameof(PricelistAssignment);
            Currency = source.Currency;
            Id = source.Id;
            List = source.List;
            MinQuantity = source.MinQuantity;
            PricelistId = source.PricelistId;
            Pricelist = source.Pricelist;
            ProductId = source.ProductId;
            Sale = source.Sale;
            OuterId = source.OuterId;
            EffectiveValue = source.EffectiveValue;
            return this;
        }
    }
}
