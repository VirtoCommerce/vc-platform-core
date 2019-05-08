using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PaymentModule.Core.Models.Search
{
    public class PaymentMethodsSearchCriteria : SearchCriteriaBase
    {
        public string StoreId { get; set; }

        public string[] Codes { get; set; }

        public bool? IsActive { get; set; }
    }
}
