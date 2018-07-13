using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.OrdersModule.Data.Handlers
{
    /// <summary>
    /// Adjust inventory for ordered items 
    /// </summary>
    public class AdjustInventoryOrderChangedEventHandler : IEventHandler<OrderChangedEvent>
    {
        private readonly IInventoryService _inventoryService;
        private readonly ISettingsManager _settingsManager;
        private readonly IStoreService _storeService;

        public AdjustInventoryOrderChangedEventHandler(IInventoryService inventoryService, IStoreService storeService, ISettingsManager settingsManager)
        {
            _inventoryService = inventoryService;
            _settingsManager = settingsManager;
            _storeService = storeService;
        }


        public virtual Task Handle(OrderChangedEvent message)
        {
            if (_settingsManager.GetValue("Order.AdjustInventory", true))
            {
                foreach (var changedEntry in message.ChangedEntries)
                {
                    TryAdjustOrderInventory(changedEntry);
                }
            }
            return Task.CompletedTask;
        }

        protected virtual async Task TryAdjustOrderInventory(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var customerOrder = changedEntry.OldEntry;
            //Skip prototypes
            if (customerOrder.IsPrototype)
                return;
                       
            var origLineItems = new LineItem[] { };
            var changedLineItems = new LineItem[] { };

            if (changedEntry.EntryState == EntryState.Added)
            {
                changedLineItems = changedEntry.NewEntry.Items.ToArray();
            }
            else if (changedEntry.EntryState == EntryState.Deleted)
            {
                origLineItems = changedEntry.OldEntry.Items.ToArray();
            }
            else
            {
                origLineItems = changedEntry.OldEntry.Items.ToArray();
                changedLineItems = changedEntry.NewEntry.Items.ToArray();
            }
            var inventoryAdjustments = new HashSet<InventoryInfo>();
            //Load all inventories records for all changes and old order items
            var inventoryInfos = await _inventoryService.GetProductsInventoryInfosAsync(origLineItems.Select(x => x.ProductId).Concat(changedLineItems.Select(x => x.ProductId)).Distinct().ToArray());
            changedLineItems.CompareTo(origLineItems, EqualityComparer<LineItem>.Default, async (state, changed, orig) =>
            {
                await AdjustInventory(inventoryInfos, inventoryAdjustments, customerOrder, state, changed, orig);
            });
            //Save inventories adjustments
            if (inventoryAdjustments != null)
            {
                await _inventoryService.SaveChangesAsync(inventoryAdjustments);
            }

        }

        protected virtual async Task AdjustInventory(IEnumerable<InventoryInfo> inventories, HashSet<InventoryInfo> changedInventories, CustomerOrder order, EntryState action, LineItem changedLineItem, LineItem origLineItem)
        {
            var fulfillmentCenterId = await GetFullfilmentCenterForLineItem(order, origLineItem);
            var inventoryInfo = inventories.Where(x=> x.FulfillmentCenterId == (fulfillmentCenterId ?? x.FulfillmentCenterId))
                                           .FirstOrDefault(x => x.ProductId.EqualsInvariant(origLineItem.ProductId));
            if (inventoryInfo != null)
            {
                int delta;

                if (action == EntryState.Added)
                {
                    delta = -origLineItem.Quantity;
                }
                else if (action == EntryState.Deleted)
                {
                    delta = origLineItem.Quantity;
                }
                else
                {
                    delta = origLineItem.Quantity - changedLineItem.Quantity;
                }

                if (delta != 0)
                {
                    changedInventories.Add(inventoryInfo);
                    inventoryInfo.InStockQuantity += delta;
                    inventoryInfo.InStockQuantity = Math.Max(0, inventoryInfo.InStockQuantity);
                }
            }
        }

        /// <summary>
        /// Returns a fulfillment center identifier much suitable for given order line item
        /// </summary>
        /// <param name="order"></param>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        protected virtual async Task<string> GetFullfilmentCenterForLineItem(CustomerOrder order, LineItem lineItem)
        {
            if(order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            if (lineItem == null)
            {
                throw new ArgumentNullException(nameof(lineItem));
            }

            var result = lineItem.FulfillmentCenterId;

            if(string.IsNullOrEmpty(result))
            {
                //Try to find a concrete shipment for given line item 
                var shipment = order.Shipments?.Where(x => x.Items != null)
                                                     .FirstOrDefault(s => s.Items.FirstOrDefault(i => i.LineItemId == lineItem.Id) != null);
                if (shipment == null)
                {
                    shipment = order.Shipments?.FirstOrDefault();
                }
                result = shipment?.FulfillmentCenterId;
            }

            //Use a default fulfillment center defined for store
            if(string.IsNullOrEmpty(result))
            {
                var store = await _storeService.GetByIdAsync(order.StoreId);
                result = store?.MainFulfillmentCenterId;
            }
            return result;
        }
    }
}
