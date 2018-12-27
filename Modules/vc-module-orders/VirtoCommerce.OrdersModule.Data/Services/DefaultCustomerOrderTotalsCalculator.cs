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

            order.DiscountTotal = 0m;
            order.DiscountTotalWithTax = 0m;
            order.FeeTotal = order.Fee;
            order.TaxTotal = 0m;

            if (!order.Items.IsNullOrEmpty())
            {
                order.SubTotal = order.Items.Sum(x => x.Price * x.Quantity);
                order.SubTotalWithTax = order.Items.Sum(x => x.PriceWithTax * x.Quantity);
                order.SubTotalTaxTotal += order.Items.Sum(x => x.TaxTotal);
                order.SubTotalDiscount = order.Items.Sum(x => x.DiscountTotal);
                order.SubTotalDiscountWithTax = order.Items.Sum(x => x.DiscountTotalWithTax);
                order.DiscountTotal += order.Items.Sum(x => x.DiscountTotal);
                order.DiscountTotalWithTax += order.Items.Sum(x => x.DiscountTotalWithTax);
                order.FeeTotal += order.Items.Sum(x => x.Fee);
                order.FeeTotalWithTax += order.Items.Sum(x => x.FeeWithTax);
                order.TaxTotal += order.Items.Sum(x => x.TaxTotal);
            }

            if (!order.Shipments.IsNullOrEmpty())
            {
                order.ShippingTotal = order.Shipments.Sum(x => x.Total);
                order.ShippingTotalWithTax = order.Shipments.Sum(x => x.TotalWithTax);
                order.ShippingSubTotal = order.Shipments.Sum(x => x.Price);
                order.ShippingSubTotalWithTax = order.Shipments.Sum(x => x.PriceWithTax);
                order.ShippingDiscountTotal = order.Shipments.Sum(x => x.DiscountAmount);
                order.ShippingDiscountTotalWithTax = order.Shipments.Sum(x => x.DiscountAmountWithTax);
                order.DiscountTotal += order.Shipments.Sum(x => x.DiscountAmount);
                order.DiscountTotalWithTax += order.Shipments.Sum(x => x.DiscountAmountWithTax);
                order.FeeTotal += order.Shipments.Sum(x => x.Fee);
                order.FeeTotalWithTax += order.Shipments.Sum(x => x.FeeWithTax);
                order.TaxTotal += order.Shipments.Sum(x => x.TaxTotal);
            }

            if (!order.InPayments.IsNullOrEmpty())
            {
                order.PaymentTotal = order.InPayments.Sum(x => x.Total);
                order.PaymentTotalWithTax = order.InPayments.Sum(x => x.TotalWithTax);
                order.PaymentSubTotal = order.InPayments.Sum(x => x.Price);
                order.PaymentSubTotalWithTax = order.InPayments.Sum(x => x.PriceWithTax);
                order.PaymentDiscountTotal = order.InPayments.Sum(x => x.DiscountAmount);
                order.PaymentDiscountTotalWithTax = order.InPayments.Sum(x => x.DiscountAmountWithTax);
                order.DiscountTotal += order.InPayments.Sum(x => x.DiscountAmount);
                order.DiscountTotalWithTax += order.InPayments.Sum(x => x.DiscountAmountWithTax);
                order.TaxTotal += order.InPayments.Sum(x => x.TaxTotal);
            }

            var taxFactor = 1 + order.TaxPercentRate;
            order.FeeWithTax = order.Fee * taxFactor;
            order.FeeTotalWithTax = order.FeeTotal * taxFactor;
            order.DiscountTotal += order.DiscountAmount;
            order.DiscountTotalWithTax += order.DiscountAmount * taxFactor;
            //Subtract from order tax total self discount tax amount
            order.TaxTotal -= order.DiscountAmount * order.TaxPercentRate;

            //Need to round all order totals
            order.SubTotal = Math.Round(order.SubTotal, 2, MidpointRounding.AwayFromZero);
            order.SubTotalWithTax = Math.Round(order.SubTotalWithTax, 2, MidpointRounding.AwayFromZero);
            order.SubTotalDiscount = Math.Round(order.SubTotalDiscount, 2, MidpointRounding.AwayFromZero);
            order.SubTotalDiscountWithTax = Math.Round(order.SubTotalDiscountWithTax, 2, MidpointRounding.AwayFromZero);
            order.TaxTotal = Math.Round(order.TaxTotal, 2, MidpointRounding.AwayFromZero);
            order.DiscountTotal = Math.Round(order.DiscountTotal, 2, MidpointRounding.AwayFromZero);
            order.DiscountTotalWithTax = Math.Round(order.DiscountTotalWithTax, 2, MidpointRounding.AwayFromZero);
            order.Fee = Math.Round(order.Fee, 2, MidpointRounding.AwayFromZero);
            order.FeeWithTax = Math.Round(order.FeeWithTax, 2, MidpointRounding.AwayFromZero);
            order.FeeTotal = Math.Round(order.FeeTotal, 2, MidpointRounding.AwayFromZero);
            order.FeeTotalWithTax = Math.Round(order.FeeTotalWithTax, 2, MidpointRounding.AwayFromZero);
            order.ShippingTotal = Math.Round(order.ShippingTotal, 2, MidpointRounding.AwayFromZero);
            order.ShippingTotalWithTax = Math.Round(order.ShippingTotal, 2, MidpointRounding.AwayFromZero);
            order.ShippingSubTotal = Math.Round(order.ShippingSubTotal, 2, MidpointRounding.AwayFromZero);
            order.ShippingSubTotalWithTax = Math.Round(order.ShippingSubTotalWithTax, 2, MidpointRounding.AwayFromZero);
            order.PaymentTotal = Math.Round(order.PaymentTotal, 2, MidpointRounding.AwayFromZero);
            order.PaymentTotalWithTax = Math.Round(order.PaymentTotalWithTax, 2, MidpointRounding.AwayFromZero);
            order.PaymentSubTotal = Math.Round(order.PaymentSubTotal, 2, MidpointRounding.AwayFromZero);
            order.PaymentSubTotalWithTax = Math.Round(order.PaymentSubTotalWithTax, 2, MidpointRounding.AwayFromZero);
            order.PaymentDiscountTotal = Math.Round(order.PaymentDiscountTotal, 2, MidpointRounding.AwayFromZero);
            order.PaymentDiscountTotalWithTax = Math.Round(order.PaymentDiscountTotalWithTax, 2, MidpointRounding.AwayFromZero);

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
            var taxFactor = 1 + shipment.TaxPercentRate;
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
