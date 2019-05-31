using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ShippingModule.Core.Model.Search
{
    public class ShippingMethodsSearchCriteria : SearchCriteriaBase
    {
        public string StoreId { get; set; }

        public IList<string> Codes { get; set; } = new List<string>();

        public bool? IsActive { get; set; }

        public string TaxType { get; set; }

        //Search only within methods that have changes and persisted
        public bool WithoutTransient { get; set; } = false;
    }
}
