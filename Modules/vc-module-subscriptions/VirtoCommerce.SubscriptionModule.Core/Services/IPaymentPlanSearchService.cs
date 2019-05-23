using System.Threading.Tasks;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;

namespace VirtoCommerce.SubscriptionModule.Core.Services
{
    public interface IPaymentPlanSearchService
    {
        Task<PaymentPlanSearchResult> SearchPlansAsync(PaymentPlanSearchCriteria criteria);
    }
}
