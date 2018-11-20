using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model;

namespace VirtoCommerce.SubscriptionModule.Core.Services
{
    /// <summary>
    /// Responsible for programmatically working with subscription
    /// </summary>
    public interface ISubscriptionBuilder
    {
        Subscription Subscription { get; }
        /// <summary>
        /// Capture given subscription for future manipulation
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        ISubscriptionBuilder TakeSubscription(Subscription subscription);    
        /// <summary>
        /// Actualize captured subscription (Statuses, Balance etc)
        /// </summary>
        /// <returns></returns>
        Task<ISubscriptionBuilder> ActualizeAsync();
        /// <summary>
        /// Create new subscription for given customer order with contains items selling by payment plan 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<Subscription> TryCreateSubscriptionFromOrderAsync(CustomerOrder order);
        /// <summary>
        /// Attempt to create new recurrent order with subscription recurring settings
        /// </summary>
        /// <returns></returns>
        Task<CustomerOrder> TryToCreateRecurrentOrderAsync(bool forceCreation = false);
        /// <summary>
        /// Cancel subscription
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        ISubscriptionBuilder CancelSubscription(string reason);
    }
}
