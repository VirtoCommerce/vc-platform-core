using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.NotificationsModule.Core;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Model.Search;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Web.Extensions;
using VirtoCommerce.NotificationsModule.Web.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Web.Controllers
{
    [Route("api/notifications")]
    [Route("api/platform/notification")]
    //[Authorize(ModuleConstants.Security.Permissions.Access)]
    public class NotificationsController : Controller
    {
        private readonly INotificationSearchService _notificationSearchService;
        private readonly INotificationService _notificationService;
        private readonly INotificationTemplateRenderer _notificationTemplateRender;
        private readonly INotificationSender _notificationSender;
        private readonly INotificationMessageSearchService _notificationMessageSearchService;
        private readonly INotificationMessageService _notificationMessageService;

        public NotificationsController(INotificationSearchService notificationSearchService
            , INotificationService notificationService
            , INotificationTemplateRenderer notificationTemplateRender
            , INotificationSender notificationSender
            , INotificationMessageSearchService notificationMessageSearchService, INotificationMessageService notificationMessageService)
        {
            _notificationSearchService = notificationSearchService;
            _notificationService = notificationService;
            _notificationTemplateRender = notificationTemplateRender;
            _notificationSender = notificationSender;
            _notificationMessageSearchService = notificationMessageSearchService;
            _notificationMessageService = notificationMessageService;
        }

        /// <summary>
        /// Get all registered notification types by criteria
        /// </summary>
        /// <param name="searchCriteria">criteria for search(keyword, skip, take and etc.)</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<NotificationSearchResult>> GetNotifications([FromBody]NotificationSearchCriteria searchCriteria)
        {
            var notifications = await _notificationSearchService.SearchNotificationsAsync(searchCriteria);

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
        [Authorize(ModuleConstants.Security.Permissions.Access)]
        public async Task<ActionResult<Notification>> GetNotificationByTypeId(string type, string tenantId = null, string tenantType = null)
        {
            var notification = await _notificationSearchService.GetNotificationAsync(type, new TenantIdentity(tenantId, tenantType));

            return Ok(notification);
        }

        /// <summary>
        /// Update notification with templates
        /// </summary>
        /// <param name="notification">Notification</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{type}")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult> UpdateNotification([FromBody]Notification notification)
        {
            await _notificationService.SaveChangesAsync(new[] { notification });

            return NoContent();
        }


        /// <summary>
        /// Render content
        /// </summary>
        /// <param name="language"></param>
        /// <param name="request">request of Notification Template with text and data</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{type}/templates/{language}/rendercontent")]
        [Authorize(ModuleConstants.Security.Permissions.ReadTemplates)]
        public async Task<ActionResult> RenderingTemplate([FromBody]NotificationTemplateRequest request, string language = null)
        {
            var result = await _notificationTemplateRender.RenderAsync(request.Text, request.Data, language);

            return Ok(new { html = result });
        }

        /// <summary>
        /// Sending notification
        /// </summary>
        [HttpPost]
        [Route("send")]
        public async Task<ActionResult<NotificationSendResult>> SendNotification([FromBody]Notification notificationRequest, string language)
        {
            var notification = await _notificationSearchService.GetNotificationAsync(notificationRequest.Type, notificationRequest.TenantIdentity);
            var result = await _notificationSender.SendNotificationAsync(notification, language);

            return Ok(result);
        }

        /// <summary>
        /// Sending notification
        /// </summary>
        /// <remarks>
        /// Method sending notification, that based on notification template. Template for rendering chosen by type, objectId, objectTypeId, language.
        /// Parameters for template may be prepared by the method of getTestingParameters. Method returns string. If sending finished with success status
        /// this string is empty, otherwise string contains error message.
        /// </remarks>
        /// <param name="request">Notification request</param>
        [HttpPost]
        [Route("template/sendnotification")]
        [Obsolete("for backward compatibility")]
        public async Task<ActionResult<NotificationSendResult>> SendNotificationByRequest([FromBody]NotificationRequest request)
        {
            var notification = await _notificationSearchService.GetNotificationAsync(request.Type, new TenantIdentity(request.ObjectId, request.ObjectTypeId));

            if (notification == null)
            {
                return new NotificationSendResult { ErrorMessage = $"{request.Type} isn't registered", IsSuccess = false };
            }

            PopulateNotification(request, notification);
            var result = await _notificationSender.SendNotificationAsync(notification, request.Language);

            return Ok(result);
        }

        /// <summary>
        /// Get all notification journal 
        /// </summary>
        /// <remarks>
        /// Method returns notification journal page with array of notification, that was send, sending or will be send in future. Result contains total count, that can be used
        /// for paging.
        /// </remarks>
        /// <param name="criteria"></param>
        [HttpPost]
        [Route("journal")]
        public async Task<ActionResult<NotificationMessageSearchResult>> GetNotificationJournal([FromBody]NotificationMessageSearchCriteria criteria)
        {
            var result = await _notificationMessageSearchService.SearchMessageAsync(criteria);

            return Ok(result);
        }

        [HttpGet]
        [Route("journal/{id}")]
        public async Task<ActionResult<NotificationMessage>> GetObjectNotificationJournal(string id)
        {
            var result = (await _notificationMessageService.GetNotificationsMessageByIds(new[] { id })).FirstOrDefault();

            return Ok(result);
        }


        private void PopulateNotification(NotificationRequest request, Notification notification)
        {
            notification.TenantIdentity = new TenantIdentity(request.ObjectId, request.ObjectTypeId);

            foreach (var parameter in request.NotificationParameters)
            {
                notification.SetValue(parameter);
            }
        }
    }
}
