using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Payment;
using VirtoCommerce.CoreModule.Core.Shipping;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.OrdersModule.Data.Repositories;
using VirtoCommerce.OrdersModule.Data.Services;
using VirtoCommerce.OrdersModule.Web.Controllers.Api;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.StoreModule.Core.Services;
using Xunit;
using Address = VirtoCommerce.OrdersModule.Core.Model.Address;
using DesignTimeDbContextFactory = VirtoCommerce.OrdersModule.Data.Repositories.DesignTimeDbContextFactory;

namespace VirtoCommerce.OrdersModule.Tests
{
    // [Trait("Category", "CI")]
    public class CustomerOrderServiceImplIntegrationTests// : FunctionalTestBase
    {
        private readonly Mock<IStoreService> _storeServiceMock;
        private readonly Mock<IShippingMethodsRegistrar> _shippingMethodRegistrarMock;
        private readonly Mock<IPaymentMethodsRegistrar> _paymentMethodRegistrarMock;
        private readonly Mock<ICustomerOrderTotalsCalculator> _customerOrderTotalsCalculatorMock;
        private readonly Mock<IUniqueNumberGenerator> _uniqueNumberGeneratorMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<IDynamicPropertyService> _dynamicPropertyServiceMock;
        private readonly Mock<IPlatformMemoryCache> _platformMemoryCacheMock;
        private readonly Mock<IChangeLogService> _changeLogServiceMock;
        private readonly Mock<ICacheEntry> _cacheEntryMock;
        private static IUnitOfWork _unitOfWorkMock;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ICustomerOrderSearchService _customerOrderSearchService;
        
        public CustomerOrderServiceImplIntegrationTests()
        {
            _storeServiceMock = new Mock<IStoreService>();
            _shippingMethodRegistrarMock = new Mock<IShippingMethodsRegistrar>();
            _paymentMethodRegistrarMock = new Mock<IPaymentMethodsRegistrar>();
            _uniqueNumberGeneratorMock = new Mock<IUniqueNumberGenerator>();
            _customerOrderTotalsCalculatorMock = new Mock<ICustomerOrderTotalsCalculator>();
            //_eventPublisherMock = new Mock<IEventPublisher>();
            _dynamicPropertyServiceMock = new Mock<IDynamicPropertyService>();
            _platformMemoryCacheMock = new Mock<IPlatformMemoryCache>();
            _cacheEntryMock = new Mock<ICacheEntry>();
            _changeLogServiceMock = new Mock<IChangeLogService>();

            var container = new ServiceCollection();
            container.AddDbContext<OrderDbContext>(options => options.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30"));
            container.AddScoped<IOrderRepository, OrderRepositoryImpl>();
            container.AddScoped<ICustomerOrderService, CustomerOrderServiceImpl>();
            container.AddScoped<ICustomerOrderSearchService, CustomerOrderSearchServiceImpl>();
            container.AddScoped<Func<IOrderRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IOrderRepository>());
            container.AddScoped<IEventPublisher, InProcessBus>();
            container.AddSingleton(tc => _customerOrderTotalsCalculatorMock.Object);
            container.AddSingleton(x => _uniqueNumberGeneratorMock.Object);
            container.AddSingleton(x => _dynamicPropertyServiceMock.Object);
            container.AddSingleton(x => _storeServiceMock.Object);
            container.AddSingleton(x => _shippingMethodRegistrarMock.Object);
            container.AddSingleton(x => _paymentMethodRegistrarMock.Object);
            container.AddSingleton(x => _platformMemoryCacheMock.Object);
            container.AddSingleton(x => _changeLogServiceMock.Object);

            var serviceProvider = container.BuildServiceProvider();
            _customerOrderService = serviceProvider.GetService<ICustomerOrderService>();
            _customerOrderSearchService = serviceProvider.GetService<ICustomerOrderSearchService>();
        }

        [Fact]
        public async Task SaveChangesAsync_CreateNewOrder()
        {
            //Arrange
            var order = GetTestOrder($"order{DateTime.Now:O}");
            var cacheKey = CacheKey.With(_customerOrderService.GetType(), "GetByIdsAsync", string.Join("-", order.Id), null);
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_cacheEntryMock.Object);

            //Act
            await _customerOrderService.SaveChangesAsync(new[] { order });
            order = await _customerOrderService.GetByIdAsync(order.Id);

            //Assert
            Assert.Equal(PaymentStatus.Pending, order.InPayments.First().PaymentStatus);
            Assert.Null(order.InPayments.First().Status);
            Assert.NotNull(order);
            Assert.Equal(PaymentStatus.Pending, order.InPayments.First().PaymentStatus);
            Assert.NotNull(order.InPayments.First().Status);
        }

        [Fact]
        public async Task SaveChangesAsync_UpdateOrder()
        {
            //Arrange
            var criteria = new CustomerOrderSearchCriteria() { Take = 1 };
            var cacheKeySearch = CacheKey.With(_customerOrderSearchService.GetType(), "SearchCustomerOrdersAsync", criteria.GetCacheKey()); 
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKeySearch)).Returns(_cacheEntryMock.Object);
            var orders = await _customerOrderSearchService.SearchCustomerOrdersAsync(criteria);
            var order = orders.Results.FirstOrDefault();
            var cacheKey = CacheKey.With(_customerOrderService.GetType(), "GetByIdsAsync", string.Join("-", order.Id), null);
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_cacheEntryMock.Object);
            order.Status = "Pending";

            //Act
            await _customerOrderService.SaveChangesAsync(new[] { order });
            

            //Assert
            Assert.NotNull(order);
            Assert.Equal("Pending", order.Status);
        }

        //[Fact]
        //public void Can_update_order_status()
        //{
        //    //arrange
        //    var order = GetTestOrder("order");
        //    var _customerOrderService = GetCustomerOrderService();

        //    //act
        //    order = _customerOrderService.GetByIds(new[] { "order" }).FirstOrDefault();

        //    _customerOrderService.SaveChanges(new[] { order });

        //    ////assert
        //    //Assert.Equal("", GetErrors(payResponse.error));
        //}

        //protected CommerceRepositoryImpl GetRepository()
        //{
        //    var repository = new CommerceRepositoryImpl(ConnectionString, new EntityPrimaryKeyGeneratorInterceptor(), new AuditableInterceptor(null));
        //    EnsureDatabaseInitialized(() => new CommerceRepositoryImpl(ConnectionString), () => Database.SetInitializer(new SetupDatabaseInitializer<CommerceRepositoryImpl, Configuration>()));
        //    return repository;
        //}

        //public override void Dispose()
        //{
        //    // Ensure LocalDb databases are deleted after use so that LocalDb doesn't throw if
        //    // the temp location in which they are stored is later cleaned.
        //    using (var context = new CommerceRepositoryImpl(ConnectionString))
        //    {
        //        context.Database.Delete();
        //    }
        //}

        private static CustomerOrder GetTestOrder(string id)
        {
            var order = new CustomerOrder
            {
                Id = id,
                Number = "CO" + id,
                Currency = "USD",
                CustomerId = "vasja customer",
                EmployeeId = "employe",
                StoreId = "test store",
                Addresses = new[]
                {
                            new Address {
                            AddressType = AddressType.Shipping,
                            City = "london",
                            Phone = "+68787687",
                            PostalCode = "22222",
                            CountryCode = "ENG",
                            CountryName = "England",
                            Email = "user@mail.com",
                            FirstName = "first name",
                            LastName = "last name",
                            Line1 = "line 1",
                            Organization = "org1"
                            }
                        }.ToList(),
                Discounts = new[] {
                    new Discount
                {
                    PromotionId = "testPromotion",
                    Currency = "USD",
                    DiscountAmount = 12,
                    Coupon = "ssss"
                    }
                }
            };
            var item1 = new LineItem
            {
                Id = Guid.NewGuid().ToString(),
                Sku = "shoes",
                Price = 10,
                ProductId = "shoes",
                CatalogId = "catalog",
                Currency = "USD",
                CategoryId = "category",
                Name = "shoes",
                Quantity = 2,
                FulfillmentLocationCode = "warehouse1",
                ShippingMethodCode = "EMS",
                Discounts = new[] {  new Discount
                {
                    PromotionId = "itemPromotion",
                    Currency = "USD",
                    DiscountAmount = 12,
                    Coupon =  "ssss"
                }}
            };
            var item2 = new LineItem
            {
                Id = Guid.NewGuid().ToString(),
                Sku = "t-shirt",
                Price = 100,
                ProductId = "t-shirt",
                CatalogId = "catalog",
                CategoryId = "category",
                Currency = "USD",
                Name = "t-shirt",
                Quantity = 2,
                FulfillmentLocationCode = "warehouse1",
                ShippingMethodCode = "EMS"
            };
            order.Items = new List<LineItem>();
            order.Items.Add(item1);
            order.Items.Add(item2);

            var shipment = new Shipment
            {
                Number = "SH" + id,
                Currency = "USD",
                DeliveryAddress = new Address
                {
                    City = "london",
                    CountryName = "England",
                    Phone = "+68787687",
                    PostalCode = "2222",
                    CountryCode = "ENG",
                    Email = "user@mail.com",
                    FirstName = "first name",
                    LastName = "last name",
                    Line1 = "line 1",
                    Organization = "org1"
                },
                Discounts = new[] {  new Discount
                {
                    PromotionId = "testPromotion",
                    Currency = "USD",
                    DiscountAmount = 12,
                    Coupon = ""
                }},

            };

            shipment.Items = new List<ShipmentItem>();
            shipment.Items.AddRange(order.Items.Select(x => new ShipmentItem(x)));

            order.Shipments = new List<Shipment>();
            order.Shipments.Add(shipment);

            var payment = new PaymentIn
            {
                Number = "PI" + id,
                PaymentStatus = PaymentStatus.Pending,
                Currency = "USD",
                Sum = 10,
                CustomerId = "et"
            };
            order.InPayments = new List<PaymentIn>();
            order.InPayments.Add(payment);

            return order;
        }
    }
}
