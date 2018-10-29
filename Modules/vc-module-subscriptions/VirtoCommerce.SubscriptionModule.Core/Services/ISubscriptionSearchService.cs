using System.Threading.Tasks;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SubscriptionModule.Core.Services
{
    public interface ISubscriptionSearchService
    {
        Task<GenericSearchResult<Subscription>> SearchSubscriptionsAsync(SubscriptionSearchCriteria criteria);
    }
}
