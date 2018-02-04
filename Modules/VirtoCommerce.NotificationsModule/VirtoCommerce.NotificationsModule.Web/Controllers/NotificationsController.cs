using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.NotificationsModule.Web.ViewModels;

namespace VirtoCommerce.NotificationsModule.Web.Controllers
{


    [Authorize]
    [Route("api/notifications")]
    public class NotificationsController : Controller
    {
        #region FakeNotifications

        NotificationSearchResult _result = new NotificationSearchResult()
        {
            TotalCount = 2,
            Results = new List<NotificationResult>()
            {
                new NotificationResult()
                {
                    Id = "1",
                    DisplayName = "notifications.types.RegistrationEmailNotification.displayName",
                    Description = "notifications.types.RegistrationEmailNotification.description",
                    SendGatewayType = "Email",
                    NotificationType = "RegistrationEmailNotification",
                    IsActive = true,
                    IsSuccessSend = false,
                    AttemptCount = 0,
                    MaxAttemptCount = 10
                },
                new NotificationResult()
                {
                    Id = "2",
                    DisplayName = "notifications.types.TwoFactorEmailNotification.displayName",
                    Description = "notifications.types.TwoFactorEmailNotification.description",
                    SendGatewayType = "SMS",
                    NotificationType = "TwoFactorEmailNotification",
                    IsActive = true,
                    IsSuccessSend = false,
                    AttemptCount = 0,
                    MaxAttemptCount = 10
                }
            }
        };

        #endregion

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NotificationSearchResult), 200)]
        public IActionResult GetNotifications(NotificationsSearchCriteria searchCriteria)
        {
            //var notifications = _notificationManager.GetNotifications();
            return Ok(_result);
        }

        [HttpGet]
        [Route("{type}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NotificationResult), 200)]
        public IActionResult GetNotificationByType(string type)
        {
            var found = _result.Results.Single(n => n.NotificationType.Equals(type));
            return Ok(found);
        }

    }
}
