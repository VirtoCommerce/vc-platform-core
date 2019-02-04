using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.CustomerModule.Data.Caching
{
    public class CustomerCacheRegion : CancellableCacheRegion<CustomerCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _memberRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(Member member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }
            var cancellationTokenSource = _memberRegionTokenLookup.GetOrAdd(member.Id, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireInventory(Member member)
        {
            if (_memberRegionTokenLookup.TryRemove(member.Id, out var token))
            {
                token.Cancel();
            }
        }

        public static string CustomersPatternCacheKey => "VirtoCommerce.Platform.Customers.";
    }
}
