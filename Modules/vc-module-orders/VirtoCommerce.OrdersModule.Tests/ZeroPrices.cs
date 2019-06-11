using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Data.Model;
using VirtoCommerce.OrdersModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.OrdersModule.Tests
{
    [Trait("Category", "CI")]
    public class ZeroPrices
    {
        public static CustomerOrderEntity GetResetedOrder()
        {
            var orderId = "orderId";

            var order = new CustomerOrderEntity
            {
                Id = orderId,
                CustomerId = "customerId",
                StoreId = "storeId",
                Number = "number",
                Currency = "usd",
                PaymentTotal = 1,
                PaymentTotalWithTax = 1,
                ShippingTotalWithTax = 1,
                DiscountAmount = 1,
                DiscountTotal = 1,
                DiscountTotalWithTax = 1,
                HandlingTotal = 1,
                HandlingTotalWithTax = 1,
                ShippingTotal = 1,
                SubTotal = 1,
                SubTotalWithTax = 1,
                Sum = 1,
                TaxPercentRate = 1,
                TaxTotal = 1,
                Total = 1
            };

            var item = new LineItemEntity
            {
                CustomerOrderId = orderId,
                Id = "itemId",
                Currency = "usd",
                ProductId = "productId",
                CatalogId = "catalogId",
                Sku = "sku",
                Name = "itemName",
                Price = 1,
                DiscountAmount = 1,
                TaxPercentRate = 1,
                TaxTotal = 1,
                DiscountAmountWithTax = 1,
                PriceWithTax = 1,
            };

            var payment = new PaymentInEntity
            {
                CustomerOrderId = orderId,
                Id = "paymentId",
                CustomerId = "customerId",
                Number = "number",
                Currency = "usd",
                Price = 1,
                DiscountAmount = 1,
                DiscountAmountWithTax = 1,
                PriceWithTax = 1,
                TaxPercentRate = 1,
                TaxTotal = 1,
                Total = 1,
                Sum = 1,
                TotalWithTax = 1,
            };

            var shipment = new ShipmentEntity
            {
                CustomerOrderId = orderId,
                Id = "shipmentId",
                Number = "number",
                Currency = "usd",
                Price = 1,
                DiscountAmount = 1,
                DiscountAmountWithTax = 1,
                PriceWithTax = 1,
                Sum = 1,
                TaxPercentRate = 1,
                TaxTotal = 1,
                Total = 1,
                TotalWithTax = 1,
            };

            order.ResetPrices();
            item.ResetPrices();
            payment.ResetPrices();
            shipment.ResetPrices();

            order.Items = new ObservableCollection<LineItemEntity> { item };
            order.InPayments = new ObservableCollection<PaymentInEntity> { payment };
            order.Shipments = new ObservableCollection<ShipmentEntity> { shipment };

            return order;
        }

        [Fact]
        public void CanZeroPrices()
        {
            var order = GetResetedOrder();

            var calc = new DefaultCustomerOrderTotalsCalculator();
            var domainOrder = (CustomerOrder)order.ToModel(AbstractTypeFactory<CustomerOrder>.TryCreateInstance());

            calc.CalculateTotals(domainOrder);

            Assert.Null(domainOrder.DiscountAmount);
            Assert.Null(domainOrder.DiscountTotal);
            Assert.Null(domainOrder.DiscountTotalWithTax);
            Assert.Null(domainOrder.Fee);
            Assert.Null(domainOrder.FeeTotal);
            Assert.Null(domainOrder.FeeTotalWithTax);
            Assert.Null(domainOrder.FeeWithTax);
            Assert.Null(domainOrder.PaymentDiscountTotal);
            Assert.Null(domainOrder.PaymentDiscountTotalWithTax);
            Assert.Null(domainOrder.PaymentSubTotal);
            Assert.Null(domainOrder.PaymentSubTotalWithTax);
            Assert.Null(domainOrder.PaymentTaxTotal);
            Assert.Null(domainOrder.PaymentTotal);
            Assert.Null(domainOrder.PaymentTotalWithTax);
            Assert.Null(domainOrder.ShippingDiscountTotal);
            Assert.Null(domainOrder.ShippingDiscountTotalWithTax);
            Assert.Null(domainOrder.ShippingSubTotal);
            Assert.Null(domainOrder.ShippingSubTotalWithTax);
            Assert.Null(domainOrder.ShippingTotal);
            Assert.Null(domainOrder.ShippingTotalWithTax);
            Assert.Null(domainOrder.SubTotal);
            Assert.Null(domainOrder.SubTotalDiscount);
            Assert.Null(domainOrder.SubTotalDiscountWithTax);
            Assert.Null(domainOrder.SubTotalTaxTotal);
            Assert.Null(domainOrder.SubTotalWithTax);
            Assert.Null(domainOrder.Sum);
            Assert.Null(domainOrder.TaxPercentRate);
            Assert.Null(domainOrder.TaxTotal);

            Assert.Null(domainOrder.InPayments.FirstOrDefault().DiscountAmount);
            Assert.Null(domainOrder.InPayments.FirstOrDefault().DiscountAmountWithTax);
            Assert.Null(domainOrder.InPayments.FirstOrDefault().Price);
            Assert.Null(domainOrder.InPayments.FirstOrDefault().PriceWithTax);
            Assert.Null(domainOrder.InPayments.FirstOrDefault().Sum);
            Assert.Null(domainOrder.InPayments.FirstOrDefault().TaxPercentRate);
            Assert.Null(domainOrder.InPayments.FirstOrDefault().TaxTotal);
            Assert.Null(domainOrder.InPayments.FirstOrDefault().Total);
            Assert.Null(domainOrder.InPayments.FirstOrDefault().TotalWithTax);

            Assert.Null(domainOrder.Shipments.FirstOrDefault().DiscountAmount);
            Assert.Null(domainOrder.Shipments.FirstOrDefault().DiscountAmountWithTax);
            Assert.Null(domainOrder.Shipments.FirstOrDefault().Fee);
            Assert.Null(domainOrder.Shipments.FirstOrDefault().FeeWithTax);
            Assert.Null(domainOrder.Shipments.FirstOrDefault().Price);
            Assert.Null(domainOrder.Shipments.FirstOrDefault().PriceWithTax);
            Assert.Null(domainOrder.Shipments.FirstOrDefault().Sum);
            Assert.Null(domainOrder.Shipments.FirstOrDefault().TaxPercentRate);
            Assert.Null(domainOrder.Shipments.FirstOrDefault().TaxTotal);
            Assert.Null(domainOrder.Shipments.FirstOrDefault().Total);
            Assert.Null(domainOrder.Shipments.FirstOrDefault().TotalWithTax);

            Assert.Null(domainOrder.Items.FirstOrDefault().DiscountAmount);
            Assert.Null(domainOrder.Items.FirstOrDefault().DiscountAmountWithTax);
            Assert.Null(domainOrder.Items.FirstOrDefault().DiscountTotal);
            Assert.Null(domainOrder.Items.FirstOrDefault().DiscountTotalWithTax);
            Assert.Null(domainOrder.Items.FirstOrDefault().ExtendedPrice);
            Assert.Null(domainOrder.Items.FirstOrDefault().ExtendedPriceWithTax);
            Assert.Null(domainOrder.Items.FirstOrDefault().Fee);
            Assert.Null(domainOrder.Items.FirstOrDefault().FeeWithTax);
            Assert.Null(domainOrder.Items.FirstOrDefault().PlacedPrice);
            Assert.Null(domainOrder.Items.FirstOrDefault().PlacedPriceWithTax);
            Assert.Null(domainOrder.Items.FirstOrDefault().Price);
            Assert.Null(domainOrder.Items.FirstOrDefault().PriceWithTax);
            Assert.Null(domainOrder.Items.FirstOrDefault().TaxPercentRate);
            Assert.Null(domainOrder.Items.FirstOrDefault().TaxTotal);
        }

        [Fact]
        public void CanZeroPricesWithoutCalc()
        {
            var order = GetResetedOrder();

            Assert.Null(order.TaxPercentRate);
            Assert.Null(order.ShippingTotalWithTax);
            Assert.Null(order.PaymentTotalWithTax);
            Assert.Null(order.DiscountAmount);
            Assert.Null(order.Total);
            Assert.Null(order.SubTotal);
            Assert.Null(order.SubTotalWithTax);
            Assert.Null(order.ShippingTotal);
            Assert.Null(order.PaymentTotal);
            Assert.Null(order.HandlingTotal);
            Assert.Null(order.HandlingTotalWithTax);
            Assert.Null(order.DiscountTotal);
            Assert.Null(order.DiscountTotalWithTax);
            Assert.Null(order.TaxTotal);
            Assert.Null(order.Sum);

            Assert.Null(order.InPayments.FirstOrDefault().Price);
            Assert.Null(order.InPayments.FirstOrDefault().PriceWithTax);
            Assert.Null(order.InPayments.FirstOrDefault().DiscountAmount);
            Assert.Null(order.InPayments.FirstOrDefault().DiscountAmountWithTax);
            Assert.Null(order.InPayments.FirstOrDefault().Total);
            Assert.Null(order.InPayments.FirstOrDefault().TotalWithTax);
            Assert.Null(order.InPayments.FirstOrDefault().TaxTotal);
            Assert.Null(order.InPayments.FirstOrDefault().TaxPercentRate);
            Assert.Null(order.InPayments.FirstOrDefault().Sum);

            Assert.Null(order.Shipments.FirstOrDefault().Price);
            Assert.Null(order.Shipments.FirstOrDefault().PriceWithTax);
            Assert.Null(order.Shipments.FirstOrDefault().DiscountAmount);
            Assert.Null(order.Shipments.FirstOrDefault().DiscountAmountWithTax);
            Assert.Null(order.Shipments.FirstOrDefault().Total);
            Assert.Null(order.Shipments.FirstOrDefault().TotalWithTax);
            Assert.Null(order.Shipments.FirstOrDefault().TaxTotal);
            Assert.Null(order.Shipments.FirstOrDefault().TaxPercentRate);
            Assert.Null(order.Shipments.FirstOrDefault().Sum);

            Assert.Null(order.Items.FirstOrDefault().Price);
            Assert.Null(order.Items.FirstOrDefault().PriceWithTax);
            Assert.Null(order.Items.FirstOrDefault().DiscountAmount);
            Assert.Null(order.Items.FirstOrDefault().DiscountAmountWithTax);
            Assert.Null(order.Items.FirstOrDefault().TaxTotal);
            Assert.Null(order.Items.FirstOrDefault().TaxPercentRate);
        }
    }
}
