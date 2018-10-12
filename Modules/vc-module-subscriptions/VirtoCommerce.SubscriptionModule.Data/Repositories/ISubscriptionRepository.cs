using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.SubscriptionModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SubscriptionModule.Data.Repositories
{
    public interface ISubscriptionRepository : IRepository
    {
        IQueryable<PaymentPlanEntity> PaymentPlans { get; }
        IQueryable<SubscriptionEntity> Subscriptions { get; }
    
        Task<PaymentPlanEntity[]> GetPaymentPlansByIdsAsync(string[] ids);
        Task RemovePaymentPlansByIdsAsync(string[] ids);

        Task<SubscriptionEntity[]> GetSubscriptionsByIdsAsync(string[] ids, string responseGroup = null);
        Task RemoveSubscriptionsByIdsAsync(string[] ids);
    }
}
