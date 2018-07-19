using System;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.CustomerModule.Core.Events;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CustomerModule.Data.Handlers
{
    public class MemberChangedEventHandler: IEventHandler<MemberChangedEvent>, IEventHandler<MemberChangingEvent>
    {
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly ISeoService _seoService;
        public MemberChangedEventHandler(IDynamicPropertyService dynamicPropertyService, ISeoService seoService)
        {
            _dynamicPropertyService = dynamicPropertyService;
            _seoService = seoService;
        }

        public async Task Handle(MemberChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                if (changedEntry.EntryState == EntryState.Added)
                {
                    await _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    if (changedEntry.NewEntry is ISeoSupport seoSupport)
                    {
                        await _seoService.SaveSeoForObjectsAsync(new[] { seoSupport });
                    }
                }
                else if (changedEntry.EntryState == EntryState.Modified)
                {
                    await _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    if (changedEntry.NewEntry is ISeoSupport seoSupport)
                    {
                        await _seoService.SaveSeoForObjectsAsync(new[] { seoSupport });
                    }
                }
                else if (changedEntry.EntryState == EntryState.Deleted)
                {
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
