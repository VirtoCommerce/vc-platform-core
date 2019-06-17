using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SitemapsModule.Core.Models.Search
{
    public class SitemapSearchCriteria : SearchCriteriaBase
    {
        public string StoreId { get; set; }

        public string Location { get; set; }
    }
}
