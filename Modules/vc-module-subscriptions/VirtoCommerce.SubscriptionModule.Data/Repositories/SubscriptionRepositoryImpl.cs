using System.Linq;
using VirtoCommerce.SubscriptionModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.SubscriptionModule.Data.Repositories
{
    public class SubscriptionRepositoryImpl : DbContextRepositoryBase<SubscriptionDbContext>, ISubscriptionRepository
    {
        public SubscriptionRepositoryImpl(SubscriptionDbContext context)
            : base(context)
        {
        }

        #region ISubscriptionRepository members    

        public IQueryable<PaymentPlanEntity> PaymentPlans => DbContext.Set<PaymentPlanEntity>();
        public IQueryable<SubscriptionEntity> Subscriptions => DbContext.Set<SubscriptionEntity>();

        public PaymentPlanEntity[] GetPaymentPlansByIds(string[] ids)
        {
            var query = PaymentPlans.Where(x => ids.Contains(x.Id));
            return query.ToArray();
        }

        public SubscriptionEntity[] GetSubscriptionsByIds(string[] ids, string responseGroup = null)
        {
            var result = Subscriptions.Where(x => ids.Contains(x.Id)).ToArray();
            return result;
        }

        public void RemovePaymentPlansByIds(string[] ids)
        {
            var paymentPlans = GetPaymentPlansByIds(ids);
            foreach (var paymentPlan in paymentPlans)
            {
                Remove(paymentPlan);
            }
        }

        public void RemoveSubscriptionsByIds(string[] ids)
        {
            var subscriptions = GetSubscriptionsByIds(ids);
            foreach (var subscription in subscriptions)
            {
                Remove(subscription);
            }
        } 
        #endregion
    }
}
