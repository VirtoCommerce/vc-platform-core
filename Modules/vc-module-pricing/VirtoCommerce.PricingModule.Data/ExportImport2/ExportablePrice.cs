using System;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class ExportablePrice : Entity, IExportViewable
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

        #region IExportViewable implementation
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }
        #endregion

        public static ExportablePrice FromModel(Price source)
        {
            var result = AbstractTypeFactory<ExportablePrice>.TryCreateInstance();

            result.Type = nameof(PricelistAssignment);
            result.Currency = source.Currency;
            result.Id = source.Id;
            result.List = source.List;
            result.MinQuantity = source.MinQuantity;
            result.PricelistId = source.PricelistId;
            result.Pricelist = source.Pricelist;
            result.ProductId = source.ProductId;
            result.Sale = source.Sale;
            result.OuterId = source.OuterId;
            result.EffectiveValue = source.EffectiveValue;

            return result;
        }
    }
}
