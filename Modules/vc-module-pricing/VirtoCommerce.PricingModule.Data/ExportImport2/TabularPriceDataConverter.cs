using System;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport2
{
    public class TabularPriceDataConverter : ITabularDataConverter
    {
        public object ToTabular(object obj)
        {
            var price = obj as Price ?? throw new ArgumentException(nameof(obj));
            var tabularPrice = new TabularPrice();

            tabularPrice.Currency = price.Currency;
            tabularPrice.EndDate = price.EndDate;
            tabularPrice.Id = price.Id;
            tabularPrice.List = price.List;
            tabularPrice.MinQuantity = price.MinQuantity;
            tabularPrice.PricelistId = price.PricelistId;
            tabularPrice.ProductId = price.ProductId;
            tabularPrice.Sale = price.Sale;
            tabularPrice.StartDate = price.StartDate;

            return tabularPrice;
        }
    }
}
