using System;

namespace VirtoCommerce.PricingModule.Core.Model.Search
{
    public class PricesSearchCriteria : PricingSearchCriteria
    {
        //Apply paginate limits to product count instead prices record
        public bool GroupByProducts { get; set; }

        public string PriceListId { get; set; }

        private string[] _priceListIds;
        public string[] PriceListIds
        {
            get
            {
                if (_priceListIds == null && !string.IsNullOrEmpty(PriceListId))
                {
                    _priceListIds = new[] { PriceListId };
                }
                return _priceListIds;
            }
            set
            {
                _priceListIds = value;
            }
        }

        public string ProductId { get; set; }

        private string[] _productIds;
        public string[] ProductIds
        {
            get
            {
                if (_productIds == null && !string.IsNullOrEmpty(ProductId))
                {
                    _productIds = new[] { ProductId };
                }
                return _productIds;
            }
            set
            {
                _productIds = value;
            }
        }

        public DateTime? ModifiedSince { get; set; }
    }
}
