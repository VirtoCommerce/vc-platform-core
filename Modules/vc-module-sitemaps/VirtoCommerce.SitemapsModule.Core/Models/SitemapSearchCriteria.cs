using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class SitemapSearchCriteria : SearchCriteriaBase
    {
        public string StoreId { get; set; }

        public string Location { get; set; }
    }
}
