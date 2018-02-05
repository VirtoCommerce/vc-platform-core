using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Abstractions;
using VirtoCommerce.NotificationsModule.Web.ViewModels;

namespace VirtoCommerce.NotificationsModule.Web.Controllers
{


    [Authorize]
    [Route("api/notifications")]
    public class NotificationsController : Controller
    {
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

        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GenericSearchResult<NotificationResult>), 200)]
        public IActionResult GetNotifications(NotificationsSearchCriteria searchCriteria)
        {
            var notifications = _notificationService.GetNotifications();
            return Ok(notifications);
        }

        [HttpGet]
        [Route("{type}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(NotificationResult), 200)]
        public IActionResult GetNotificationByType(string type)
        {
            var notification = _notificationService.GetNotificationByTypeId(type);
            return Ok(notification);
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
