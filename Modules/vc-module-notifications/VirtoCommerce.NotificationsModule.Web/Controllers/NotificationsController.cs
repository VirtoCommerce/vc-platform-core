using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Security;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Web.Infrastructure;
using VirtoCommerce.NotificationsModule.Web.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Web.Controllers
{
    [Authorize]
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

        [HttpPost]
        [ProducesResponseType(typeof(GenericSearchResult<Notification>), 200)]
        //[Authorize(SecurityConstants.Permissions.Read)]
        public IActionResult GetNotifications(NotificationSearchCriteria searchCriteria)
        {
            var notifications = _notificationSearchService.SearchNotifications(searchCriteria);

            return Ok(notifications);
        }

        [HttpGet]
        [Route("{type}")]
        [ProducesResponseType(typeof(Notification), 200)]
        //[Authorize(SecurityConstants.Permissions.Access)]
        public async Task<IActionResult> GetNotificationByTypeId(string type, string tenantId = null, string tenantType = null)
        {
            var responseGroup = NotificationResponseGroup.Full.ToString();
            var notification = await _notificationService.GetByTypeAsync(type, tenantId, tenantType, responseGroup);

            return Ok(notification);
        }

        [HttpPut]
        [Route("{type}")]
        [ProducesResponseType(typeof(void), 200)]
        //[Authorize(SecurityConstants.Permissions.Update)]
        public async Task<IActionResult> UpdateNotification([FromBody]Notification notification)
        {
            await _notificationService.SaveChangesAsync(new [] {notification});

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("{type}/templates/{language}/rendercontent")]
        [ProducesResponseType(typeof(string), 200)]
        //[Authorize(SecurityConstants.Permissions.ReadTemplates)]
        public IActionResult RenderingTemplate([FromBody]NotificationTemplateRequest request)
        {
            var result = _notificationTemplateRender.Render(request.Text, request.Data);

            return Ok(new { html = result });
        }
    }
}
