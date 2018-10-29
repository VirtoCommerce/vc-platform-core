using System;
using System.Linq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Services
{
    /// <summary>
    /// Respond for totals values calculation for Shopping cart and all nested objects
    /// </summary>
    public class DefaultShoppingCartTotalsCalculator : IShoppingCartTotalsCalculator
    {
        /// <summary>
        /// Cart subtotal discount
        /// When a discount is applied to the cart subtotal, the tax calculation has already been applied, and is reflected in the tax subtotal.
        /// Therefore, a discount applying to the cart subtotal will occur after tax.
        /// For instance, if the cart subtotal is $100, and $15 is the tax subtotal, a cart - wide discount of 10 % will yield a total of $105($100 subtotal – $10 discount + $15 tax on the original $100).
        /// </summary>
        public virtual void CalculateTotals(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }

            //Calculate totals for line items
            if (!cart.Items.IsNullOrEmpty())
            {
                foreach (var item in cart.Items)
                {
                    CalculateLineItemTotals(item);
                }
            }
            //Calculate totals for shipments
            if (!cart.Shipments.IsNullOrEmpty())
            {
                foreach (var shipment in cart.Shipments)
                {
                    CalculateShipmentTotals(shipment);
                }
            }
            //Calculate totals for payments
            if (!cart.Payments.IsNullOrEmpty())
            {
                foreach (var payment in cart.Payments)
                {
                    CalculatePaymentTotals(payment);
                }
            }

            cart.DiscountTotal = 0m;
            cart.DiscountTotalWithTax = 0m;
            cart.FeeTotal = cart.Fee;
            cart.TaxTotal = 0m;

            if (!cart.Items.IsNullOrEmpty())
            {
                cart.SubTotal = cart.Items.Sum(x => x.ListPrice * x.Quantity);
                cart.SubTotalWithTax = cart.Items.Sum(x => x.ListPriceWithTax * x.Quantity);
                cart.SubTotalDiscount = cart.Items.Sum(x => x.DiscountTotal);
                cart.SubTotalDiscountWithTax = cart.Items.Sum(x => x.DiscountTotalWithTax);
                cart.DiscountTotal += cart.Items.Sum(x => x.DiscountTotal);
                cart.DiscountTotalWithTax += cart.Items.Sum(x => x.DiscountTotalWithTax);
                cart.FeeTotal += cart.Items.Sum(x => x.Fee);
                cart.FeeTotalWithTax += cart.Items.Sum(x => x.FeeWithTax);
                cart.TaxTotal += cart.Items.Sum(x => x.TaxTotal);
            }

            if (!cart.Shipments.IsNullOrEmpty())
            {
                cart.ShippingTotal = cart.Shipments.Sum(x => x.Total);
                cart.ShippingTotalWithTax = cart.Shipments.Sum(x => x.TotalWithTax);
                cart.ShippingSubTotal = cart.Shipments.Sum(x => x.Price);
                cart.ShippingSubTotalWithTax = cart.Shipments.Sum(x => x.PriceWithTax);
                cart.ShippingDiscountTotal = cart.Shipments.Sum(x => x.DiscountAmount);
                cart.ShippingDiscountTotalWithTax = cart.Shipments.Sum(x => x.DiscountAmountWithTax);
                cart.DiscountTotal += cart.Shipments.Sum(x => x.DiscountAmount);
                cart.DiscountTotalWithTax += cart.Shipments.Sum(x => x.DiscountAmountWithTax);
                cart.FeeTotal += cart.Shipments.Sum(x => x.Fee);
                cart.FeeTotalWithTax += cart.Shipments.Sum(x => x.FeeWithTax);
                cart.TaxTotal += cart.Shipments.Sum(x => x.TaxTotal);
            }

            if (!cart.Payments.IsNullOrEmpty())
            {
                cart.PaymentTotal = cart.Payments.Sum(x => x.Total);
                cart.PaymentTotalWithTax = cart.Payments.Sum(x => x.TotalWithTax);
                cart.PaymentSubTotal = cart.Payments.Sum(x => x.Price);
                cart.PaymentSubTotalWithTax = cart.Payments.Sum(x => x.PriceWithTax);
                cart.PaymentDiscountTotal = cart.Payments.Sum(x => x.DiscountAmount);
                cart.PaymentDiscountTotalWithTax = cart.Payments.Sum(x => x.DiscountAmountWithTax);
                cart.DiscountTotal += cart.Payments.Sum(x => x.DiscountAmount);
                cart.DiscountTotalWithTax += cart.Payments.Sum(x => x.DiscountAmountWithTax);
                cart.TaxTotal += cart.Payments.Sum(x => x.TaxTotal);
            }

            var taxFactor = 1 + cart.TaxPercentRate;
            cart.FeeWithTax = cart.Fee * taxFactor;
            cart.FeeTotalWithTax = cart.FeeTotal * taxFactor;
            cart.DiscountTotal += cart.DiscountAmount;
            cart.DiscountTotalWithTax += cart.DiscountAmount * taxFactor;
            //Subtract from cart tax total self discount tax amount
            cart.TaxTotal -= cart.DiscountAmount * cart.TaxPercentRate;

            //Need to round all cart totals
            cart.SubTotal = Math.Round(cart.SubTotal, 2, MidpointRounding.AwayFromZero);
            cart.SubTotalWithTax = Math.Round(cart.SubTotalWithTax, 2, MidpointRounding.AwayFromZero);
            cart.SubTotalDiscount = Math.Round(cart.SubTotalDiscount, 2, MidpointRounding.AwayFromZero);
            cart.SubTotalDiscountWithTax = Math.Round(cart.SubTotalDiscountWithTax, 2, MidpointRounding.AwayFromZero);
            cart.TaxTotal = Math.Round(cart.TaxTotal, 2, MidpointRounding.AwayFromZero);
            cart.DiscountTotal = Math.Round(cart.DiscountTotal, 2, MidpointRounding.AwayFromZero);
            cart.DiscountTotalWithTax = Math.Round(cart.DiscountTotalWithTax, 2, MidpointRounding.AwayFromZero);
            cart.Fee = Math.Round(cart.Fee, 2, MidpointRounding.AwayFromZero);
            cart.FeeWithTax = Math.Round(cart.FeeWithTax, 2, MidpointRounding.AwayFromZero);
            cart.FeeTotal = Math.Round(cart.FeeTotal, 2, MidpointRounding.AwayFromZero);
            cart.FeeTotalWithTax = Math.Round(cart.FeeTotalWithTax, 2, MidpointRounding.AwayFromZero);
            cart.ShippingTotal = Math.Round(cart.ShippingTotal, 2, MidpointRounding.AwayFromZero);
            cart.ShippingTotalWithTax = Math.Round(cart.ShippingTotalWithTax, 2, MidpointRounding.AwayFromZero);
            cart.ShippingSubTotal = Math.Round(cart.ShippingSubTotal, 2, MidpointRounding.AwayFromZero);
            cart.ShippingSubTotalWithTax = Math.Round(cart.ShippingSubTotalWithTax, 2, MidpointRounding.AwayFromZero);
            cart.PaymentTotal = Math.Round(cart.PaymentTotal, 2, MidpointRounding.AwayFromZero);
            cart.PaymentTotalWithTax = Math.Round(cart.PaymentTotalWithTax, 2, MidpointRounding.AwayFromZero);
            cart.PaymentSubTotal = Math.Round(cart.PaymentSubTotal, 2, MidpointRounding.AwayFromZero);
            cart.PaymentSubTotalWithTax = Math.Round(cart.PaymentSubTotalWithTax, 2, MidpointRounding.AwayFromZero);
            cart.PaymentDiscountTotal = Math.Round(cart.PaymentDiscountTotal, 2, MidpointRounding.AwayFromZero);
            cart.PaymentDiscountTotalWithTax = Math.Round(cart.PaymentDiscountTotalWithTax, 2, MidpointRounding.AwayFromZero);

            cart.Total = cart.SubTotal + cart.ShippingSubTotal + cart.TaxTotal + cart.PaymentSubTotal + cart.FeeTotal - cart.DiscountTotal;
        }

        protected virtual void CalculatePaymentTotals(Payment payment)
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
        }

        protected virtual void CalculateLineItemTotals(LineItem lineItem)
        {
            if (lineItem == null)
            {
                throw new ArgumentNullException(nameof(lineItem));
            }
            var taxFactor = 1 + lineItem.TaxPercentRate;
            lineItem.ListPriceWithTax = lineItem.ListPrice * taxFactor;
            lineItem.SalePriceWithTax = lineItem.SalePrice * taxFactor;
            lineItem.PlacedPrice = lineItem.ListPrice - lineItem.DiscountAmount;
            lineItem.PlacedPriceWithTax = lineItem.PlacedPrice * taxFactor;
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
