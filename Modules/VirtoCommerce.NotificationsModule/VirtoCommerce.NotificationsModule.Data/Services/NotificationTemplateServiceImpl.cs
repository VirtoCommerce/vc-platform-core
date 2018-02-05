using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Data.Abstractions;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationTemplateServiceImpl : INotificationTemplateService
    {
        public IEnumerable<NotificationTemplate> GetAllTemplates()
        {
            throw new System.NotImplementedException();
        }

        public NotificationTemplate GetById(string notificationTemplateId)
        {
            throw new System.NotImplementedException();
        }

        public NotificationTemplate GetByNotification(string notificationTypeId, string objectId, string objectTypeId,
            string language)
        {
            throw new System.NotImplementedException();
        }

        public NotificationTemplate[] GetNotificationTemplatesByNotification(string notificationTypeId, string objectId,
            string objectTypeId)
        {
            throw new System.NotImplementedException();
        }

        public NotificationTemplate Create(NotificationTemplate notificationTemplate)
        {
            throw new System.NotImplementedException();
        }

        public void Update(NotificationTemplate[] notificationTemplates)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(string[] notificationTemplateIds)
        {
            throw new System.NotImplementedException();
        }
    }
}
