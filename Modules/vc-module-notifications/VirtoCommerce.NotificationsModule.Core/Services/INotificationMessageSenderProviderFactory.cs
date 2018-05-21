using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    public interface INotificationMessageSenderProviderFactory
    {
        void RegisterSenderForType<T1, T2>() where T1 : Notification where T2 : INotificationMessageSender;
        INotificationMessageSender GetSenderForNotificationType(string type);
    }
}
