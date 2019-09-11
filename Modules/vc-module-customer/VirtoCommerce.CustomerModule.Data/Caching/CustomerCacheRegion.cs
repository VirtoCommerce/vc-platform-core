using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
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

        public static void ExpireMemberById(string memberId)
        {
            if (!string.IsNullOrEmpty(memberId) && _memberRegionTokenLookup.TryRemove(memberId, out var token))
            {
                token.Cancel();
            }
        }
    }
}
