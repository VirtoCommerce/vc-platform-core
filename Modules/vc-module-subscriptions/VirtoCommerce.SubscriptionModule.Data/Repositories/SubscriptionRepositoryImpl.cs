using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<PaymentPlanEntity[]> GetPaymentPlansByIdsAsync(string[] ids)
        {
            var query = PaymentPlans.Where(x => ids.Contains(x.Id));
            return await query.ToArrayAsync();
        }

        public async Task<SubscriptionEntity[]> GetSubscriptionsByIdsAsync(string[] ids, string responseGroup = null)
        {
            var result = await Subscriptions.Where(x => ids.Contains(x.Id)).ToArrayAsync();
            return result;
        }

        public async Task RemovePaymentPlansByIdsAsync(string[] ids)
        {
            var paymentPlans = await GetPaymentPlansByIdsAsync(ids);
            foreach (var paymentPlan in paymentPlans)
            {
                Remove(paymentPlan);
            }
        }

        public async Task RemoveSubscriptionsByIdsAsync(string[] ids)
        {
            var subscriptions = await GetSubscriptionsByIdsAsync(ids);
            foreach (var subscription in subscriptions)
            {
                Remove(subscription);
            }
        } 
        #endregion
    }
}
