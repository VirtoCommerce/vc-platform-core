using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Domain.Customer.Model;

namespace VirtoCommerce.CustomerModule.Web.Model
{
    public class VendorSearchResult
    {
        public List<Vendor> Vendors { get; set; }
        public int TotalCount { get; set; }
    }
}