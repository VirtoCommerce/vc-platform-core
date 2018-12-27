using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.SubscriptionModule.Core.Model;

namespace VirtoCommerce.SubscriptionModule.Data.Caching
{
    public class PaymentPlanCacheRegion : CancellableCacheRegion<PaymentPlanCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _paymentPlanRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(PaymentPlan paymentPlan)
        {
            if (paymentPlan == null)
            {
                throw new ArgumentNullException(nameof(paymentPlan));
            }
            var cancellationTokenSource = _paymentPlanRegionTokenLookup.GetOrAdd(paymentPlan.Id, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpirePaymentPlan(PaymentPlan paymentPlan)
        {
            if (_paymentPlanRegionTokenLookup.TryRemove(paymentPlan.Id, out var token))
            {
                token.Cancel();
            }
        }
    }
}
