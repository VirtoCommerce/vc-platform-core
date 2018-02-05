using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Abstractions
{
    public interface INotificationTemplateService
    {
        IEnumerable<NotificationTemplate> GetAllTemplates();
        NotificationTemplate GetById(string notificationTemplateId);
        NotificationTemplate GetByNotification(string notificationTypeId, string objectId, string objectTypeId, string language);
        NotificationTemplate[] GetNotificationTemplatesByNotification(string notificationTypeId, string objectId, string objectTypeId);
        NotificationTemplate Create(NotificationTemplate notificationTemplate);
        void Update(NotificationTemplate[] notificationTemplates);
        void Delete(string[] notificationTemplateIds);
    }
}
