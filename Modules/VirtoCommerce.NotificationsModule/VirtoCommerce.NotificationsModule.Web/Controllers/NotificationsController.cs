using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.NotificationsModule.Web.Controllers
{
    [Route("api/[controller]")]
    public class NotificationsController : Controller
    {
        [HttpGet]
        //[ResponseType(typeof(webModels.Notification[]))]
        [Route("")]
        public IActionResult GetNotifications()
        {
            //var notifications = _notificationManager.GetNotifications();
            return Ok();
        }
    }
}
