using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.OrdersModule.Core;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.OrdersModule.Data.Handlers
{
    /// <summary>
    /// Adjust inventory for ordered items 
    /// </summary>
    public class AdjustInventoryOrderChangedEventHandler : IEventHandler<OrderChangedEvent>
    {
        public class ProductInventoryChange
        {
            public string ProductId { get; set; }
            public string FulfillmentCenterId { get; set; }
            public int QuantityDelta { get; set; }
        }

        private readonly IInventoryService _inventoryService;
        private readonly ISettingsManager _settingsManager;
        private readonly IStoreService _storeService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inventoryService">Inventory service to use for adjusting inventories.</param>
        /// <param name="storeService">Implementation of store service.</param>
        /// <param name="settingsManager">Implementation of settings manager.</param>
        public AdjustInventoryOrderChangedEventHandler(IInventoryService inventoryService, IStoreService storeService, ISettingsManager settingsManager)
        {
            _inventoryService = inventoryService;
            _settingsManager = settingsManager;
            _storeService = storeService;
        }

        /// <summary>
        /// Handles the order changed event by queueing a Hangfire task that adjusts inventories.
        /// </summary>
        /// <param name="message">Order changed event to handle.</param>
        /// <returns>A task that allows to <see langword="await"/> this method.</returns>
        public virtual async Task Handle(OrderChangedEvent message)
        {
            if (_settingsManager.GetValue(ModuleConstants.Settings.General.OrderAdjustInventory.Name, true))
            {
                foreach (var changedEntry in message.ChangedEntries)
                {
                    var customerOrder = changedEntry.OldEntry;
                    //Do not process prototypes
                    if (!customerOrder.IsPrototype)
                    {
                        var itemChanges = await GetProductInventoryChangesFor(changedEntry);
                        if (itemChanges.Any())
                        {
                            //Background task is used here to  prevent concurrent update conflicts that can be occur during applying of adjustments for same inventory object
                            BackgroundJob.Enqueue(() => TryAdjustOrderInventoryBackgroundJob(itemChanges));
                        }
                    }
                }
            }
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public async Task TryAdjustOrderInventoryBackgroundJob(ProductInventoryChange[] productInventoryChanges)
        {
            await TryAdjustOrderInventory(productInventoryChanges);
        }

        /// <summary>
        /// Forms a list of product inventory changes for inventory adjustment. This method is intended for unit-testing only,
        /// and there should be no need to call it from the production code.
        /// </summary>
        /// <param name="changedEntry">The entry that describes changes made to order.</param>
        /// <returns>Array of required product inventory changes.</returns>
        public virtual Task<ProductInventoryChange[]> GetProductInventoryChangesFor(GenericChangedEntry<CustomerOrder> changedEntry)
        {
            var customerOrder = changedEntry.NewEntry;
            var customerOrderShipments = customerOrder.Shipments?.ToArray();

            var oldLineItems = changedEntry.OldEntry.Items?.ToArray() ?? Array.Empty<LineItem>();
            var newLineItems = changedEntry.NewEntry.Items?.ToArray() ?? Array.Empty<LineItem>();

            var itemChanges = new List<ProductInventoryChange>();
            newLineItems.CompareTo(oldLineItems, EqualityComparer<LineItem>.Default, async (state, changedItem, originalItem) =>
            {
                var newQuantity = changedItem.Quantity;
                var oldQuantity = originalItem.Quantity;

                if (changedEntry.EntryState == EntryState.Added || state == EntryState.Added)
                {
                    oldQuantity = 0;
                }
                else if (changedEntry.EntryState == EntryState.Deleted || state == EntryState.Deleted)
                {
                    newQuantity = 0;
                }

                if (oldQuantity != newQuantity)
                {
                    itemChanges.Add(new ProductInventoryChange
                    {
                        ProductId = changedItem.ProductId,
                        QuantityDelta = newQuantity - oldQuantity,
                        FulfillmentCenterId = await GetFullfilmentCenterForLineItem(changedItem, customerOrder.StoreId, customerOrderShipments)
                    });
                }
            });
            //Do not return unchanged records
            return Task.FromResult(itemChanges.Where(x => x.QuantityDelta != 0).ToArray());
        }


        protected virtual async Task TryAdjustOrderInventory(ProductInventoryChange[] productInventoryChanges)
        {
            var inventoryAdjustments = new HashSet<InventoryInfo>();
            //Load all inventories records for all changes and old order items
            var productIds = productInventoryChanges.Select(x => x.ProductId).Distinct().ToArray();
            var inventoryInfos = await _inventoryService.GetProductsInventoryInfosAsync(productIds);
            foreach (var productInventoryChange in productInventoryChanges)
            {
                var inventoryInfo = inventoryInfos.Where(x => x.FulfillmentCenterId == (productInventoryChange.FulfillmentCenterId ?? x.FulfillmentCenterId))
                    .FirstOrDefault(x => x.ProductId.EqualsInvariant(productInventoryChange.ProductId));
                if (inventoryInfo != null)
                {
                    inventoryAdjustments.Add(inventoryInfo);

                    // NOTE: itemChange.QuantityDelta keeps the count of additional items that should be taken from the inventory.
                    //       That's why we subtract it from the current in-stock quantity instead of adding it.
                    inventoryInfo.InStockQuantity = Math.Max(0, inventoryInfo.InStockQuantity - productInventoryChange.QuantityDelta);
                }
            }
            if (inventoryAdjustments.Any())
            {
                //Save inventories adjustments
                await _inventoryService.SaveChangesAsync(inventoryAdjustments);
            }
        }

        [Obsolete("This method is not used anymore. Please use the TryAdjustOrderInventory(OrderLineItemChange[]) method instead.")]
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
            var fulfillmentCenterId = await GetFullfilmentCenterForLineItem(origLineItem, order.StoreId, order.Shipments?.ToArray());
            var inventoryInfo = inventories.Where(x => x.FulfillmentCenterId == (fulfillmentCenterId ?? x.FulfillmentCenterId))
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
        /// <param name="lineItem"></param>
        /// <param name="orderStoreId"></param>
        /// <param name="orderShipments"></param>
        /// <returns></returns>
        protected virtual async Task<string> GetFullfilmentCenterForLineItem(LineItem lineItem, string orderStoreId, Shipment[] orderShipments)
        {
            if (lineItem == null)
            {
                throw new ArgumentNullException(nameof(lineItem));
            }

            var result = lineItem.FulfillmentCenterId;

            if (string.IsNullOrEmpty(result))
            {
                //Try to find a concrete shipment for given line item 
                var shipment = orderShipments?.Where(x => x.Items != null)
                    .FirstOrDefault(s => s.Items.Any(i => i.LineItemId == lineItem.Id));
                if (shipment == null)
                {
                    shipment = orderShipments?.FirstOrDefault();
                }
                result = shipment?.FulfillmentCenterId;
            }

            //Use a default fulfillment center defined for store
            if (string.IsNullOrEmpty(result))
            {
                var store = await _storeService.GetByIdAsync(orderStoreId, StoreResponseGroup.StoreInfo.ToString());
                result = store?.MainFulfillmentCenterId;
            }
            return result;
        }
    }
}
