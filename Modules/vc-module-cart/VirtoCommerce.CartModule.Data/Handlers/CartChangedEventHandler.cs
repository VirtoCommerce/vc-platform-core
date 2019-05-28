using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CartModule.Data.Handlers
{
    public class CartChangedEventHandler : IEventHandler<CartChangedEvent>, IEventHandler<CartChangeEvent>
    {
        private readonly IDynamicPropertyService _dynamicPropertyService;

        public CartChangedEventHandler(IDynamicPropertyService dynamicPropertyService)
        {
            _dynamicPropertyService = dynamicPropertyService;
        }

        public virtual Task Handle(CartChangedEvent message)
        {
            foreach (var changedEntry in message.ChangedEntries)
            {
                if (changedEntry.EntryState == EntryState.Added)
                {
                    BackgroundJob.Enqueue(() => SaveDynamicPropertyValuesInBackground(changedEntry.NewEntry));
                }
                else if (changedEntry.EntryState == EntryState.Modified)
                {
                    BackgroundJob.Enqueue(() =>
                        SaveAndDeleteForRemovedLineItemsDynamicPropertyValuesInBackground(changedEntry.NewEntry, changedEntry.OldEntry));
                }
                else if (changedEntry.EntryState == EntryState.Deleted)
                {
                    BackgroundJob.Enqueue(() => DeleteDynamicPropertyValuesInBackground(changedEntry.NewEntry));
                }

            }

            return Task.CompletedTask;
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public void SaveDynamicPropertyValuesInBackground(IHasDynamicProperties entry)
        {
            _dynamicPropertyService.SaveDynamicPropertyValuesAsync(entry).GetAwaiter().GetResult();
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public void DeleteDynamicPropertyValuesInBackground(IHasDynamicProperties entry)
        {
            _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(entry).GetAwaiter().GetResult();
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public void SaveAndDeleteForRemovedLineItemsDynamicPropertyValuesInBackground(ShoppingCart newEntry, ShoppingCart oldEntry)
        {
            _dynamicPropertyService.SaveDynamicPropertyValuesAsync(newEntry).GetAwaiter().GetResult();
            TryDeleteDynamicPropertiesForRemovedLineItems(newEntry, oldEntry).GetAwaiter().GetResult();
        }


        protected virtual async Task TryDeleteDynamicPropertiesForRemovedLineItems(ShoppingCart newEntry, ShoppingCart oldEntry)
        {
            var originalDynPropOwners = oldEntry.GetFlatObjectsListWithInterface<IHasDynamicProperties>()
                                          .Distinct()
                                          .ToList();
            var modifiedDynPropOwners = newEntry.GetFlatObjectsListWithInterface<IHasDynamicProperties>()
                                         .Distinct()
                                         .ToList();
            var removingDynPropOwners = new List<IHasDynamicProperties>();
            var hasDynPropComparer = AnonymousComparer.Create((IHasDynamicProperties x) => x.Id);
            modifiedDynPropOwners.CompareTo(originalDynPropOwners, hasDynPropComparer, (state, changed, orig) =>
            {
                if (state == EntryState.Deleted)
                {
                    removingDynPropOwners.Add(orig);
                }
            });

            foreach (var dynamicPropertiese in removingDynPropOwners)
            {
                await _dynamicPropertyService.DeleteDynamicPropertyValuesAsync(dynamicPropertiese);
            }
        }

        public Task Handle(CartChangeEvent message)
        {
            return Task.CompletedTask;
        }
    }
}
