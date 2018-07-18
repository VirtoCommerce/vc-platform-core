using System.Collections.Generic;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.CustomerModule.Web.Model
{
    public class VendorSearchResult
    {
        public List<Vendor> Vendors { get; set; }
        public int TotalCount { get; set; }
    }
}
