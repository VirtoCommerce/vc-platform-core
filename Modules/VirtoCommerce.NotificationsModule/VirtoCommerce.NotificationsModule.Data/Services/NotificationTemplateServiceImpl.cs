using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.NotificationsModule.Data.Abstractions;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationTemplateServiceImpl : INotificationTemplateService
    {
        #region FakeTemplates

        NotificationTemplateResult[] _templateResult = new NotificationTemplateResult[]
        {
            new NotificationTemplateResult
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

        public IEnumerable<NotificationTemplateResult> GetAllTemplates()
        {
            return _templateResult;
        }

        public NotificationTemplateResult GetById(string type, string notificationTemplateId)
        {
            return _templateResult.Single(t => t.NotificationType.Equals(type) && t.Id.Equals(notificationTemplateId));
        }

        public NotificationTemplateResult GetByNotification(string notificationTypeId, string objectId, string objectTypeId,
            string language)
        {
            throw new System.NotImplementedException();
        }

        public NotificationTemplateResult[] GetNotificationTemplatesByNotification(string notificationTypeId, string objectId,
            string objectTypeId)
        {
            return _templateResult.Where(t => t.NotificationType.Equals(notificationTypeId)).ToArray();
        }

        //public NotificationTemplate Create(NotificationTemplate notificationTemplate)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public void Update(NotificationTemplate[] notificationTemplates)
        //{
        //    throw new System.NotImplementedException();
        //}

        public void Delete(string[] notificationTemplateIds)
        {
            throw new System.NotImplementedException();
        }
    }
}
