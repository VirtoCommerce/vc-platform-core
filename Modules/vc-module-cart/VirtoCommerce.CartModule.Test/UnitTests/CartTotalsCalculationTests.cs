using System.Collections.Generic;
using System.Collections.ObjectModel;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Services;
using Xunit;

namespace VirtoCommerce.CartModule.Test.UnitTests
{
    [Trait("Category", "CI")]
    public class OrderTotalsCalculationTest
    {

        [Fact]
        public void CalculateTotals_CartTotals_MustBe_Sum_Of_Parts_After_Round()
        {
            var item1 = new LineItem { ListPrice = 49.95m, SalePrice = 49.95m, DiscountAmount = 4.995m, TaxPercentRate = 0m, Fee = 0m, Quantity = 1 };

            var cart = new ShoppingCart
            {
                Items = new List<LineItem> { item1 },
            };
            var totalsCalculator = new DefaultShoppingCartTotalsCalculator();
            totalsCalculator.CalculateTotals(cart);

            Assert.Equal(49.95m, cart.SubTotal);
            Assert.Equal(5m, cart.DiscountTotal);
            Assert.Equal(44.95m, cart.Total);
        }


        [Fact]
        public void CalculateTotals_Should_Be_RightTotals()
        {
            var item1 = new LineItem { ListPrice = 10.99m, SalePrice = 9.66m, DiscountAmount = 1.33m, TaxPercentRate = 0.12m, Fee = 0.33m, Quantity = 2 };
            var item2 = new LineItem { ListPrice = 55.22m, SalePrice = 49.33m, DiscountAmount = 5.89m, TaxPercentRate = 0.12m, Fee = 0.12m, Quantity = 5 };
            var item3 = new LineItem { ListPrice = 88.45m, SalePrice = 77.67m, DiscountAmount = 10.78m, TaxPercentRate = 0.12m, Fee = 0.08m, Quantity = 12 };
            var payment = new Payment { Price = 44.52m, DiscountAmount = 10, TaxPercentRate = 0.12m };
            var shipment = new Shipment { Price = 22.0m, DiscountAmount = 5m, TaxPercentRate = 0.12m };

            var cart = new ShoppingCart
            {
                TaxPercentRate = 0.12m,
                Fee = 13.11m,
                Items = new List<LineItem> { item1, item2, item3 },
                Payments = new List<Payment> { payment },
                Shipments = new List<Shipment> { shipment }
            };
            var totalsCalculator = new DefaultShoppingCartTotalsCalculator();
            totalsCalculator.CalculateTotals(cart);

            Assert.Equal(12.3088m, item1.ListPriceWithTax);
            Assert.Equal(10.8192m, item1.SalePriceWithTax);
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

            Assert.Equal(1359.48m, cart.SubTotal);
            Assert.Equal(161.47m, cart.SubTotalDiscount);
            Assert.Equal(180.85m, cart.SubTotalDiscountWithTax);
            Assert.Equal(1522.62m, cart.SubTotalWithTax);
            Assert.Equal(17.00m, cart.ShippingTotal);
            Assert.Equal(19.04m, cart.ShippingTotalWithTax);
            Assert.Equal(22.00m, cart.ShippingSubTotal);
            Assert.Equal(24.64m, cart.ShippingSubTotalWithTax);
            Assert.Equal(44.52m, cart.PaymentSubTotal);
            Assert.Equal(49.86m, cart.PaymentSubTotalWithTax);
            Assert.Equal(150.01m, cart.TaxTotal);
            Assert.Equal(176.47m, cart.DiscountTotal);
            Assert.Equal(197.65m, cart.DiscountTotalWithTax);
            Assert.Equal(13.64m, cart.FeeTotal);
            Assert.Equal(15.28m, cart.FeeTotalWithTax);
            Assert.Equal(14.68m, cart.FeeWithTax);
            Assert.Equal(1413.18m, cart.Total);
        }

        [Fact]
        public void CouponsPatch()
        {
            var originalEntity = new ShoppingCartEntity
            {
                Coupons = new ObservableCollection<CouponEntity>
                {
                    new CouponEntity { Code = "12345", Id = "aa"},
                    new CouponEntity { Code = "abcde", Id = "ab" },
                    new CouponEntity { Code = "AA-BB-CC", Id = "ac" },
                    new CouponEntity { Code = "00-11-22", Id = "ad" },
                    new CouponEntity { Code = "ABCDE", Id = "ae" },
                }
            };

            var modifiedEntity = new ShoppingCartEntity
            {
                Coupons = new ObservableCollection<CouponEntity>
                {
                    new CouponEntity { Code = "abcde", Id = "ba" },
                    new CouponEntity { Code = "AA-BB-CC", Id = "bb" },
                    new CouponEntity { Code = "00-11-22", Id = "bc" },
                    new CouponEntity { Code = "ABCDE", Id = "bd" },
                    new CouponEntity { Code = "FGHIJ", Id = "be" },
                    new CouponEntity { Code = "KLMNO", Id = "bf" },
                }
            };

            modifiedEntity.Patch(originalEntity);

            Assert.Equal(6, originalEntity.Coupons.Count);
            Assert.Equal("abcde", originalEntity.Coupons[0].Code);
            Assert.Equal("ab", originalEntity.Coupons[0].Id);
            Assert.Equal("AA-BB-CC", originalEntity.Coupons[1].Code);
            Assert.Equal("ac", originalEntity.Coupons[1].Id);
            Assert.Equal("00-11-22", originalEntity.Coupons[2].Code);
            Assert.Equal("ad", originalEntity.Coupons[2].Id);
            Assert.Equal("ABCDE", originalEntity.Coupons[3].Code);
            Assert.Equal("ae", originalEntity.Coupons[3].Id);
            Assert.Equal("FGHIJ", originalEntity.Coupons[4].Code);
            Assert.Equal("be", originalEntity.Coupons[4].Id);
            Assert.Equal("KLMNO", originalEntity.Coupons[5].Code);
            Assert.Equal("bf", originalEntity.Coupons[5].Id);
        }
    }
}

