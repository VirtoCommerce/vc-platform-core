using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CartModule.Core;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Web.Controllers.Api
{
    [Route("api/carts")]
    [EnableCors]
    public class CartModuleController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartSearchService _searchService;
        private readonly IShoppingCartBuilder _cartBuilder;

        public CartModuleController(IShoppingCartService shoppingCartService, IShoppingCartSearchService searchService, IShoppingCartBuilder cartBuilder)
        {
            _shoppingCartService = shoppingCartService;
            _searchService = searchService;
            _cartBuilder = cartBuilder;
        }

        [HttpGet]
        [Route("{storeId}/{customerId}/{cartName}/{currency}/{cultureName}/current")]
        [ProducesResponseType(typeof(ShoppingCart), 200)]
        public async Task<IActionResult> GetCart(string storeId, string customerId, string cartName, string currency, string cultureName)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(string.Join(":", storeId, customerId, cartName, currency))).LockAsync())
            {
                await _cartBuilder.GetOrCreateCartAsync(storeId, customerId, cartName, currency, cultureName);
            }
            return Ok(_cartBuilder.Cart);
        }

        [HttpGet]
        [Route("{cartId}/itemscount")]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<IActionResult> GetCartItemsCount(string cartId)
        {
            var carts = await _shoppingCartService.GetByIdsAsync(new[] {cartId});
            var cart = carts.FirstOrDefault();
            return Ok(cart?.Items?.Count ?? 0);
        }

        [HttpPost]
        [Route("{cartId}/items")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> AddItemToCart(string cartId, [FromBody] LineItem lineItem)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(cartId)).LockAsync())
            {
                var cart = await _shoppingCartService.GetByIdAsync(cartId);
                await _cartBuilder.TakeCart(cart).AddItem(lineItem).SaveAsync();
            }
            return Ok();
        }

        [HttpPut]
        [Route("{cartId}/items")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> ChangeCartItem(string cartId, string lineItemId, int quantity)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(cartId)).LockAsync())
            {
                var cart = await _shoppingCartService.GetByIdAsync(cartId);
                _cartBuilder.TakeCart(cart);
                var lineItem = _cartBuilder.Cart.Items.FirstOrDefault(i => i.Id == lineItemId);
                if (lineItem != null)
                {
                    await _cartBuilder.ChangeItemQuantity(lineItemId, quantity).SaveAsync();
                }
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{cartId}/items/{lineItemId}")]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<IActionResult> RemoveCartItem(string cartId, string lineItemId)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(cartId)).LockAsync())
            {
                var cart = await _shoppingCartService.GetByIdAsync(cartId);
                await _cartBuilder.TakeCart(cart).RemoveItem(lineItemId).SaveAsync();
            }
            return Ok(_cartBuilder.Cart.Items.Count);
        }

        [HttpDelete]
        [Route("{cartId}/items")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> ClearCart(string cartId)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(cartId)).LockAsync())
            {
                var cart = await _shoppingCartService.GetByIdAsync(cartId);
                await _cartBuilder.TakeCart(cart).Clear().SaveAsync();
            }
            return Ok();
        }

        [HttpPatch]
        [Route("{cartId}")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> MergeWithCart(string cartId, [FromBody]ShoppingCart otherCart)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(cartId)).LockAsync())
            {
                var cart = await _shoppingCartService.GetByIdAsync(cartId);
                var builder = await _cartBuilder.TakeCart(cart).MergeWithCartAsync(otherCart);
                await builder.SaveAsync();
            }
            return Ok();
        }

        //TODO
        //[HttpGet]
        //[Route("{cartId}/availshippingrates")]
        //[ProducesResponseType(typeof(ICollection<ShippingRate>), 200)]
        //public IActionResult GetAvailableShippingRates(string cartId)
        //{
        //    var cart = _shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault();
        //    var shippingRates = _cartBuilder.TakeCart(cart).GetAvailableShippingRates();
        //    return Ok(shippingRates);
        //}

        //[HttpPost]
        //[Route("availshippingrates")]
        //[ProducesResponseType(typeof(ICollection<ShippingRate>), 200)]
        //public IActionResult GetAvailableShippingRatesByContext(ShippingEvaluationContext context)
        //{
        //    var shippingRates = _cartBuilder.TakeCart(context.ShoppingCart).GetAvailableShippingRates();
        //    return Ok(shippingRates);
        //}

        //[HttpGet]
        //[Route("{cartId}/availpaymentmethods")]
        //[ProducesResponseType(typeof(ICollection<Domain.Payment.Model.PaymentMethod>), 200)]
        //public IActionResult GetAvailablePaymentMethods(string cartId)
        //{
        //    var cart = _shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault();
        //    var paymentMethods = _cartBuilder.TakeCart(cart).GetAvailablePaymentMethods();
        //    return Ok(paymentMethods);
        //}

        [HttpPost]
        [Route("{cartId}/coupons/{couponCode}")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> AddCartCoupon(string cartId, string couponCode)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(cartId)).LockAsync())
            {
                var cart = await _shoppingCartService.GetByIdAsync(cartId);
                await _cartBuilder.TakeCart(cart).AddCoupon(couponCode).SaveAsync();
            }
            return Ok();
        }

        [HttpDelete]
        [Route("{cartId}/coupons/{couponCode}")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> RemoveCartCoupon(string cartId, string couponCode)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(cartId)).LockAsync())
            {
                var cart = await _shoppingCartService.GetByIdAsync(cartId);
                await _cartBuilder.TakeCart(cart).RemoveCoupon().SaveAsync();
            }
            return Ok();
        }

        //[HttpPost]
        //[Route("{cartId}/shipments")]
        //[ProducesResponseType(typeof(void), 200)]
        //public async Task<IActionResult> AddOrUpdateCartShipment(string cartId, [FromBody] Shipment shipment)
        //{
        //    using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(cartId)).LockAsync())
        //    {
        //        var cart = _shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault();
        //        _cartBuilder.TakeCart(cart).AddOrUpdateShipment(shipment).Save();
        //    }
        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //[HttpPost]
        //[Route("{cartId}/payments")]
        //[ProducesResponseType(typeof(void), 200)]
        //public async Task<IActionResult> AddOrUpdateCartPayment(string cartId, [FromBody] Payment payment)
        //{
        //    using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(cartId)).LockAsync())
        //    {
        //        var cart = _shoppingCartService.GetByIds(new[] { cartId }).FirstOrDefault();
        //        _cartBuilder.TakeCart(cart).AddOrUpdatePayment(payment).Save();
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        /// <summary>
        /// Get shopping cart by id
        /// </summary>
        /// <param name="cartId">Shopping cart id</param>
        [HttpGet]
        [Route("{cartId}")]
        [ProducesResponseType(typeof(ShoppingCart), 200)]
        public async Task<IActionResult> GetCartById(string cartId)
        {
            var cart = await _shoppingCartService.GetByIdAsync(cartId);
            return Ok(cart);
        }

        /// <summary>
        /// Search shopping carts by given criteria
        /// </summary>
        /// <param name="criteria">Shopping cart search criteria</param>
        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(GenericSearchResult<ShoppingCart>), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<IActionResult> Search(ShoppingCartSearchCriteria criteria)
        {
            var result = await _searchService.SearchCartAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Create shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart model</param>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(ShoppingCart), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<IActionResult> Create(ShoppingCart cart)
        {
            await _shoppingCartService.SaveChangesAsync(new[] { cart });
            return Ok(cart);
        }

        /// <summary>
        /// Update shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart model</param>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(ShoppingCart), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> Update(ShoppingCart cart)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(cart.Id)).LockAsync())
            {
                await _shoppingCartService.SaveChangesAsync(new[] { cart });
            }
            return Ok(cart);
        }


        /// <summary>
        /// Delete shopping carts by ids
        /// </summary>
        /// <param name="ids">Array of shopping cart ids</param>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<IActionResult> DeleteCarts([FromRoute] string[] ids)
        {
            await _shoppingCartService.DeleteAsync(ids);
            return Ok();
        }

        private static string GetAsyncLockCartKey(string cartId)
        {
            return "Cart:" + cartId;
        }

    }
}
