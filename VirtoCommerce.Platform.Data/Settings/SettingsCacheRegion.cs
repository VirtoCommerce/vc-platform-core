using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Data.Settings
{
    public class SettingsCacheRegion : CancellableCacheRegion<SettingsCacheRegion>
    {
        private static readonly ConcurrentDictionary<SettingEntry, CancellationTokenSource> _settingsRegionTokenLookup =  new ConcurrentDictionary<SettingEntry, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(SettingEntry settingEntry)
        {
            if (settingEntry == null)
            {
                throw new ArgumentNullException(nameof(settingEntry));
            }
            var cancellationTokenSource = _settingsRegionTokenLookup.GetOrAdd(settingEntry, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireSetting(SettingEntry settingEntry)
        {
            if (_settingsRegionTokenLookup.TryRemove(settingEntry, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
