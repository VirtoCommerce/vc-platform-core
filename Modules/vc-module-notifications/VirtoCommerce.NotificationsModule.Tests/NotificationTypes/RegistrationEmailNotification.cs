using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Tests.NotificationTypes
{
    public class RegistrationEmailNotification : EmailNotification
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
    }
}
