using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.CustomerModule.Core.Events;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CustomerModule.Data.Handlers
{
    public class MemberChangedEventHandler : IEventHandler<MemberChangedEvent>, IEventHandler<MemberChangingEvent>
    {
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly ISeoService _seoService;
        private readonly ICacheBackplane _cacheBackplane;

        public MemberChangedEventHandler(IDynamicPropertyService dynamicPropertyService, ISeoService seoService, ICacheBackplane cacheBackplane)
        {
            _dynamicPropertyService = dynamicPropertyService;
            _seoService = seoService;
            _cacheBackplane = cacheBackplane;
        }

        public async Task Handle(MemberChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                if (changedEntry.EntryState == EntryState.Added)
                {
                    await _cacheBackplane.NotifyChangeAsync(changedEntry.NewEntry.Id, CacheItemChangedEventAction.Add);
                    await _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    if (changedEntry.NewEntry is ISeoSupport seoSupport)
                    {
                        await _seoService.SaveSeoForObjectsAsync(new[] { seoSupport });
                    }
                }
                else if (changedEntry.EntryState == EntryState.Modified)
                {
                    await _cacheBackplane.NotifyChangeAsync(changedEntry.NewEntry.Id, CacheItemChangedEventAction.Update);
                    await _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    if (changedEntry.NewEntry is ISeoSupport seoSupport)
                    {
                        await _seoService.SaveSeoForObjectsAsync(new[] { seoSupport });
                    }
                }
                else if (changedEntry.EntryState == EntryState.Deleted)
                {
                    await _cacheBackplane.NotifyRemoveAsync(changedEntry.NewEntry.Id);
                    await _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    if (changedEntry.NewEntry is ISeoSupport seoSupport)
                    {
                        await _seoService.DeleteSeoForObjectAsync(seoSupport);
                    }
                }
            }
        }

        public Task Handle(MemberChangingEvent message)
        {
            return Task.CompletedTask;
        }
    }
}
