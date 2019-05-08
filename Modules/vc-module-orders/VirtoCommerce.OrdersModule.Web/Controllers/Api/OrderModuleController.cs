using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Common;
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
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
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
        private readonly ICustomerOrderBuilder _customerOrderBuilder;
        private readonly IShoppingCartService _cartService;
        private readonly INotificationSender _notificationSender;

        private readonly INotificationTemplateRenderer _notificationTemplateRenderer;
        private readonly IChangeLogService _changeLogService;

        public OrderModuleController(ICustomerOrderService customerOrderService, ICustomerOrderSearchService searchService, IStoreService storeService
            , IUniqueNumberGenerator numberGenerator
            , IPlatformMemoryCache platformMemoryCache
            , Func<IOrderRepository> repositoryFactory
            , ICustomerOrderBuilder customerOrderBuilder
            , IShoppingCartService cartService
            , INotificationSender notificationSender
            , IChangeLogService changeLogService, INotificationTemplateRenderer notificationTemplateRenderer)
        {
            _customerOrderService = customerOrderService;
            _searchService = searchService;
            _uniqueNumberGenerator = numberGenerator;
            _storeService = storeService;
            _platformMemoryCache = platformMemoryCache;
            _repositoryFactory = repositoryFactory;
            //_securityService = securityService;
            //_permissionScopeService = permissionScopeService;
            _customerOrderBuilder = customerOrderBuilder;
            _cartService = cartService;
            _notificationSender = notificationSender;
            _changeLogService = changeLogService;
            _notificationTemplateRenderer = notificationTemplateRenderer;
        }

        /// <summary>
        /// Search customer orders by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<GenericSearchResult<CustomerOrder>>> Search([FromBody]CustomerOrderSearchCriteria criteria)
        {
            //Scope bound ACL filtration
            criteria = FilterOrderSearchCriteria(User.Identity.Name, criteria);

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

            //TODO Resource based auth
            //if (retVal != null)
            //{
            //    var scopes = _permissionScopeService.GetObjectPermissionScopeStrings(retVal).ToArray();
            //    if (!_securityService.UserHasAnyPermission(User.Identity.Name, scopes, ModuleConstants.Security.Permissions.Read))
            //    {
            //        return Unauthorized();
            //    }
            //    //Set scopes for UI scope bounded ACL checking
            //    retVal.Scopes = scopes;
            //}
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

            //TODO
            ////Scope bound security check
            //var scopes = _permissionScopeService.GetObjectPermissionScopeStrings(retVal).ToArray();
            //if (!_securityService.UserHasAnyPermission(User.Identity.Name, scopes, OrderPredefinedPermissions.Read))
            //{
            //    throw new HttpResponseException(HttpStatusCode.Unauthorized);
            //}

            ////Set scopes for UI scope bounded ACL checking
            //retVal.Scopes = scopes;

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
            //Check scope bound permission
            //var scopes = _permissionScopeService.GetObjectPermissionScopeStrings(customerOrder).ToArray();

            //TODO
            //if (!_securityService.UserHasAnyPermission(User.Identity.Name, scopes, OrderPredefinedPermissions.Read))
            //{
            //    return Unauthorized();
            //}

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
            var byteArray = converter.Convert(pdf);
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

        private CustomerOrderSearchCriteria FilterOrderSearchCriteria(string userName, CustomerOrderSearchCriteria criteria)
        {
            //TODO
            //if (!_securityService.UserHasAnyPermission(userName, null, OrderPredefinedPermissions.Read))
            //{
            //    //Get defined user 'read' permission scopes
            //    var readPermissionScopes = _securityService.GetUserPermissions(userName)
            //        .Where(x => x.Id.StartsWith(OrderPredefinedPermissions.Read))
            //        .SelectMany(x => x.AssignedScopes)
            //        .ToList();

            //    //Check user has a scopes
            //    //Stores
            //    criteria.StoreIds = readPermissionScopes.OfType<OrderStoreScope>()
            //        .Select(x => x.Scope)
            //        .Where(x => !string.IsNullOrEmpty(x))
            //        .ToArray();

            //    var responsibleScope = readPermissionScopes.OfType<OrderResponsibleScope>().FirstOrDefault();
            //    //employee id
            //    if (responsibleScope != null)
            //    {
            //        criteria.EmployeeId = userName;
            //    }
            //}
            return criteria;
        }
    }
}
