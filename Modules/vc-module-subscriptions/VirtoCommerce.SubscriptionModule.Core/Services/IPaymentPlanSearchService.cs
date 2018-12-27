using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;

namespace VirtoCommerce.SubscriptionModule.Core.Services
{
    public interface IPaymentPlanSearchService
    {
        Task<GenericSearchResult<PaymentPlan>> SearchPlansAsync(PaymentPlanSearchCriteria criteria);
    }
}
