using System.Linq;
using VirtoCommerce.SubscriptionModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SubscriptionModule.Data.Repositories
{
    public interface ISubscriptionRepository : IRepository
    {
        IQueryable<PaymentPlanEntity> PaymentPlans { get; }
        IQueryable<SubscriptionEntity> Subscriptions { get; }
    
        PaymentPlanEntity[] GetPaymentPlansByIds(string[] ids);
        void RemovePaymentPlansByIds(string[] ids);

        SubscriptionEntity[] GetSubscriptionsByIds(string[] ids, string responseGroup = null);
        void RemoveSubscriptionsByIds(string[] ids);
    }
}
