using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Payment;
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
using VirtoCommerce.OrdersModule.Web.BackgroundJobs;
using VirtoCommerce.OrdersModule.Web.Model;
using VirtoCommerce.OrdersModule.Web.Security;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.StoreModule.Core.Services;

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
        private readonly IPermissionScopeRequirementService _permissionScopeService;
        private readonly ICustomerOrderBuilder _customerOrderBuilder;
        private readonly IShoppingCartService _cartService;
        private readonly INotificationSender _notificationSender;
        private readonly INotificationTemplateRenderer _notificationTemplateRenderer;
        private readonly IChangeLogService _changeLogService;
        private readonly IAuthorizationService _authorizationService;

        public OrderModuleController(ICustomerOrderService customerOrderService, ICustomerOrderSearchService searchService, IStoreService storeService
            , IUniqueNumberGenerator numberGenerator
            , IPlatformMemoryCache platformMemoryCache
            , Func<IOrderRepository> repositoryFactory
            , IPermissionScopeRequirementService permissionScopeService
            , ICustomerOrderBuilder customerOrderBuilder
            , IShoppingCartService cartService
            , INotificationSender notificationSender
            , IChangeLogService changeLogService
            , INotificationTemplateRenderer notificationTemplateRenderer
            , IAuthorizationService authorizationService)
        {
            _customerOrderService = customerOrderService;
            _searchService = searchService;
            _uniqueNumberGenerator = numberGenerator;
            _storeService = storeService;
            _platformMemoryCache = platformMemoryCache;
            _repositoryFactory = repositoryFactory;
            _permissionScopeService = permissionScopeService;
            _customerOrderBuilder = customerOrderBuilder;
            _cartService = cartService;
            _notificationSender = notificationSender;
            _changeLogService = changeLogService;
            _notificationTemplateRenderer = notificationTemplateRenderer;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Search customer orders by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<GenericSearchResult<CustomerOrder>>> Search([FromBody]CustomerOrderSearchCriteria criteria)
        {
            // Search criteria could be modified in authorization handler based on user permision scopes
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, criteria, new OrderSearchCriteriaRequirement() { DesiredPermission = ModuleConstants.Security.Permissions.Read });
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

            var result = await _searchService.SearchCustomerOrdersAsync(searchCriteria);

            var retVal = result.Results.FirstOrDefault();

            if (retVal != null)
            {
                if (!await CheckAuthorization(ModuleConstants.Security.Permissions.Read, retVal))
                {
                    return Unauthorized();
                }

                //Set scopes for UI scope bounded ACL checking
                var scopes = _permissionScopeService.GetObjectPermissionScopeStrings(retVal).ToArray();
                retVal.Scopes = scopes;
            }

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
            var retVal = await _customerOrderService.GetByIdAsync(id, respGroup);
            if (retVal == null)
            {
                return NotFound();
            }

            if (!await CheckAuthorization(ModuleConstants.Security.Permissions.Read, retVal))
            {
                return Unauthorized();
            }

            //Set scopes for UI scope bounded ACL checking
            var scopes = _permissionScopeService.GetObjectPermissionScopeStrings(retVal).ToArray();
            retVal.Scopes = scopes;

            return Ok(retVal);
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
            //Nothing to do special because all order totals will be evaluated in domain CustomerOrder properties transiently        
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
        public async Task<ActionResult<ProcessPaymentResult>> ProcessOrderPayments(string orderId, string paymentId, [SwaggerOptional] BankCardInfo bankCardInfo)
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

            var store = await _storeService.GetByIdAsync(order.StoreId);
            var paymentMethod = store.PaymentMethods.FirstOrDefault(x => x.Code == payment.GatewayCode);
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
        public async Task<ActionResult> Update([FromBody]CustomerOrder customerOrder)
        {
            if (!await CheckAuthorization(ModuleConstants.Security.Permissions.Read, customerOrder))
            {
                return Unauthorized();
            }

            await _customerOrderService.SaveChangesAsync(new[] { customerOrder });
            return Ok();
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

                var store = await _storeService.GetByIdAsync(order.StoreId);
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

                var store = await _storeService.GetByIdAsync(order.StoreId);
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
        public async Task<IActionResult> DeleteOrdersByIds([FromQuery] string[] ids)
        {
            await _customerOrderService.DeleteAsync(ids);
            return Ok();
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
        public async Task<ActionResult<PostProcessPaymentResult>> PostProcessPayment([FromBody]PaymentCallbackParameters callback)
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
            var store = await _storeService.GetByIdAsync(order.StoreId);
            var paymentMethodCode = parameters.Get("code");
            //Need to use concrete  payment method if it code passed otherwise use all order payment methods
            var paymentMethods = store.PaymentMethods.Where(x => x.IsActive)
                                                     .Where(x => orderPaymentsCodes.Contains(x.Code));
            if (!string.IsNullOrEmpty(paymentMethodCode))
            {
                paymentMethods = paymentMethods.Where(x => x.Code.EqualsInvariant(paymentMethodCode));
            }

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

        [HttpGet]
        [Route("invoice/{orderNumber}")]
        [SwaggerFileResponse]
        public async Task<IActionResult> GetInvoicePdf(string orderNumber)
        {
            var searchCriteria = AbstractTypeFactory<CustomerOrderSearchCriteria>.TryCreateInstance();
            searchCriteria.Number = orderNumber;
            searchCriteria.Take = 1;

            var orders = await _searchService.SearchCustomerOrdersAsync(searchCriteria);
            var order = orders.Results.FirstOrDefault();

            if (order == null)
            {
                throw new InvalidOperationException($"Cannot find order with number {orderNumber}");
            }

            var notification = new InvoiceEmailNotification { CustomerOrder = order };
            await _notificationSender.SendNotificationAsync(notification, order.LanguageCode);
            var message = AbstractTypeFactory<NotificationMessage>.TryCreateInstance($"{notification.Kind}Message");
            message.LanguageCode = order.LanguageCode;
            var emailNotificationMessage = (EmailNotificationMessage)notification.ToMessage(message, _notificationTemplateRenderer);

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = { ColorMode = ColorMode.Color, Orientation = Orientation.Landscape, PaperSize = PaperKind.A4Plus },
                Objects = { new ObjectSettings { PagesCount = true, HtmlContent = emailNotificationMessage.Body } }
            };
            var converter = new SynchronizedConverter(new PdfTools());
            byte[] byteArray = converter.Convert(pdf);
            Stream stream = new MemoryStream(byteArray);

            return new FileStreamResult(stream, "application/pdf");
        }


        [HttpGet]
        [Route("{id}/changes")]
        public async Task<ActionResult<OperationLog[]>> GetOrderChanges(string id)
        {
            var result = new OperationLog[] { };
            var order = await _customerOrderService.GetByIdAsync(id);
            if (order != null)
            {
                _changeLogService.LoadChangeLogs(order);
                //Load general change log for order
                result = order.GetFlatObjectsListWithInterface<IHasChangesHistory>()
                                          .Distinct()
                                          .SelectMany(x => x.OperationsLog)
                                          .OrderBy(x => x.CreatedDate)
                                          .Distinct().ToArray();
            }
            return Ok(result);
        }

        /// <summary>
        /// Checks if current user has specific permission to specific order (regular and scoped permission are checked).
        /// </summary>
        /// <param name="permission">Required permission.</param>
        /// <param name="order">Cutomer order to check permissions for.</param>
        /// <returns>True if user has specified permission to specific order, otherwise false.</returns>
        private async Task<bool> CheckAuthorization(string permission, CustomerOrder order)
        {
            var result = false;
            var requirementsToCheck = new PermissionScopeRequirement[]
            {
                new OrderStoreRequirement() { DesiredPermission = permission },
                new OrderResponsibleRequirement() { DesiredPermission = permission },
            };
            foreach (var requirement in requirementsToCheck)
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, order, requirement);
                if (authorizationResult.Succeeded)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
