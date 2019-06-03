using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.MarketingModule.Core.Events;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Data.Handlers
{
    public class DynamicContentItemEventHandlers : IEventHandler<DynamicContentItemChangedEvent>
    {
        private readonly IDynamicPropertyService _dynamicPropertyService;

        public DynamicContentItemEventHandlers(IDynamicPropertyService dynamicPropertyService)
        {
            _dynamicPropertyService = dynamicPropertyService;
        }

        public Task Handle(DynamicContentItemChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                switch (changedEntry.EntryState)
                {
                    case EntryState.Added:
                    case EntryState.Modified:
                        BackgroundJob.Enqueue(() => SaveDynamicPropertyValuesInBackground(changedEntry.NewEntry));
                        break;
                    case EntryState.Deleted:
                        BackgroundJob.Enqueue(() => DeleteDynamicPropertyValuesInBackground(changedEntry.NewEntry));
                        break;
                }
            }

            return Task.CompletedTask;
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public void SaveDynamicPropertyValuesInBackground(DynamicContentItem newEntry)
        {
            var dynamicProperties = (IHasDynamicProperties)newEntry;
            _dynamicPropertyService.SaveDynamicPropertyValuesAsync(dynamicProperties).GetAwaiter().GetResult();
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public void DeleteDynamicPropertyValuesInBackground(DynamicContentItem newEntry)
        {
            var dynamicProperties = (IHasDynamicProperties)newEntry;
            _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(dynamicProperties).GetAwaiter().GetResult();
        }
    }
}
