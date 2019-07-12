using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Data.Handlers;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using Xunit;

namespace VirtoCommerce.OrderModule.Test
{
    [CLSCompliant(false)]
    [Trait("Category", "CI")]
    public class OrderInventoryAdjustmentTests
    {
        private const string TestStoreId = "TestStore";
        private const string TestFulfillmentCenterId = "TestFulfillmentCenter";

        private static readonly CustomerOrder InitialOrder = new CustomerOrder
        {
            StoreId = TestStoreId,
            Items = new List<LineItem>
            {
                new LineItem
                {
                    Id = "{01234567-89ab-cdef-0123-456789abcdef}",
                    ProductId = "shoes",
                    Quantity = 2
                },
                new LineItem
                {
                    Id = "{abcdef01-2345-6789-abcd-ef0123456789}",
                    ProductId = "t-shirt",
                    Quantity = 2
                }
            }
        };

        private static readonly CustomerOrder ChangedOrder = new CustomerOrder
        {
            StoreId = TestStoreId,
            Items = new List<LineItem>
            {
                new LineItem
                {
                    Id = "{01234567-89ab-cdef-0123-456789abcdef}",
                    ProductId = "shoes",
                    Quantity = 5
                },
                new LineItem
                {
                    Id = "{fedcba98-7654-3210-0123-456789abcdef}",
                    ProductId = "t-shirt-new",
                    Quantity = 1
                }
            }
        };

        public static readonly IList<object[]> TestData = new List<object[]>
        {
            new object[]
            {
                new GenericChangedEntry<CustomerOrder>(ChangedOrder, InitialOrder, EntryState.Modified),
                new[]
                {
                    new AdjustInventoryOrderChangedEventHandler.ProductInventoryChange
                    {
                        FulfillmentCenterId = TestFulfillmentCenterId,
                        ProductId = "shoes",
                        QuantityDelta = 3
                    },
                    new AdjustInventoryOrderChangedEventHandler.ProductInventoryChange
                    {
                        FulfillmentCenterId = TestFulfillmentCenterId,
                        ProductId = "t-shirt-new",
                        QuantityDelta = 1
                    },
                    new AdjustInventoryOrderChangedEventHandler.ProductInventoryChange
                    {
                        FulfillmentCenterId = TestFulfillmentCenterId,
                        ProductId = "t-shirt",
                        QuantityDelta = -2
                    }
                }
            },
            new object[]
            {
                new GenericChangedEntry<CustomerOrder>(InitialOrder, InitialOrder, EntryState.Added),
                new[]
                {
                    new AdjustInventoryOrderChangedEventHandler.ProductInventoryChange
                    {
                        FulfillmentCenterId = TestFulfillmentCenterId,
                        ProductId = "shoes",
                        QuantityDelta = 2
                    },
                    new AdjustInventoryOrderChangedEventHandler.ProductInventoryChange
                    {
                        FulfillmentCenterId = TestFulfillmentCenterId,
                        ProductId = "t-shirt",
                        QuantityDelta = 2
                    }
                }
            },
            new object[]
            {
                new GenericChangedEntry<CustomerOrder>(InitialOrder, InitialOrder, EntryState.Deleted),
                new[]
                {
                    new AdjustInventoryOrderChangedEventHandler.ProductInventoryChange
                    {
                        FulfillmentCenterId = TestFulfillmentCenterId,
                        ProductId = "shoes",
                        QuantityDelta = -2
                    },
                    new AdjustInventoryOrderChangedEventHandler.ProductInventoryChange
                    {
                        FulfillmentCenterId = TestFulfillmentCenterId,
                        ProductId = "t-shirt",
                        QuantityDelta = -2
                    }
                }
            }
        };

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task AdjustInventoryHandler_GetProductInventoryChanges_ForOrderChanges(GenericChangedEntry<CustomerOrder> orderChangedEntry,
            IEnumerable<AdjustInventoryOrderChangedEventHandler.ProductInventoryChange> expectedChanges)
        {
            // Arrange
            var inventoryServiceMock = new Mock<IInventoryService>();
            var settingsManagerMock = new Mock<ISettingsManager>();

            var storeServiceMock = new Mock<IStoreService>();
            storeServiceMock.Setup(x => x.GetByIdAsync(TestStoreId, null))
                .ReturnsAsync(new Store { MainFulfillmentCenterId = TestFulfillmentCenterId });

            var targetHandler = new AdjustInventoryOrderChangedEventHandler(inventoryServiceMock.Object,
                storeServiceMock.Object, settingsManagerMock.Object);

            // Act
            var actualChanges = await targetHandler.GetProductInventoryChangesFor(orderChangedEntry);

            // Assert
            var equalityComparer = AnonymousComparer.Create((AdjustInventoryOrderChangedEventHandler.ProductInventoryChange x) => $"{x.FulfillmentCenterId} {x.ProductId} {x.QuantityDelta}");
            Assert.Equal(expectedChanges, actualChanges, equalityComparer);
        }
    }
}
