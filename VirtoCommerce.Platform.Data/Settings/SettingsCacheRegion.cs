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
        private static readonly ConcurrentDictionary<ObjectSettingEntry, CancellationTokenSource> _settingsRegionTokenLookup = new ConcurrentDictionary<ObjectSettingEntry, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(ObjectSettingEntry settingEntry)
        {
            if (settingEntry == null)
            {
                throw new ArgumentNullException(nameof(settingEntry));
            }
            var cancellationTokenSource = _settingsRegionTokenLookup.GetOrAdd(settingEntry, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireSetting(ObjectSettingEntry settingEntry)
        {
            if (_settingsRegionTokenLookup.TryRemove(settingEntry, out var token))
            {
                token.Cancel();
            }
        }
    }
}
