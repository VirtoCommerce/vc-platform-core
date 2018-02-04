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

        #region FakeTemplates

        TemplateResult[] _templateResult = new TemplateResult[]
        {
            new TemplateResult
            {
                Id = "1",
                NotificationType = "RegistrationEmailNotification",
                Language = "en-US",
                IsDefault = false,
                Created = "2018-01-01",
                Modified = "2018-01-01",
                DisplayName = "notifications.types.RegistrationEmailNotification.displayName",
                SendGatewayType = "Email",
                CcRecipients = null,
                BccRecipients = null,
                Recipient = "a@a.com",
                Sender = "s@s.s",
                Subject = "some",
                Body = "Thank you for registration {{firstname}} {{lastname}}",
                DynamicProperties =  "{\n \"firstname\": \"Name\",\n \"lastname\": \"Last\"\n}"
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

        [HttpGet]
        [Route("{type}/templates")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TemplateResult[]), 200)]
        public IActionResult GetTemplatesByNotificationType(string type)
        {
            var templates = _templateResult.Where(t => t.NotificationType.Equals(type)).ToArray();
            return Ok(templates);
        }

        [HttpGet]
        [Route("{type}/templates/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TemplateResult), 200)]
        public IActionResult GetTemplateById(string type, string id)
        {
            var template = _templateResult.Single(t => t.NotificationType.Equals(type) && t.Id.Equals(id));
            return Ok(template);
        }
    }
}
