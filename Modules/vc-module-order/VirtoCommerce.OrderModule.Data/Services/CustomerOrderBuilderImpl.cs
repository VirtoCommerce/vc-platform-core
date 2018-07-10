using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Model.Payment;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.OrderModule.Core.Model;
using VirtoCommerce.OrderModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrderModule.Data.Services
{
    public class CustomerOrderBuilderImpl : ICustomerOrderBuilder
    {
        private readonly ICustomerOrderService _customerOrderService;

        public CustomerOrderBuilderImpl(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }

        //TODO after CartModule
        //#region ICustomerOrderConverter Members


        //public virtual CustomerOrder PlaceCustomerOrderFromCart(ShoppingCart cart)
        //{
        //    var customerOrder = ConvertCartToOrder(cart);
        //    _customerOrderService.SaveChanges(new[] { customerOrder });
        //    return customerOrder;
        //}

        //#endregion
        //protected virtual CustomerOrder ConvertCartToOrder(ShoppingCart cart)
        //{
        //    var retVal = AbstractTypeFactory<CustomerOrder>.TryCreateInstance();
        //    retVal.ShoppingCartId = cart.Id;
        //    retVal.Comment = cart.Comment;
        //    retVal.Currency = cart.Currency;
        //    retVal.ChannelId = cart.ChannelId;
        //    retVal.CustomerId = cart.CustomerId;
        //    retVal.CustomerName = cart.CustomerName;
        //    retVal.DiscountAmount = cart.DiscountAmount;
        //    retVal.OrganizationId = cart.OrganizationId;
        //    retVal.StoreId = cart.StoreId;
        //    retVal.TaxPercentRate = cart.TaxPercentRate;
        //    retVal.TaxType = cart.TaxType;
        //    retVal.LanguageCode = cart.LanguageCode;

        //    retVal.Status = "New";

        //    var cartLineItemsMap = new Dictionary<string, LineItem>();
        //    if (cart.Items != null)
        //    {
        //        retVal.Items = new List<LineItem>();
        //        foreach (var cartLineItem in cart.Items)
        //        {
        //            var orderLineItem = ToOrderModel(cartLineItem);
        //            retVal.Items.Add(orderLineItem);
        //            cartLineItemsMap.Add(cartLineItem.Id, orderLineItem);
        //        }
        //    }
        //    if (cart.Discounts != null)
        //    {
        //        retVal.Discounts = cart.Discounts.Select(ToOrderModel).ToList();
        //    }

        //    if (cart.Addresses != null)
        //    {
        //        retVal.Addresses = cart.Addresses.ToList();
        //    }

        //    if (cart.Shipments != null)
        //    {
        //        retVal.Shipments = new List<Shipment>();
        //        foreach (var cartShipment in cart.Shipments)
        //        {
        //            var shipment = ToOrderModel(cartShipment);
        //            if (!cartShipment.Items.IsNullOrEmpty())
        //            {
        //                shipment.Items = new List<ShipmentItem>();
        //                foreach (var cartShipmentItem in cartShipment.Items)
        //                {
        //                    var shipmentItem = ToOrderModel(cartShipmentItem);
        //                    if (cartLineItemsMap.ContainsKey(cartShipmentItem.LineItemId))
        //                    {
        //                        shipmentItem.LineItem = cartLineItemsMap[cartShipmentItem.LineItemId];
        //                        shipment.Items.Add(shipmentItem);
        //                    }
        //                }
        //            }
        //            retVal.Shipments.Add(shipment);
        //        }
        //        //Add shipping address to order
        //        retVal.Addresses.AddRange(retVal.Shipments.Where(x => x.DeliveryAddress != null).Select(x => x.DeliveryAddress));

        //    }
        //    if (cart.Payments != null)
        //    {
        //        retVal.InPayments = new List<PaymentIn>();
        //        foreach (var payment in cart.Payments)
        //        {
        //            var paymentIn = ToOrderModel(payment);
        //            paymentIn.CustomerId = cart.CustomerId;
        //            retVal.InPayments.Add(paymentIn);
        //            if (payment.BillingAddress != null)
        //            {
        //                retVal.Addresses.Add(payment.BillingAddress);
        //            }
        //        }
        //    }

        //    //Save only disctinct addresses for order
        //    retVal.Addresses = retVal.Addresses.Distinct().ToList();
        //    foreach (var address in retVal.Addresses)
        //    {
        //        //Reset primary key for addresses
        //        address.Key = null;
        //    }
        //    retVal.TaxDetails = cart.TaxDetails;
        //    return retVal;
        //}

        protected virtual LineItem ToOrderModel(LineItem lineItem)
        {
            if (lineItem == null)
                throw new ArgumentNullException(nameof(lineItem));

            var retVal = AbstractTypeFactory<LineItem>.TryCreateInstance();

            retVal.CreatedDate = lineItem.CreatedDate;
            retVal.CatalogId = lineItem.CatalogId;
            retVal.CategoryId = lineItem.CategoryId;
            retVal.Comment = lineItem.Comment;
            retVal.Currency = lineItem.Currency;
            retVal.Height = lineItem.Height;
            retVal.ImageUrl = lineItem.ImageUrl;
            retVal.IsGift = lineItem.IsGift;
            retVal.Length = lineItem.Length;
            retVal.MeasureUnit = lineItem.MeasureUnit;
            retVal.Name = lineItem.Name;
            retVal.PriceId = lineItem.PriceId;
            retVal.ProductId = lineItem.ProductId;
            retVal.ProductType = lineItem.ProductType;
            retVal.Quantity = lineItem.Quantity;
            retVal.Sku = lineItem.Sku;
            retVal.TaxPercentRate = lineItem.TaxPercentRate;
            retVal.TaxType = lineItem.TaxType;
            retVal.Weight = lineItem.Weight;
            retVal.WeightUnit = lineItem.WeightUnit;
            retVal.Width = lineItem.Width;
            retVal.FulfillmentCenterId = lineItem.FulfillmentCenterId;
            retVal.FulfillmentCenterName = lineItem.FulfillmentCenterName;

            retVal.DiscountAmount = lineItem.DiscountAmount;
            retVal.Price = lineItem.Price;

            retVal.FulfillmentLocationCode = lineItem.FulfillmentLocationCode;
            retVal.DynamicProperties = null; //to prevent copy dynamic properties from ShoppingCart LineItem to Order LineItem
            if (lineItem.Discounts != null)
            {
                retVal.Discounts = lineItem.Discounts.Select(ToOrderModel).ToList();
            }
            retVal.TaxDetails = lineItem.TaxDetails;
            return retVal;
        }

        protected virtual Discount ToOrderModel(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var retVal = AbstractTypeFactory<Discount>.TryCreateInstance();

            retVal.Coupon = discount.Coupon;
            retVal.Currency = discount.Currency;
            retVal.Description = discount.Description;
            retVal.DiscountAmount = discount.DiscountAmount;
            retVal.DiscountAmountWithTax = discount.DiscountAmountWithTax;
            retVal.PromotionId = discount.PromotionId;

            return retVal;
        }

        protected virtual Shipment ToOrderModel(Shipment shipment)
        {
            var retVal = AbstractTypeFactory<Shipment>.TryCreateInstance();

            retVal.Currency = shipment.Currency;
            retVal.DiscountAmount = shipment.DiscountAmount;
            retVal.Height = shipment.Height;
            retVal.Length = shipment.Length;
            retVal.MeasureUnit = shipment.MeasureUnit;
            retVal.FulfillmentCenterId = shipment.FulfillmentCenterId;
            retVal.FulfillmentCenterName = shipment.FulfillmentCenterName;
            retVal.ShipmentMethodCode = shipment.ShipmentMethodCode;
            retVal.ShipmentMethodOption = shipment.ShipmentMethodOption;
            retVal.Sum = shipment.Total;
            retVal.Weight = shipment.Weight;
            retVal.WeightUnit = shipment.WeightUnit;
            retVal.Width = shipment.Width;
            retVal.TaxPercentRate = shipment.TaxPercentRate;
            retVal.DiscountAmount = shipment.DiscountAmount;
            retVal.Price = shipment.Price;
            retVal.Status = "New";
            if (shipment.DeliveryAddress != null)
            {
                retVal.DeliveryAddress = shipment.DeliveryAddress;
                retVal.DeliveryAddress.Key = null;
            }
            if (shipment.Discounts != null)
            {
                retVal.Discounts = shipment.Discounts.Select(ToOrderModel).ToList();
            }
            retVal.TaxDetails = shipment.TaxDetails;
            return retVal;
        }

        protected virtual ShipmentItem ToOrderModel(ShipmentItem shipmentItem)
        {
            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            var retVal = AbstractTypeFactory<ShipmentItem>.TryCreateInstance();
            retVal.BarCode = shipmentItem.BarCode;
            retVal.Quantity = shipmentItem.Quantity;
            return retVal;
        }

        //TODO
        //protected virtual PaymentIn ToOrderModel(Payment payment)
        //{
        //    if (payment == null)
        //        throw new ArgumentNullException(nameof(payment));

        //    var retVal = AbstractTypeFactory<PaymentIn>.TryCreateInstance();
        //    retVal.Currency = payment.Currency;
        //    retVal.DiscountAmount = payment.DiscountAmount;
        //    retVal.Price = payment.Price;
        //    retVal.TaxPercentRate = payment.TaxPercentRate;
        //    retVal.TaxType = payment.TaxType;

        //    retVal.GatewayCode = payment.PaymentGatewayCode;
        //    retVal.Sum = payment.Amount;
        //    retVal.PaymentStatus = PaymentStatus.New;
        //    if (payment.BillingAddress != null)
        //    {
        //        retVal.BillingAddress = payment.BillingAddress;
        //        retVal.BillingAddress.Key = null;
        //    }
        //    retVal.TaxDetails = payment.TaxDetails;
        //    return retVal;
        //}
    }
}
