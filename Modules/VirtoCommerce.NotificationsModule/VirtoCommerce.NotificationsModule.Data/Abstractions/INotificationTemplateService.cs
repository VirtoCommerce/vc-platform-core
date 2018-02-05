using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Data.Abstractions
{
    public interface INotificationTemplateService
    {
        IEnumerable<NotificationTemplateResult> GetAllTemplates();
        NotificationTemplateResult GetById(string type, string notificationTemplateId);
        NotificationTemplateResult GetByNotification(string notificationTypeId, string objectId, string objectTypeId, string language);
        NotificationTemplateResult[] GetNotificationTemplatesByNotification(string notificationTypeId, string objectId, string objectTypeId);
        //NotificationTemplateResult Create(NotificationTemplate notificationTemplate);
        //void Update(NotificationTemplate[] notificationTemplates);
        void Delete(string[] notificationTemplateIds);
    }
}
