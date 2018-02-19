using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Web.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Web.Controllers
{
    //[Authorize]
    [Route("api/notifications")]
    public class NotificationsController : Controller
    {
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationSearchService notificationSearchService, INotificationService notificationService)
        {
            _notificationSearchService = notificationSearchService;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GenericSearchResult<Notification>), 200)]
        public async Task<IActionResult> GetNotifications(NotificationSearchCriteria searchCriteria)
        {
            var notifications = await _notificationSearchService.SearchNotificationsAsync(searchCriteria);

            return Ok(notifications);
        }

        [HttpGet]
        [Route("{type}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Notification), 200)]
        public async Task<IActionResult> GetNotificationByTypeId(string type)
        {
            var notification = await _notificationService.GetNotificationByTypeAsync(type);

            return Ok(notification);
        }

        [HttpPut]
        [Route("{type}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> UpdateNotification([FromBody]Notification notification)
        {
            await _notificationService.SaveChangesAsync(new [] {notification});

            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}
