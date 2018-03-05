using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Web.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Web.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/notifications")]
    public class NotificationsController : Controller
    {
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationService _notificationService;
        private readonly INotificationTemplateRender _notificationTemplateRender;

        public NotificationsController(INotificationSearchService notificationSearchService
            , INotificationService notificationService
            , INotificationTemplateRender notificationTemplateRender
            )
        {
            _notificationSearchService = notificationSearchService;
            _notificationService = notificationService;
            _notificationTemplateRender = notificationTemplateRender;
        }

        [HttpPost]
        [ProducesResponseType(typeof(GenericSearchResult<Notification>), 200)]
        public IActionResult GetNotifications(NotificationSearchCriteria searchCriteria)
        {
            var notifications = _notificationSearchService.SearchNotifications(searchCriteria);

            return Ok(notifications);
        }

        [HttpGet]
        [Route("{type}")]
        [ProducesResponseType(typeof(Notification), 200)]
        public async Task<IActionResult> GetNotificationByTypeId(string type, string tenantId = null, string tenantType = null)
        {
            var notification = await _notificationService.GetNotificationByTypeAsync(type, tenantId, tenantType);

            return Ok(notification);
        }

        [HttpPut]
        [Route("{type}")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> UpdateNotification([FromBody]Notification notification)
        {
            await _notificationService.SaveChangesAsync(new [] {notification});

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("template-render")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult RenderingTemplate([FromBody]NotificationTemplateRequest request)
        {
            var result = _notificationTemplateRender.Render(request.Text, request.Data);

            return Ok(new { html = result });
        }
    }
}
