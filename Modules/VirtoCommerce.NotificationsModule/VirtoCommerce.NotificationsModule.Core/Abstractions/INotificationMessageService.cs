using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationMessageService
    {
        NotifcationMessage[] GetNotificationsMessageByIds(string[] ids);
        void SaveChanges(NotifcationMessage[] messages);
    }
}
