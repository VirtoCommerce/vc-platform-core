using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Events;
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

        public async Task Handle(DynamicContentItemChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                switch (changedEntry.EntryState)
                {
                    case EntryState.Added:
                    case EntryState.Modified:
                        await _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                        break;
                    case EntryState.Deleted:
                        await _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(changedEntry.NewEntry);
                        break;

                }
            }
        }
    }
}
