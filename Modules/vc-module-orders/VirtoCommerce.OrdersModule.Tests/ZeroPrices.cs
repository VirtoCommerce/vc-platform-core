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

            Assert.Equal(0, domainOrder.DiscountAmount);
            Assert.Equal(0, domainOrder.DiscountTotal);
            Assert.Equal(0, domainOrder.DiscountTotalWithTax);
            Assert.Equal(0, domainOrder.Fee);
            Assert.Equal(0, domainOrder.FeeTotal);
            Assert.Equal(0, domainOrder.FeeTotalWithTax);
            Assert.Equal(0, domainOrder.FeeWithTax);
            Assert.Equal(0, domainOrder.PaymentDiscountTotal);
            Assert.Equal(0, domainOrder.PaymentDiscountTotalWithTax);
            Assert.Equal(0, domainOrder.PaymentSubTotal);
            Assert.Equal(0, domainOrder.PaymentSubTotalWithTax);
            Assert.Equal(0, domainOrder.PaymentTaxTotal);
            Assert.Equal(0, domainOrder.PaymentTotal);
            Assert.Equal(0, domainOrder.PaymentTotalWithTax);
            Assert.Equal(0, domainOrder.ShippingDiscountTotal);
            Assert.Equal(0, domainOrder.ShippingDiscountTotalWithTax);
            Assert.Equal(0, domainOrder.ShippingSubTotal);
            Assert.Equal(0, domainOrder.ShippingSubTotalWithTax);
            Assert.Equal(0, domainOrder.ShippingTotal);
            Assert.Equal(0, domainOrder.ShippingTotalWithTax);
            Assert.Equal(0, domainOrder.SubTotal);
            Assert.Equal(0, domainOrder.SubTotalDiscount);
            Assert.Equal(0, domainOrder.SubTotalDiscountWithTax);
            Assert.Equal(0, domainOrder.SubTotalTaxTotal);
            Assert.Equal(0, domainOrder.SubTotalWithTax);
            Assert.Equal(0, domainOrder.Sum);
            Assert.Equal(0, domainOrder.TaxPercentRate);
            Assert.Equal(0, domainOrder.TaxTotal);

            Assert.Equal(0, domainOrder.InPayments.FirstOrDefault().DiscountAmount);
            Assert.Equal(0, domainOrder.InPayments.FirstOrDefault().DiscountAmountWithTax);
            Assert.Equal(0, domainOrder.InPayments.FirstOrDefault().Price);
            Assert.Equal(0, domainOrder.InPayments.FirstOrDefault().PriceWithTax);
            Assert.Equal(0, domainOrder.InPayments.FirstOrDefault().Sum);
            Assert.Equal(0, domainOrder.InPayments.FirstOrDefault().TaxPercentRate);
            Assert.Equal(0, domainOrder.InPayments.FirstOrDefault().TaxTotal);
            Assert.Equal(0, domainOrder.InPayments.FirstOrDefault().Total);
            Assert.Equal(0, domainOrder.InPayments.FirstOrDefault().TotalWithTax);

            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().DiscountAmount);
            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().DiscountAmountWithTax);
            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().Fee);
            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().FeeWithTax);
            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().Price);
            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().PriceWithTax);
            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().Sum);
            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().TaxPercentRate);
            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().TaxTotal);
            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().Total);
            Assert.Equal(0, domainOrder.Shipments.FirstOrDefault().TotalWithTax);

            Assert.Equal(0, domainOrder.Items.FirstOrDefault().DiscountAmount);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().DiscountAmountWithTax);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().DiscountTotal);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().DiscountTotalWithTax);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().ExtendedPrice);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().ExtendedPriceWithTax);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().Fee);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().FeeWithTax);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().PlacedPrice);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().PlacedPriceWithTax);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().Price);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().PriceWithTax);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().TaxPercentRate);
            Assert.Equal(0, domainOrder.Items.FirstOrDefault().TaxTotal);
        }

        [Fact]
        public void CanZeroPricesWithoutCalc()
        {
            var order = GetResetedOrder();

            Assert.Equal(0, order.TaxPercentRate);
            Assert.Equal(0, order.ShippingTotalWithTax);
            Assert.Equal(0, order.PaymentTotalWithTax);
            Assert.Equal(0, order.DiscountAmount);
            Assert.Equal(0, order.Total);
            Assert.Equal(0, order.SubTotal);
            Assert.Equal(0, order.SubTotalWithTax);
            Assert.Equal(0, order.ShippingTotal);
            Assert.Equal(0, order.PaymentTotal);
            Assert.Equal(0, order.HandlingTotal);
            Assert.Equal(0, order.HandlingTotalWithTax);
            Assert.Equal(0, order.DiscountTotal);
            Assert.Equal(0, order.DiscountTotalWithTax);
            Assert.Equal(0, order.TaxTotal);
            Assert.Equal(0, order.Sum);

            Assert.Equal(0, order.InPayments.FirstOrDefault().Price);
            Assert.Equal(0, order.InPayments.FirstOrDefault().PriceWithTax);
            Assert.Equal(0, order.InPayments.FirstOrDefault().DiscountAmount);
            Assert.Equal(0, order.InPayments.FirstOrDefault().DiscountAmountWithTax);
            Assert.Equal(0, order.InPayments.FirstOrDefault().Total);
            Assert.Equal(0, order.InPayments.FirstOrDefault().TotalWithTax);
            Assert.Equal(0, order.InPayments.FirstOrDefault().TaxTotal);
            Assert.Equal(0, order.InPayments.FirstOrDefault().TaxPercentRate);
            Assert.Equal(0, order.InPayments.FirstOrDefault().Sum);

            Assert.Equal(0, order.Shipments.FirstOrDefault().Price);
            Assert.Equal(0, order.Shipments.FirstOrDefault().PriceWithTax);
            Assert.Equal(0, order.Shipments.FirstOrDefault().DiscountAmount);
            Assert.Equal(0, order.Shipments.FirstOrDefault().DiscountAmountWithTax);
            Assert.Equal(0, order.Shipments.FirstOrDefault().Total);
            Assert.Equal(0, order.Shipments.FirstOrDefault().TotalWithTax);
            Assert.Equal(0, order.Shipments.FirstOrDefault().TaxTotal);
            Assert.Equal(0, order.Shipments.FirstOrDefault().TaxPercentRate);
            Assert.Equal(0, order.Shipments.FirstOrDefault().Sum);

            Assert.Equal(0, order.Items.FirstOrDefault().Price);
            Assert.Equal(0, order.Items.FirstOrDefault().PriceWithTax);
            Assert.Equal(0, order.Items.FirstOrDefault().DiscountAmount);
            Assert.Equal(0, order.Items.FirstOrDefault().DiscountAmountWithTax);
            Assert.Equal(0, order.Items.FirstOrDefault().TaxTotal);
            Assert.Equal(0, order.Items.FirstOrDefault().TaxPercentRate);
        }
    }
}
