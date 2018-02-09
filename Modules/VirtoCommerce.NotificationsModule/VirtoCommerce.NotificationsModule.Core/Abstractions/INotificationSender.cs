using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationSender
    {
        Task SendNotificationAsync(Notification notification, string language);
    }
}
