using System;
using System.Linq;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Data.Services
{
    /// <summary>
    /// Respond for totals values calculation for Customer order and all nested objects
    /// </summary>
    public class DefaultCustomerOrderTotalsCalculator : ICustomerOrderTotalsCalculator
    {
        /// <summary>
        /// Order subtotal discount
        /// When a discount is applied to the cart subtotal, the tax calculation has already been applied, and is reflected in the tax subtotal.
        /// Therefore, a discount applying to the cart subtotal will occur after tax.
        /// For instance, if the cart subtotal is $100, and $15 is the tax subtotal, a cart - wide discount of 10 % will yield a total of $105($100 subtotal – $10 discount + $15 tax on the original $100).
        /// </summary>
        public virtual void CalculateTotals(CustomerOrder order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            //Calculate totals for line items
            if (!order.Items.IsNullOrEmpty())
            {
                foreach (var item in order.Items)
                {
                    CalculateLineItemTotals(item);
                }
            }
            //Calculate totals for shipments
            if (!order.Shipments.IsNullOrEmpty())
            {
                foreach (var shipment in order.Shipments)
                {
                    CalculateShipmentTotals(shipment);
                }
            }
            //Calculate totals for payments
            if (!order.InPayments.IsNullOrEmpty())
            {
                foreach (var payment in order.InPayments)
                {
                    CalculatePaymentTotals(payment);
                }
            }

            order.FeeTotal = order.Fee;

            if (!order.Items.IsNullOrEmpty())
            {
                var price = order.Items.Any(x => x.Price.HasValue) ? order.Items.Sum(x => x.Price * x.Quantity) : null;
                order.SubTotal = price != null ? Convert.ToDecimal(order.SubTotal) + price : order.SubTotal;

                order.SubTotalWithTax = order.Items.Any(x => x.PriceWithTax.HasValue) ? order.Items.Sum(x => x.PriceWithTax * x.Quantity) : null;

                var taxTotal = order.Items.Any(x => x.TaxTotal.HasValue) ? order.Items.Sum(x => x.TaxTotal) : null;
                order.SubTotalTaxTotal = taxTotal != null ? Convert.ToDecimal(order.SubTotalTaxTotal) + taxTotal : order.SubTotalTaxTotal;
                order.TaxTotal = taxTotal != null ? Convert.ToDecimal(order.TaxTotal) + taxTotal : order.TaxTotal;

                var discountTotal = order.Items.Any(x => x.DiscountTotal.HasValue) ? order.Items.Sum(x => x.DiscountTotal) : null;
                order.SubTotalDiscount = discountTotal;
                order.DiscountTotal = discountTotal != null ? Convert.ToDecimal(order.DiscountTotal) + discountTotal : order.DiscountTotal;

                var discountTotalWithTax = order.Items.Any(x => x.DiscountTotalWithTax.HasValue) ? order.Items.Sum(x => x.DiscountTotalWithTax) : null;
                order.SubTotalDiscountWithTax = discountTotalWithTax;
                order.DiscountTotalWithTax = discountTotalWithTax != null ? Convert.ToDecimal(order.DiscountTotalWithTax) + discountTotalWithTax : order.DiscountTotalWithTax;

                var fee = order.Items.Any(x => x.Fee.HasValue) ? order.Items.Sum(x => x.Fee) : null;
                order.FeeTotal = fee != null ? Convert.ToDecimal(order.FeeTotal) + fee : order.FeeTotal;
                var feeWithTax = order.Items.Any(x => x.FeeWithTax.HasValue) ? order.Items.Sum(x => x.FeeWithTax) : null;
                order.FeeTotalWithTax = feeWithTax != null ? Convert.ToDecimal(order.FeeTotalWithTax) + feeWithTax : order.FeeTotalWithTax;

            }

            if (!order.Shipments.IsNullOrEmpty())
            {
                order.ShippingTotal = order.Shipments.Any(x => x.Total.HasValue) ? order.Shipments.Sum(x => x.Total) : null;
                order.ShippingTotalWithTax = order.Shipments.Any(x => x.TotalWithTax.HasValue) ? order.Shipments.Sum(x => x.TotalWithTax) : null;
                order.ShippingSubTotal = order.Shipments.Any(x => x.Price.HasValue) ? order.Shipments.Sum(x => x.Price) : null;
                order.ShippingSubTotalWithTax = order.Shipments.Any(x => x.PriceWithTax.HasValue) ? order.Shipments.Sum(x => x.PriceWithTax) : null;
                order.ShippingDiscountTotal = order.Shipments.Any(x => x.DiscountAmount.HasValue) ? order.Shipments.Sum(x => x.DiscountAmount) : null;
                order.ShippingDiscountTotalWithTax = order.Shipments.Any(x => x.DiscountAmountWithTax.HasValue) ? order.Shipments.Sum(x => x.DiscountAmountWithTax) : null;
                order.DiscountTotal += order.Shipments.Sum(x => x.DiscountAmount);
                order.DiscountTotalWithTax += order.Shipments.Sum(x => x.DiscountAmountWithTax);
                order.FeeTotal += order.Shipments.Sum(x => x.Fee);
                order.FeeTotalWithTax += order.Shipments.Sum(x => x.FeeWithTax);
                order.TaxTotal += order.Shipments.Sum(x => x.TaxTotal);
            }

            if (!order.InPayments.IsNullOrEmpty())
            {
                order.PaymentTotal = order.InPayments.Any(x => x.Total.HasValue) ? order.InPayments.Sum(x => x.Total) : null;
                order.PaymentTotalWithTax = order.InPayments.Any(x => x.TotalWithTax.HasValue) ? order.InPayments.Sum(x => x.TotalWithTax) : null;
                order.PaymentSubTotal = order.InPayments.Any(x => x.Price.HasValue) ? order.InPayments.Sum(x => x.Price) : null;
                order.PaymentSubTotalWithTax = order.InPayments.Any(x => x.PriceWithTax.HasValue) ? order.InPayments.Sum(x => x.PriceWithTax) : null;
                order.PaymentDiscountTotal = order.InPayments.Any(x => x.DiscountAmount.HasValue) ? order.InPayments.Sum(x => x.DiscountAmount) : null;

                var discountAmountWithTax = order.InPayments.Any(x => x.DiscountAmountWithTax.HasValue) ? order.InPayments.Sum(x => x.DiscountAmountWithTax) : null;
                order.PaymentDiscountTotalWithTax = discountAmountWithTax;
                order.DiscountTotalWithTax = discountAmountWithTax != null ? Convert.ToDecimal(order.DiscountTotalWithTax) + discountAmountWithTax : order.DiscountTotalWithTax;

                var discountAmount = order.InPayments.Any(x => x.DiscountAmount.HasValue) ? order.InPayments.Sum(x => x.DiscountAmount) : null;
                order.DiscountTotal = discountAmount != null ? Convert.ToDecimal(order.DiscountTotal) + order.InPayments.Sum(x => x.DiscountAmount) : order.DiscountTotal;

                var taxTotal = order.InPayments.Any(x => x.TaxTotal.HasValue) ? order.InPayments.Sum(x => x.TaxTotal) : null;
                order.TaxTotal = taxTotal != null ? Convert.ToDecimal(order.TaxTotal) + taxTotal : order.TaxTotal;
            }

            var taxFactor = 1 + order.TaxPercentRate ?? 0m;
            order.FeeWithTax = order.Fee * taxFactor;
            order.FeeTotalWithTax = order.FeeTotal * taxFactor;
            order.DiscountTotal += order.DiscountAmount;
            order.DiscountTotalWithTax += order.DiscountAmount * taxFactor;
            //Subtract from order tax total self discount tax amount
            order.TaxTotal -= order.DiscountAmount * order.TaxPercentRate;

            //Need to round all order totals
            order.SubTotal = order.SubTotal != null ? Math.Round((decimal)order.SubTotal, 2, MidpointRounding.AwayFromZero) : order.SubTotal;
            order.SubTotalWithTax = order.SubTotalWithTax != null ? Math.Round((decimal)order.SubTotalWithTax, 2, MidpointRounding.AwayFromZero) : order.SubTotalWithTax;
            order.SubTotalDiscount = order.SubTotalDiscount != null ? Math.Round((decimal)order.SubTotalDiscount, 2, MidpointRounding.AwayFromZero) : order.SubTotalDiscount;
            order.SubTotalDiscountWithTax = order.SubTotalDiscountWithTax != null ? Math.Round((decimal)order.SubTotalDiscountWithTax, 2, MidpointRounding.AwayFromZero) : order.SubTotalDiscountWithTax;
            order.TaxTotal = order.TaxTotal != null ? Math.Round((decimal)order.TaxTotal, 2, MidpointRounding.AwayFromZero) : order.TaxTotal;
            order.DiscountTotal = order.DiscountTotal != null ? Math.Round((decimal)order.DiscountTotal, 2, MidpointRounding.AwayFromZero) : order.DiscountTotal;
            order.DiscountTotalWithTax = order.DiscountTotalWithTax != null ? Math.Round((decimal)order.DiscountTotalWithTax, 2, MidpointRounding.AwayFromZero) : order.DiscountTotalWithTax;
            order.Fee = order.Fee != null ? Math.Round((decimal)order.Fee, 2, MidpointRounding.AwayFromZero) : order.Fee;
            order.FeeWithTax = order.FeeWithTax != null ? Math.Round((decimal)order.FeeWithTax, 2, MidpointRounding.AwayFromZero) : order.FeeWithTax;
            order.FeeTotal = order.FeeTotal != null ? Math.Round((decimal)order.FeeTotal, 2, MidpointRounding.AwayFromZero) : order.FeeTotal;
            order.FeeTotalWithTax = order.FeeTotalWithTax != null ? Math.Round((decimal)order.FeeTotalWithTax, 2, MidpointRounding.AwayFromZero) : order.FeeTotalWithTax;
            order.ShippingTotal = order.ShippingTotal != null ? Math.Round((decimal)order.ShippingTotal, 2, MidpointRounding.AwayFromZero) : order.ShippingTotal;
            order.ShippingTotalWithTax = order.ShippingTotalWithTax != null ? Math.Round((decimal)order.ShippingTotal, 2, MidpointRounding.AwayFromZero) : order.ShippingTotalWithTax;
            order.ShippingSubTotal = order.ShippingSubTotal != null ? Math.Round((decimal)order.ShippingSubTotal, 2, MidpointRounding.AwayFromZero) : order.ShippingSubTotal;
            order.ShippingSubTotalWithTax = order.ShippingSubTotalWithTax != null ? Math.Round((decimal)order.ShippingSubTotalWithTax, 2, MidpointRounding.AwayFromZero) : order.ShippingSubTotalWithTax;
            order.PaymentTotal = order.PaymentTotal != null ? Math.Round((decimal)order.PaymentTotal, 2, MidpointRounding.AwayFromZero) : order.PaymentTotal;
            order.PaymentTotalWithTax = order.PaymentTotalWithTax != null ? Math.Round((decimal)order.PaymentTotalWithTax, 2, MidpointRounding.AwayFromZero) : order.PaymentTotalWithTax;
            order.PaymentSubTotal = order.PaymentSubTotal != null ? Math.Round((decimal)order.PaymentSubTotal, 2, MidpointRounding.AwayFromZero) : order.PaymentSubTotal;
            order.PaymentSubTotalWithTax = order.PaymentSubTotalWithTax != null ? Math.Round((decimal)order.PaymentSubTotalWithTax, 2, MidpointRounding.AwayFromZero) : order.PaymentSubTotalWithTax;
            order.PaymentDiscountTotal = order.PaymentDiscountTotal != null ? Math.Round((decimal)order.PaymentDiscountTotal, 2, MidpointRounding.AwayFromZero) : order.PaymentDiscountTotal;
            order.PaymentDiscountTotalWithTax = order.PaymentDiscountTotalWithTax != null ? Math.Round((decimal)order.PaymentDiscountTotalWithTax, 2, MidpointRounding.AwayFromZero) : order.PaymentDiscountTotalWithTax;

            order.Total = order.SubTotal + order.ShippingSubTotal + order.TaxTotal + order.PaymentSubTotal + order.FeeTotal - order.DiscountTotal;
            order.Sum = order.Total;
        }

        protected virtual void CalculatePaymentTotals(PaymentIn payment)
        {
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment));
            }
            var taxFactor = 1 + payment.TaxPercentRate;
            payment.Total = payment.Price - payment.DiscountAmount;
            payment.TotalWithTax = payment.Total * taxFactor;
            payment.PriceWithTax = payment.Price * taxFactor;
            payment.DiscountAmountWithTax = payment.DiscountAmount * taxFactor;
            payment.TaxTotal = payment.Total * payment.TaxPercentRate;
            payment.Sum = payment.Total;
        }

        protected virtual void CalculateShipmentTotals(Shipment shipment)
        {
            if (shipment == null)
            {
                throw new ArgumentNullException(nameof(shipment));
            }
            var taxFactor = 1 + shipment.TaxPercentRate ?? 0m;
            shipment.PriceWithTax = shipment.Price * taxFactor;
            shipment.DiscountAmountWithTax = shipment.DiscountAmount * taxFactor;
            shipment.FeeWithTax = shipment.Fee * taxFactor;
            shipment.Total = shipment.Price + shipment.Fee - shipment.DiscountAmount;
            shipment.TotalWithTax = shipment.PriceWithTax + shipment.FeeWithTax - shipment.DiscountAmountWithTax;
            shipment.TaxTotal = shipment.Total * shipment.TaxPercentRate;
            shipment.Sum = shipment.Total;
        }

        protected virtual void CalculateLineItemTotals(LineItem lineItem)
        {
            if (lineItem == null)
            {
                throw new ArgumentNullException(nameof(lineItem));
            }
            var taxFactor = 1 + lineItem.TaxPercentRate;
            lineItem.PriceWithTax = lineItem.Price * taxFactor;
            lineItem.PlacedPrice = lineItem.Price - lineItem.DiscountAmount;
            lineItem.ExtendedPrice = lineItem.PlacedPrice * lineItem.Quantity;
            lineItem.DiscountAmountWithTax = lineItem.DiscountAmount * taxFactor;
            lineItem.DiscountTotal = lineItem.DiscountAmount * Math.Max(1, lineItem.Quantity);
            lineItem.FeeWithTax = lineItem.Fee * taxFactor;
            lineItem.PlacedPriceWithTax = lineItem.PlacedPrice * taxFactor;
            lineItem.ExtendedPriceWithTax = lineItem.PlacedPriceWithTax * lineItem.Quantity;
            lineItem.DiscountTotalWithTax = lineItem.DiscountAmountWithTax * Math.Max(1, lineItem.Quantity);
            lineItem.TaxTotal = (lineItem.ExtendedPrice + lineItem.Fee) * lineItem.TaxPercentRate;
        }
    }
}
