using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.MarketingModule.Web.Model
{
    public class MarketingSearchResult
    {

        public int TotalCount { get; set; }

        public List<Promotion> Promotions { get; set; }
        public List<DynamicContentPlace> ContentPlaces { get; set; }
        public List<DynamicContentItem> ContentItems { get; set; }
        public List<DynamicContentPublication> ContentPublications { get; set; }
        public List<DynamicContentFolder> ContentFolders { get; set; }

    }
}
