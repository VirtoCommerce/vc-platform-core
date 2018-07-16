using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Payment;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.OrdersModule.Data.Repositories;
using VirtoCommerce.OrdersModule.Data.Services;
using VirtoCommerce.OrdersModule.Web.Controllers.Api;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.StoreModule.Core.Services;
using Xunit;
using Address = VirtoCommerce.OrdersModule.Core.Model.Address;
using DesignTimeDbContextFactory = VirtoCommerce.OrdersModule.Data.Repositories.DesignTimeDbContextFactory;

namespace VirtoCommerce.OrdersModule.Tests
{
    // [Trait("Category", "CI")]
    public class CRUDScenarios// : FunctionalTestBase
    {
        private readonly ICustomerOrderService _customerOrderService;

        public CRUDScenarios()
        {
            _customerOrderService = new CustomerOrderServiceImpl(GetOrderRepositoryFactory(), null, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task SaveChangesAsync_CreateNewOrder()
        {
            //Arrange
            var order = GetTestOrder("order");

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
                Id = "shoes",
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
                Id = "t-shirt",
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

        private static Func<IOrderRepository> GetOrderRepositoryFactory()
        {
            Func<IOrderRepository> orderRepositoryFactory = () =>
            {
                string[] args = null;
                var factory = new DesignTimeDbContextFactory();
                return new OrderRepositoryImpl(factory.CreateDbContext(args));
            };
            return orderRepositoryFactory;
        }

    }
}
