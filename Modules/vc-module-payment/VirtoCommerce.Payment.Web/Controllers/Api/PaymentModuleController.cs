using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Contexts;
using VirtoCommerce.PaymentModule.Core.Models;
using VirtoCommerce.PaymentModule.Core.Models.Search;
using VirtoCommerce.PaymentModule.Core.PaymentResults;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.PaymentModule.Web.Models;
using VirtoCommerce.Platform.Core.Common;
using KeyValuePair = VirtoCommerce.PaymentModule.Web.Models.KeyValuePair;

namespace VirtoCommerce.PaymentModule.Web.Controllers.Api
{
    [Route("api/payment")]
    public class PaymentModuleController : Controller
    {
        private readonly ICustomerOrderSearchService _searchService;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;
        private readonly IPaymentMethodsService _paymentMethodsService;

        public PaymentModuleController(
            ICustomerOrderSearchService searchService,
            ICustomerOrderService customerOrderService,
            IPaymentMethodsSearchService paymentMethodsSearchService,
            IPaymentMethodsService paymentMethodsService
            )
        {
            _searchService = searchService;
            _customerOrderService = customerOrderService;
            _paymentMethodsSearchService = paymentMethodsSearchService;
            _paymentMethodsService = paymentMethodsService;
        }

        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<PaymentMethodsSearchResult>> SearchPaymentMethods([FromBody]PaymentMethodsSearchCriteria criteria)
        {
            var result = await _paymentMethodsSearchService.SearchPaymentMethodsAsync(criteria);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<PaymentMethod>> GetPaymentMethodById(string id)
        {
            var result = await _paymentMethodsService.GetByIdAsync(id, null);
            return Ok(result);
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<PaymentMethod>> UpdatePaymentMethod([FromBody]PaymentMethod paymentMethod)
        {
            await _paymentMethodsService.SaveChangesAsync(new[] { paymentMethod });
            return Ok(paymentMethod);
        }

        /// <summary>
        /// Register customer order payment in external payment system
        /// </summary>
        /// <remarks>Used in storefront checkout or manual order payment registration</remarks>
        /// <param name="orderId">customer order id</param>
        /// <param name="paymentId">payment id</param>
        /// <param name="bankCardInfo">banking card information</param>
        [HttpPost]
        [Route("process/{orderId}/{paymentId}")]
        public async Task<ActionResult<ProcessPaymentResult>> ProcessOrder(string orderId, string paymentId, [SwaggerOptional] BankCardInfo bankCardInfo)
        {
            var order = await _customerOrderService.GetByIdAsync(orderId, CustomerOrderResponseGroup.Full.ToString());

            if (order == null)
            {
                var searchCriteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();
                searchCriteria.Number = orderId;
                searchCriteria.ResponseGroup = CustomerOrderResponseGroup.Full.ToString();

                var orders = await _searchService.SearchCustomerOrdersAsync(searchCriteria);
                order = orders.Results.FirstOrDefault();
            }

            if (order == null)
            {
                throw new InvalidOperationException($"Cannot find order with ID {orderId}");
            }

            var payment = order.InPayments.FirstOrDefault(x => x.Id == paymentId);
            if (payment == null)
            {
                throw new InvalidOperationException($"Cannot find payment with ID {paymentId}");
            }

            var paymentMethodsSearchCriteria = new PaymentMethodsSearchCriteria
            {
                StoreId = order.StoreId,
                Codes = new[] { payment.GatewayCode },
                IsActive = true,
                Take = 1
            };

            var paymentMethod = (await _paymentMethodsSearchService.SearchPaymentMethodsAsync(paymentMethodsSearchCriteria))
                                .Results
                                .FirstOrDefault();

            if (paymentMethod == null)
            {
                throw new InvalidOperationException($"Cannot find payment method with code {payment.GatewayCode}");
            }

            var context = new ProcessPaymentEvaluationContext
            {
                OrderId = order.Id,
                PaymentId = payment.Id,
                //TODO
                //Store = store,
                //BankCardInfo = bankCardInfo
            };

            var result = paymentMethod.ProcessPayment(context);
            if (result.OuterId != null)
            {
                payment.OuterId = result.OuterId;
            }

            await _customerOrderService.SaveChangesAsync(new[] { order });

            return Ok(result);
        }


        /// <summary>
        /// Payment callback operation used by external payment services to inform post process payment in our system
        /// </summary>
        /// <param name="callback">payment callback parameters</param>
        [HttpPost]
        [Route("callback")]
        public async Task<ActionResult<PostProcessPaymentResult>> PostProcess([FromBody]PaymentCallbackParameters callback)
        {
            var parameters = new NameValueCollection();
            foreach (var param in callback?.Parameters ?? Array.Empty<KeyValuePair>())
            {
                parameters.Add(param.Key, param.Value);
            }
            var orderId = parameters.Get("orderid");
            if (string.IsNullOrEmpty(orderId))
            {
                throw new InvalidOperationException("the 'orderid' parameter must be passed");
            }

            //some payment method require customer number to be passed and returned. First search customer order by number
            var searchCriteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();
            searchCriteria.Number = orderId;
            searchCriteria.ResponseGroup = CustomerOrderResponseGroup.Full.ToString();
            //if order not found by order number search by order id
            var orders = await _searchService.SearchCustomerOrdersAsync(searchCriteria);
            var order = orders.Results.FirstOrDefault() ?? await _customerOrderService.GetByIdAsync(orderId, CustomerOrderResponseGroup.Full.ToString());

            if (order == null)
            {
                throw new InvalidOperationException($"Cannot find order with ID {orderId}");
            }

            var orderPaymentsCodes = order.InPayments.Select(x => x.GatewayCode).Distinct().ToArray();
            var paymentMethodCode = parameters.Get("code");

            //Need to use concrete payment method if it code passed otherwise use all order payment methods
            var paymentMethodCodes = paymentMethodCode.IsNullOrEmpty() ? orderPaymentsCodes : new[] { paymentMethodCode };

            var paymentMethodsSearchCriteria = new PaymentMethodsSearchCriteria
            {
                StoreId = order.StoreId,
                Codes = paymentMethodCodes,
                IsActive = true,
                Take = int.MaxValue
            };

            var paymentMethods = (await _paymentMethodsSearchService.SearchPaymentMethodsAsync(paymentMethodsSearchCriteria)).Results;

            foreach (var paymentMethod in paymentMethods)
            {
                //Each payment method must check that these parameters are addressed to it
                var result = paymentMethod.ValidatePostProcessRequest(parameters);
                if (result.IsSuccess)
                {
                    var paymentOuterId = result.OuterId;
                    var payment = order.InPayments.FirstOrDefault(x => string.IsNullOrEmpty(x.OuterId) || x.OuterId == paymentOuterId);
                    if (payment == null)
                    {
                        throw new InvalidOperationException(@"Cannot find payment");
                    }
                    var context = new PostProcessPaymentEvaluationContext
                    {
                        OrderId = order.Id,
                        PaymentId = payment.Id,
                        //TODO
                        //Store = store,
                        //OuterId = paymentOuterId,
                        Parameters = parameters
                    };
                    var retVal = paymentMethod.PostProcessPayment(context);
                    if (retVal != null)
                    {
                        await _customerOrderService.SaveChangesAsync(new[] { order });

                        // order Number is required
                        retVal.OrderId = order.Number;
                    }
                    return Ok(retVal);
                }
            }
            return Ok(new PostProcessPaymentResult { ErrorMessage = "Payment method not found" });
        }

        #region backward compatibility to version 2

        /// <summary>
        /// Register customer order payment in external payment system
        /// </summary>
        /// <remarks>Used in storefront checkout or manual order payment registration</remarks>
        /// <param name="orderId">customer order id</param>
        /// <param name="paymentId">payment id</param>
        /// <param name="bankCardInfo">banking card information</param>
        [Obsolete("This api is deprecated, please use 'api/payment/process' instead.")]
        [HttpPost]
        [Route("~/api/order/customerOrders/{orderId}/processPayment/{paymentId}")]
        public async Task<ActionResult<ProcessPaymentResult>> ProcessOrderPaymentsOld(string orderId, string paymentId, [SwaggerOptional] BankCardInfo bankCardInfo)
        {
            return await ProcessOrder(orderId, paymentId, bankCardInfo);
        }

        /// <summary>
        /// Payment callback operation used by external payment services to inform post process payment in our system
        /// </summary>
        /// <param name="callback">payment callback parameters</param>
        [Obsolete("This api is deprecated, please use 'api/payment/callback' instead.")]
        [HttpPost]
        [Route("~/api/paymentcallback")]
        public async Task<ActionResult<PostProcessPaymentResult>> PostProcessPaymentOld([FromBody]PaymentCallbackParameters callback)
        {
            return await PostProcess(callback);
        }

        #endregion
    }
}
