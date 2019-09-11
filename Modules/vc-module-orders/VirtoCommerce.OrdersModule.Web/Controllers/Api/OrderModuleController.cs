using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SelectPdf;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.OrdersModule.Core;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Notifications;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.OrdersModule.Data.Caching;
using VirtoCommerce.OrdersModule.Data.Repositories;
using VirtoCommerce.OrdersModule.Data.Services;
using VirtoCommerce.OrdersModule.Web.Authorization;
using VirtoCommerce.OrdersModule.Web.BackgroundJobs;
using VirtoCommerce.OrdersModule.Web.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using CustomerOrderSearchResult = VirtoCommerce.OrdersModule.Core.Model.Search.CustomerOrderSearchResult;

namespace VirtoCommerce.OrdersModule.Web.Controllers.Api
{
    [Route("api/order/customerOrders")]
    public class OrderModuleController : Controller
    {
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ICustomerOrderSearchService _searchService;
        private readonly IUniqueNumberGenerator _uniqueNumberGenerator;
        private readonly IStoreService _storeService;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly Func<IOrderRepository> _repositoryFactory;
        private readonly ICustomerOrderBuilder _customerOrderBuilder;
        private readonly IShoppingCartService _cartService;
        private readonly ICustomerOrderTotalsCalculator _totalsCalculator;
        private readonly INotificationSearchService _notificationSearchService;
        private readonly IAuthorizationService _authorizationService;

        private readonly INotificationTemplateRenderer _notificationTemplateRenderer;
        private readonly IChangeLogSearchService _changeLogSearchService;

        public OrderModuleController(
              ICustomerOrderService customerOrderService
            , ICustomerOrderSearchService searchService
            , IStoreService storeService
            , IUniqueNumberGenerator numberGenerator
            , IPlatformMemoryCache platformMemoryCache
            , Func<IOrderRepository> repositoryFactory
            , ICustomerOrderBuilder customerOrderBuilder
            , IShoppingCartService cartService
            , IChangeLogSearchService changeLogSearchService
            , INotificationTemplateRenderer notificationTemplateRenderer
            , INotificationSearchService notificationSearchService
            , ICustomerOrderTotalsCalculator totalsCalculator
            , IAuthorizationService authorizationService)
        {
            _customerOrderService = customerOrderService;
            _searchService = searchService;
            _uniqueNumberGenerator = numberGenerator;
            _storeService = storeService;
            _platformMemoryCache = platformMemoryCache;
            _repositoryFactory = repositoryFactory;
            _customerOrderBuilder = customerOrderBuilder;
            _cartService = cartService;
            _changeLogSearchService = changeLogSearchService;
            _notificationTemplateRenderer = notificationTemplateRenderer;
            _notificationSearchService = notificationSearchService;
            _totalsCalculator = totalsCalculator;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Search customer orders by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<CustomerOrderSearchResult>> SearchCustomerOrder([FromBody]CustomerOrderSearchCriteria criteria)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, criteria, new OrderAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }
            
            var result = await _searchService.SearchCustomerOrdersAsync(criteria);

            return Ok(result);
        }

        /// <summary>
        /// Find customer order by number
        /// </summary>
        /// <remarks>Return a single customer order with all nested documents or null if order was not found</remarks>
        /// <param name="number">customer order number</param>
        /// <param name="respGroup"></param>
        [HttpGet]
        [Route("number/{number}")]
        public async Task<ActionResult<CustomerOrder>> GetByNumber(string number, [FromRoute] string respGroup = null)
        {
            var searchCriteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();
            searchCriteria.Number = number;
            searchCriteria.ResponseGroup = respGroup;
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, searchCriteria, new OrderAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }
            var result = await _searchService.SearchCustomerOrdersAsync(searchCriteria);

            var retVal = result.Results.FirstOrDefault();      
            return Ok(retVal);
        }

        /// <summary>
        /// Find customer order by id
        /// </summary>
        /// <remarks>Return a single customer order with all nested documents or null if order was not found</remarks>
        /// <param name="id">customer order id</param>
        /// <param name="respGroup"></param>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CustomerOrder>> GetById(string id, [FromRoute] string respGroup = null)
        {
            var searchCriteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();
            searchCriteria.Ids = new[] { id };
            searchCriteria.ResponseGroup = respGroup;
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, searchCriteria, new OrderAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }
            var result = await _searchService.SearchCustomerOrdersAsync(searchCriteria);
                   
            return Ok(result.Results.FirstOrDefault());
        }

        /// <summary>
        /// Calculate order totals after changes
        /// </summary>
        /// <remarks>Return order with recalculated totals</remarks>
        /// <param name="order">Customer order</param>
        [HttpPut]
        [Route("recalculate")]
        public ActionResult<CustomerOrder> CalculateTotals([FromBody]CustomerOrder order)
        {
            _totalsCalculator.CalculateTotals(order);

            return Ok(order);

        }

        /// <summary>
        /// Register customer order payment in external payment system
        /// </summary>
        /// <remarks>Used in storefront checkout or manual order payment registration</remarks>
        /// <param name="orderId">customer order id</param>
        /// <param name="paymentId">payment id</param>
        /// <param name="bankCardInfo">banking card information</param>
        [HttpPost]
        [Route("{orderId}/processPayment/{paymentId}")]
        public async Task<ActionResult<ProcessPaymentRequestResult>> ProcessOrderPayments(string orderId, string paymentId, [SwaggerOptional] BankCardInfo bankCardInfo)
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

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, order, new OrderAuthorizationRequirement(ModuleConstants.Security.Permissions.Update));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            var inPayment = order.InPayments.FirstOrDefault(x => x.Id == paymentId);
            if (inPayment == null)
            {
                throw new InvalidOperationException($"Cannot find payment with ID {paymentId}");
            }
            if (inPayment.PaymentMethod == null)
            {
                throw new InvalidOperationException($"Cannot find payment method with code {inPayment.GatewayCode}");
            }

            var request = new ProcessPaymentRequest
            {
                OrderId = order.Id,
                Order = order,
                PaymentId = inPayment.Id,
                Payment = inPayment,
                //TODO
                //Store = store,
                BankCardInfo = bankCardInfo
            };
            var result = inPayment.PaymentMethod.ProcessPayment(request);
            if (result.OuterId != null)
            {
                inPayment.OuterId = result.OuterId;
            }

            await _customerOrderService.SaveChangesAsync(new[] { order });

            return Ok(result);
        }

        /// <summary>
        /// Create new customer order based on shopping cart.
        /// </summary>
        /// <param name="cartId">shopping cart id</param>
        [HttpPost]
        [Route("{cartId}")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<CustomerOrder>> CreateOrderFromCart(string cartId)
        {
            CustomerOrder retVal = null;

            using (await AsyncLock.GetLockByKey(cartId).LockAsync())
            {
                var cart = await _cartService.GetByIdAsync(cartId);
                retVal = await _customerOrderBuilder.PlaceCustomerOrderFromCartAsync(cart);
            }

            return Ok(retVal);
        }

        /// <summary>
        /// Add new customer order to system
        /// </summary>
        /// <param name="customerOrder">customer order</param>
        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<CustomerOrder>> CreateOrder([FromBody]CustomerOrder customerOrder)
        {
            await _customerOrderService.SaveChangesAsync(new[] { customerOrder });
            return Ok(customerOrder);
        }

        /// <summary>
        ///  Update a existing customer order 
        /// </summary>
        /// <param name="customerOrder">customer order</param>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult> UpdateOrder([FromBody]CustomerOrder customerOrder)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, customerOrder, new OrderAuthorizationRequirement(ModuleConstants.Security.Permissions.Update));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            await _customerOrderService.SaveChangesAsync(new[] { customerOrder });
            return NoContent();
        }

        /// <summary>
        /// Get new shipment for specified customer order
        /// </summary>
        /// <remarks>Return new shipment document with populates all required properties.</remarks>
        /// <param name="id">customer order id </param>
        [HttpGet]
        [Route("{id}/shipments/new")]
        public async Task<ActionResult<Shipment>> GetNewShipment(string id)
        {
            var order = await _customerOrderService.GetByIdAsync(id, CustomerOrderResponseGroup.Full.ToString());
            if (order != null)
            {
                var retVal = AbstractTypeFactory<Shipment>.TryCreateInstance();

                retVal.Id = Guid.NewGuid().ToString();
                retVal.Currency = order.Currency;
                retVal.Status = "New";

                var store = await _storeService.GetByIdAsync(order.StoreId, StoreResponseGroup.StoreInfo.ToString());
                var numberTemplate = store.Settings.GetSettingValue(
                    ModuleConstants.Settings.General.OrderShipmentNewNumberTemplate.Name,
                    ModuleConstants.Settings.General.OrderShipmentNewNumberTemplate.DefaultValue);
                retVal.Number = _uniqueNumberGenerator.GenerateNumber(numberTemplate.ToString());

                return Ok(retVal);

                ////Detect not whole shipped items
                ////TODO: LineItem partial shipping
                //var shippedLineItemIds = order.Shipments.SelectMany(x => x.Items).Select(x => x.LineItemId);

                ////TODO Add check for digital products (don't add to shipment)
                //retVal.Items = order.Items.Where(x => !shippedLineItemIds.Contains(x.Id))
                //              .Select(x => new coreModel.ShipmentItem(x)).ToList();
                //return Ok(retVal.ToWebModel());
            }

            return NotFound();
        }

        /// <summary>
        /// Get new payment for specified customer order
        /// </summary>
        /// <remarks>Return new payment  document with populates all required properties.</remarks>
        /// <param name="id">customer order id </param>
        [HttpGet]
        [Route("{id}/payments/new")]
        public async Task<ActionResult<PaymentIn>> GetNewPayment(string id)
        {
            var order = await _customerOrderService.GetByIdAsync(id, CustomerOrderResponseGroup.Full.ToString());
            if (order != null)
            {
                var retVal = AbstractTypeFactory<PaymentIn>.TryCreateInstance();
                retVal.Id = Guid.NewGuid().ToString();
                retVal.Currency = order.Currency;
                retVal.CustomerId = order.CustomerId;
                retVal.Status = retVal.PaymentStatus.ToString();

                var store = await _storeService.GetByIdAsync(order.StoreId, StoreResponseGroup.StoreInfo.ToString());
                var numberTemplate = store.Settings.GetSettingValue(
                    ModuleConstants.Settings.General.OrderPaymentInNewNumberTemplate.Name,
                    ModuleConstants.Settings.General.OrderPaymentInNewNumberTemplate.DefaultValue);
                retVal.Number = _uniqueNumberGenerator.GenerateNumber(numberTemplate.ToString());
                return Ok(retVal);
            }

            return NotFound();
        }

        /// <summary>
        ///  Delete a whole customer orders
        /// </summary>
        /// <param name="ids">customer order ids for delete</param>
        [HttpDelete]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> DeleteOrdersByIds([FromQuery] string[] ids)
        {
            await _customerOrderService.DeleteAsync(ids);
            return NoContent();
        }

        /// <summary>
        ///  Get a some order statistic information for Commerce manager dashboard
        /// </summary>
        /// <param name="start">start interval date</param>
        /// <param name="end">end interval date</param>
        [HttpGet]
        [Route("~/api/order/dashboardStatistics")]
        public async Task<ActionResult<DashboardStatisticsResult>> GetDashboardStatisticsAsync([FromRoute]DateTime? start = null, [FromRoute]DateTime? end = null)
        {
            DashboardStatisticsResult retVal;
            start = start ?? DateTime.UtcNow.AddYears(-1);
            end = end ?? DateTime.UtcNow;

            // Hack: to compinsate for incorrect Local dates to UTC
            end = end.Value.AddDays(2);
            var cacheKey = CacheKey.With(GetType(), string.Join(":", "Statistic", start.Value.ToString("yyyy-MM-dd"), end.Value.ToString("yyyy-MM-dd")));
            retVal = await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(OrderSearchCacheRegion.CreateChangeToken());
                var collectStaticJob = new CollectOrderStatisticJob(_repositoryFactory);
                var result = await collectStaticJob.CollectStatisticsAsync(start.Value, end.Value);
                return result;
            });

            return Ok(retVal);
        }

        /// <summary>
        /// Payment callback operation used by external payment services to inform post process payment in our system
        /// </summary>
        /// <param name="callback">payment callback parameters</param>
        [HttpPost]
        [Route("~/api/paymentcallback")]
        public async Task<ActionResult<PostProcessPaymentRequestResult>> PostProcessPayment([FromBody]PaymentCallbackParameters callback)
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

            var paymentMethodCode = parameters.Get("code");

            //Need to use concrete  payment method if it code passed otherwise use all order payment methods
            foreach (var inPayment in order.InPayments.Where(x => x.PaymentMethod != null && (string.IsNullOrEmpty(paymentMethodCode) || x.GatewayCode.EqualsInvariant(paymentMethodCode))))
            {
                //Each payment method must check that these parameters are addressed to it
                var result = inPayment.PaymentMethod.ValidatePostProcessRequest(parameters);
                if (result.IsSuccess)
                {

                    var request = new PostProcessPaymentRequest
                    {
                        OrderId = order.Id,
                        Order = order,
                        PaymentId = inPayment.Id,
                        Payment = inPayment,
                        StoreId = order.StoreId,
                        //TODO
                        //Store = store,
                        OuterId = result.OuterId,
                        Parameters = parameters
                    };
                    var retVal = inPayment.PaymentMethod.PostProcessPayment(request);
                    if (retVal != null)
                    {
                        await _customerOrderService.SaveChangesAsync(new[] { order });

                        // order Number is required
                        retVal.OrderId = order.Number;
                    }
                    return Ok(retVal);
                }
            }
            return Ok(new PostProcessPaymentRequestResult { ErrorMessage = "Payment method not found" });
        }

        [HttpGet]
        [Route("invoice/{orderNumber}")]
        [SwaggerFileResponse]
        public async Task<ActionResult> GetInvoicePdf(string orderNumber)
        {
            var searchCriteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();
            searchCriteria.Number = orderNumber;
            searchCriteria.Take = 1;
            //ToDo
            //searchCriteria.ResponseGroup = OrderReadPricesPermission.ApplyResponseGroupFiltering(_securityService.GetUserPermissions(User.Identity.Name), null);

            var orders = await _searchService.SearchCustomerOrdersAsync(searchCriteria);
            var order = orders.Results.FirstOrDefault();

            if (order == null)
            {
                throw new InvalidOperationException($"Cannot find order with number {orderNumber}");
            }

            var notification = await _notificationSearchService.GetNotificationAsync<InvoiceEmailNotification>(new TenantIdentity(order.StoreId, nameof(StoreModule.Core.Model.Store)));
            notification.CustomerOrder = order;
            var message = AbstractTypeFactory<NotificationMessage>.TryCreateInstance($"{notification.Kind}Message");
            message.LanguageCode = order.LanguageCode;
            notification.ToMessage(message, _notificationTemplateRenderer);

            //need to do https://selectpdf.com/html-to-pdf/docs/html/Deployment.htm
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;

            var doc = converter.ConvertHtmlString(((EmailNotificationMessage)message).Body);
            var byteArray = doc.Save();
            return new FileContentResult(byteArray, "application/pdf");
        }

        [HttpGet]
        [Route("{id}/changes")]
        public async Task<ActionResult<OperationLog[]>> GetOrderChanges(string id)
        {
            var result = Array.Empty<OperationLog>();
            var order = await _customerOrderService.GetByIdAsync(id);
            if (order != null)
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, order, new OrderAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
                if (!authorizationResult.Succeeded)
                {
                    return Unauthorized();
                }

                //Load general change log for order
                var allHasHangesObjects = order.GetFlatObjectsListWithInterface<IHasChangesHistory>()
                                          .Distinct().ToArray();

                var criteria = new ChangeLogSearchCriteria
                {
                    ObjectIds = allHasHangesObjects.Select(x => x.Id).Distinct().ToArray(),
                    ObjectTypes = allHasHangesObjects.Select(x => x.GetType().Name).Distinct().ToArray()
                };
                result = (await _changeLogSearchService.SearchAsync(criteria)).Results.ToArray();

            }
            return Ok(result);
        }

    }
}
