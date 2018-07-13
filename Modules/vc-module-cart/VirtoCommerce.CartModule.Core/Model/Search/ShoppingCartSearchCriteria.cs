using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Core.Model.Search
{
    public class ShoppingCartSearchCriteria : SearchCriteriaBase
    {
        public string Name { get; set; }
        public string CustomerId { get; set; }
        public string StoreId { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string[] CustomerIds { get; set; }
        public string OrganizationId { get; set; }
    }
}
