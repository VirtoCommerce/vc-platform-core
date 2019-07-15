using System.Collections.Generic;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Data.Services;
using Xunit;

namespace VirtoCommerce.OrdersModule.Tests
{
    [Trait("Category", "CI")]
    public class OrderTotalsCalculationTest
    {
        [Fact]
        public void CalculateTotals_ShouldBe_RightTotals()
        {
            var item1 = new LineItem { Price = 10.99m, DiscountAmount = 1.33m, TaxPercentRate = 0.12m, Fee = 0.33m, Quantity = 2 };
            var item2 = new LineItem { Price = 55.22m, DiscountAmount = 5.89m, TaxPercentRate = 0.12m, Fee = 0.12m, Quantity = 5 };
            var item3 = new LineItem { Price = 88.45m, DiscountAmount = 10.78m, TaxPercentRate = 0.12m, Fee = 0.08m, Quantity = 12 };
            var payment = new PaymentIn { Price = 44.52m, DiscountAmount = 10, TaxPercentRate = 0.12m };
            var shipment = new Shipment { Price = 22.0m, DiscountAmount = 5m, TaxPercentRate = 0.12m };

            var order = new CustomerOrder
            {
                TaxPercentRate = 0.12m,
                Fee = 13.11m,
                Items = new List<LineItem> { item1, item2, item3 },
                InPayments = new List<PaymentIn> { payment },
                Shipments = new List<Shipment> { shipment }
            };
            var totalsCalculator = new DefaultCustomerOrderTotalsCalculator();
            totalsCalculator.CalculateTotals(order);

            Assert.Equal(12.3088m, item1.PriceWithTax);
            Assert.Equal(9.66m, item1.PlacedPrice);
            Assert.Equal(19.32m, item1.ExtendedPrice);
            Assert.Equal(1.4896m, item1.DiscountAmountWithTax);
            Assert.Equal(2.66m, item1.DiscountTotal);
            Assert.Equal(0.3696m, item1.FeeWithTax);
            Assert.Equal(10.8192m, item1.PlacedPriceWithTax);
            Assert.Equal(21.6384m, item1.ExtendedPriceWithTax);
            Assert.Equal(2.9792m, item1.DiscountTotalWithTax);
            Assert.Equal(2.358m, item1.TaxTotal);

            Assert.Equal(5.6m, shipment.DiscountAmountWithTax);
            Assert.Equal(24.64m, shipment.PriceWithTax);
            Assert.Equal(0.0m, shipment.FeeWithTax);
            Assert.Equal(17.0m, shipment.Total);
            Assert.Equal(19.04m, shipment.TotalWithTax);
            Assert.Equal(2.04m, shipment.TaxTotal);

            Assert.Equal(34.52m, payment.Total);
            Assert.Equal(49.8624m, payment.PriceWithTax);
            Assert.Equal(38.6624m, payment.TotalWithTax);
            Assert.Equal(11.2m, payment.DiscountAmountWithTax);
            Assert.Equal(4.1424m, payment.TaxTotal);

            Assert.Equal(1359.48m, order.SubTotal);
            Assert.Equal(161.47m, order.SubTotalDiscount);
            Assert.Equal(180.85m, order.SubTotalDiscountWithTax);
            Assert.Equal(1522.62m, order.SubTotalWithTax);
            Assert.Equal(22.00m, order.ShippingSubTotal);
            Assert.Equal(24.64m, order.ShippingSubTotalWithTax);
            Assert.Equal(44.52m, order.PaymentSubTotal);
            Assert.Equal(49.86m, order.PaymentSubTotalWithTax);
            Assert.Equal(150.01m, order.TaxTotal);
            Assert.Equal(176.47m, order.DiscountTotal);
            Assert.Equal(197.65m, order.DiscountTotalWithTax);
            Assert.Equal(13.64m, order.FeeTotal);
            Assert.Equal(15.28m, order.FeeTotalWithTax);
            Assert.Equal(14.68m, order.FeeWithTax);
            Assert.Equal(1413.18m, order.Total);
        }
    }
}

