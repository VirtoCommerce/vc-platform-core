using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VirtoCommerce.NotificationsModule.Core;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Model.Search;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
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
        public async Task<ActionResult<NotificationSearchResult>> GetNotifications(NotificationSearchCriteria searchCriteria)
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
            var responseGroup = NotificationResponseGroup.Full.ToString();
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

            return StatusCode((int)HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Render content
        /// </summary>
        /// <param name="request">request of Notification Template with text and data</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{type}/templates/{language}/rendercontent")]
        [Authorize(ModuleConstants.Security.Permissions.ReadTemplates)]
        public ActionResult RenderingTemplate([FromBody]NotificationTemplateRequest request)
        {
            var result = _notificationTemplateRender.Render(request.Text, request.Data);

            return Ok(new { html = result });
        }

        /// <summary>
        /// Sending test notification
        /// </summary>
        /// <remarks>
        /// Method sending notification, that based on notification template. Template for rendering chosen by type, objectId, objectTypeId, language.
        /// Parameters for template may be prepared by the method of getTestingParameters. Method returns string. If sending finished with success status
        /// this string is empty, otherwise string contains error message.
        /// </remarks>
        /// <param name="request">Notification request</param>
        [HttpPost]
        [Route("template/sendnotification")]
        public async Task<ActionResult<NotificationSendResult>> SendNotification([FromBody]NotificationRequest request)
        {
            var notification = await _notificationService.GetByTypeAsync(request.Type);

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
        /// <param name="start">Page setting start</param>
        /// <param name="count">Page setting count</param>
        /// <param name="sort">Sort expression</param>
        [HttpGet]
        [Route("journal")]
        public async Task<ActionResult<NotificationMessageSearchResult>> GetNotificationJournal([FromQuery]int start, [FromQuery]int count, [FromQuery]string sort)
        {
            var searchCriteria = AbstractTypeFactory<NotificationMessageSearchCriteria>.TryCreateInstance();
            searchCriteria.Skip = start;
            searchCriteria.Take = count;
            searchCriteria.Sort = sort;

            var result = await _notificationMessageSearchService.SearchMessageAsync(searchCriteria);

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
                SetValue(notification, parameter);
            }
        }

        private void SetValue(Notification notification, NotificationParameter param)
        {
            var property = notification.GetType().GetProperty(ConvertPropertyName(param.ParameterName));
            if (property == null) return;

            var jObject = param.Value as JObject;
            var jArray = param.Value as JArray;
            if (jObject != null && param.IsDictionary)
            {
                property.SetValue(notification, jObject.ToObject<Dictionary<string, string>>());
            }
            else
            {
                switch (param.Type)
                {
                    case NotificationParameterValueType.Boolean:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<Boolean[]>());
                        else
                            property.SetValue(notification, param.Value.ToNullable<Boolean>());
                        break;

                    case NotificationParameterValueType.DateTime:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<DateTime[]>());
                        else
                            property.SetValue(notification, param.Value.ToNullable<DateTime>());
                        break;

                    case NotificationParameterValueType.Decimal:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<Decimal[]>());
                        else
                            property.SetValue(notification, Convert.ToDecimal(param.Value));
                        break;

                    case NotificationParameterValueType.Integer:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<Int32[]>());
                        else
                            property.SetValue(notification, param.Value.ToNullable<Int32>());
                        break;

                    case NotificationParameterValueType.String:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<String[]>());
                        else
                            property.SetValue(notification, (string)param.Value);
                        break;

                    default:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<String[]>());
                        else
                            property.SetValue(notification, (string)param.Value);
                        break;
                }
            }
        }

        private string ConvertPropertyName(string propertyName)
        {
            return propertyName
                .Replace("Sender", "From")
                .Replace("Recipient", "To");
        }
    }
}
