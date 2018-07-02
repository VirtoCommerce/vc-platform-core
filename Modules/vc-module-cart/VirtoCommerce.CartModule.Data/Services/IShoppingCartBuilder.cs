using System.Collections.Generic;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.CartModule.Data.Services
{
    /// <summary>
    /// Represent abstraction for working with customer shopping cart
    /// </summary>
    public interface IShoppingCartBuilder
    {
        ///// <summary>
        /////  Capture passed cart and all next changes will be implemented on it
        ///// </summary>
        ///// <param name="cart"></param>
        ///// <returns></returns>
        //IShoppingCartBuilder TakeCart(ShoppingCart cart);

        ///// <summary>
        ///// Load or created new cart with specified parameters
        ///// </summary>
        ///// <returns></returns>
        //IShoppingCartBuilder GetOrCreateCart(string storeId, string customerId, string cartName, string currency, string cultureName);

        ///// <summary>
        ///// Add new lineitem  to cart
        ///// </summary>
        ///// <param name="lineItem"></param>
        ///// <returns></returns>
        //IShoppingCartBuilder AddItem(LineItem lineItem);

        ///// <summary>
        ///// Change cart item qty by product index
        ///// </summary>
        ///// <param name="lineItemId"></param>
        ///// <param name="quantity"></param>
        ///// <returns></returns>
        //IShoppingCartBuilder ChangeItemQuantity(string lineItemId, int quantity);

        ///// <summary>
        ///// Remove item from cart by id
        ///// </summary>
        ///// <param name="lineItemId"></param>
        ///// <returns></returns>
        //IShoppingCartBuilder RemoveItem(string lineItemId);

        ///// <summary>
        ///// Apply marketing coupon to captured cart
        ///// </summary>
        ///// <param name="couponCode"></param>
        ///// <returns></returns>
        //IShoppingCartBuilder AddCoupon(string couponCode);

        ///// <summary>
        ///// remove exist coupon from cart
        ///// </summary>
        ///// <returns></returns>
        //IShoppingCartBuilder RemoveCoupon();

        ///// <summary>
        ///// Clear cart remove all items and shipments and payments
        ///// </summary>
        ///// <returns></returns>
        //IShoppingCartBuilder Clear();

        ///// <summary>
        ///// Add or update shipment to cart
        ///// </summary>
        ///// <param name="shipment"></param>
        ///// <returns></returns>
        //IShoppingCartBuilder AddOrUpdateShipment(Shipment shipment);

        ///// <summary>
        ///// Remove exist shipment from cart
        ///// </summary>
        ///// <param name="shipmentId"></param>
        ///// <returns></returns>
        //IShoppingCartBuilder RemoveShipment(string shipmentId);

        ///// <summary>
        ///// Add or update payment in cart
        ///// </summary>
        ///// <param name="payment"></param>
        ///// <returns></returns>
        //IShoppingCartBuilder AddOrUpdatePayment(Payment payment);

        ///// <summary>
        ///// Merge other cart with captured
        ///// </summary>
        ///// <param name="cart"></param>
        ///// <returns></returns>
        //IShoppingCartBuilder MergeWithCart(ShoppingCart cart);

        ///// <summary>
        ///// Remove cart from service
        ///// </summary>
        ///// <returns></returns>
        //IShoppingCartBuilder RemoveCart();

        ///// <summary>
        ///// Returns all available shipment methods for current cart
        ///// </summary>
        ///// <returns></returns>
        //ICollection<ShippingRate> GetAvailableShippingRates();

        ///// <summary>
        ///// Returns all available payment methods for current cart
        ///// </summary>
        ///// <returns></returns>
        //ICollection<Domain.Payment.Model.PaymentMethod> GetAvailablePaymentMethods();


        ////Save cart changes
        //void Save();

        //ShoppingCart Cart { get; }
    }
}
