using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationMessageService
    {
        NotificationMessage[] GetNotificationsMessageByIds(string[] ids);
        void SaveNotificationMessages(NotificationMessage[] messages);
    }
}
