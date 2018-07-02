using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CartModule.Data.Handlers
{
    public class CartChangedEventHandler : IEventHandler<CartChangedEvent>
    {
        private readonly IDynamicPropertyService _dynamicPropertyService;

        public CartChangedEventHandler(IDynamicPropertyService dynamicPropertyService)
        {
            _dynamicPropertyService = dynamicPropertyService;
        }

        public virtual async Task Handle(CartChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                if (changedEntry.EntryState == EntryState.Added)
                {
                    await _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                }
                else if (changedEntry.EntryState == EntryState.Modified)
                {
                    await _dynamicPropertyService.SaveDynamicPropertyValuesAsync(changedEntry.NewEntry);
                    TryDeleteDynamicPropertiesForRemovedLineItems(changedEntry);
                }
                else if (changedEntry.EntryState == EntryState.Deleted)
                {
                    await _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(changedEntry.NewEntry);
                }
                
            }
        }

        protected virtual void TryDeleteDynamicPropertiesForRemovedLineItems(GenericChangedEntry<ShoppingCart> changedEntry)
        {
            var originalDynPropOwners = changedEntry.OldEntry.GetFlatObjectsListWithInterface<IHasDynamicProperties>()
                                          .Distinct()
                                          .ToList();
            var modifiedDynPropOwners = changedEntry.NewEntry.GetFlatObjectsListWithInterface<IHasDynamicProperties>()
                                         .Distinct()
                                         .ToList();
            var removingDynPropOwners = new List<IHasDynamicProperties>();
            var hasDynPropComparer = AnonymousComparer.Create((IHasDynamicProperties x) => x.Id);
            modifiedDynPropOwners.CompareTo(originalDynPropOwners, hasDynPropComparer, async (state, changed, orig) =>
            {
                if (state == EntryState.Deleted)
                {
                    await _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(orig);
                }

            });

        }
    }
}
