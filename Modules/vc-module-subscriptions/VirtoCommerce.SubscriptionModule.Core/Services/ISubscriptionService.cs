using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.SubscriptionModule.Core.Model;

namespace VirtoCommerce.SubscriptionModule.Core.Services
{
    public interface ISubscriptionService
    {
        Task<Subscription[]> GetByIdsAsync(string[] subscriptionIds, string responseGroup = null);
        Task SaveSubscriptionsAsync(Subscription[] subscriptions);
        Task DeleteAsync(string[] ids);
    }
}
