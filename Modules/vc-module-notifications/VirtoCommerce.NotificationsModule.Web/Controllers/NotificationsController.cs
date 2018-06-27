using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.NotificationsModule.Core;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Web.Infrastructure;
using VirtoCommerce.NotificationsModule.Web.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/notifications")]
    public class NotificationsController : Controller
    {
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationService _notificationService;
        private readonly INotificationTemplateRenderer _notificationTemplateRender;

        public NotificationsController(INotificationSearchService notificationSearchService
            , INotificationService notificationService
            , INotificationTemplateRenderer notificationTemplateRender
            )
        {
            _notificationSearchService = notificationSearchService;
            _notificationService = notificationService;
            _notificationTemplateRender = notificationTemplateRender;
        }

        /// <summary>
        /// Get all registered notification types by criteria
        /// </summary>
        /// <param name="searchCriteria">criteria for search(keyword, skip, take and etc.)</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(GenericSearchResult<Notification>), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public IActionResult GetNotifications(NotificationSearchCriteria searchCriteria)
        {
            var notifications = _notificationSearchService.SearchNotifications(searchCriteria);

            return Ok(notifications);
        }

        /// <summary>
        /// Get notification by type
        /// </summary>
        /// <param name="type">Notification type of template</param>
        /// <param name="tenantId">Tenant id of template</param>
        /// <param name="tenantType">Tenant type id of template</param>
        /// <remarks>
        /// Get all notification templates by notification type, tenantId, teantTypeId. Tenant id and tenant type id - params of tenant, that initialize creating of
        /// template. By default tenant id and tenant type id = "Platform". For example for store with id = "SampleStore", tenantId = "SampleStore", tenantType = "Store".
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("{type}")]
        [ProducesResponseType(typeof(Notification), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Access)]
        public async Task<IActionResult> GetNotificationByTypeId(string type, string tenantId = null, string tenantType = null)
        {
            var responseGroup = NotificationResponseGroup.Full.ToString();
            var notification = await _notificationService.GetByTypeAsync(type, tenantId, tenantType, responseGroup);

            return Ok(notification);
        }

        /// <summary>
        /// Update notification with templates
        /// </summary>
        /// <param name="notification">Notification</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{type}")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdateNotification([FromBody]Notification notification)
        {
            await _notificationService.SaveChangesAsync(new [] {notification});

            return StatusCode((int)HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Render content
        /// </summary>
        /// <param name="request">request of Notification Template with text and data</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{type}/templates/{language}/rendercontent")]
        [ProducesResponseType(typeof(string), 200)]
        [Authorize(ModuleConstants.Security.Permissions.ReadTemplates)]
        public IActionResult RenderingTemplate([FromBody]NotificationTemplateRequest request)
        {
            var result = _notificationTemplateRender.Render(request.Text, request.Data);

            return Ok(new { html = result });
        }
    }
}
