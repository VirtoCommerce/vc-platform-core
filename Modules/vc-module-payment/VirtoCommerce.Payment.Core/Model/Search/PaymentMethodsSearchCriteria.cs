using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PaymentModule.Core.Model.Search
{
    public class PaymentMethodsSearchCriteria : SearchCriteriaBase
    {
        public string StoreId { get; set; }

        public IList<string> Codes { get; set; } = new List<string>();

        public bool? IsActive { get; set; }
    }
}
