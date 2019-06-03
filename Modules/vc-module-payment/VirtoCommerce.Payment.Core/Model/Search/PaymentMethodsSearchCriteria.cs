using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PaymentModule.Core.Model.Search
{
    public class PaymentMethodsSearchCriteria : SearchCriteriaBase
    {
        public string StoreId { get; set; }

        public IList<string> Codes { get; set; } = new List<string>();

        public bool? IsActive { get; set; }

        //Search only within tax providers that have changes and persisted
        public bool WithoutTransient { get; set; } = false;
    }
}
