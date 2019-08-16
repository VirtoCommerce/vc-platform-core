namespace VirtoCommerce.PricingModule.Core.Model.Search
{
    public class PricelistAssignmentsSearchCriteria : PricingSearchCriteria
    {
        public string PriceListId { get; set; }
        public string[] CatalogIds { get; set; }
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
            set => _priceListIds = value;
        }

    }
}
