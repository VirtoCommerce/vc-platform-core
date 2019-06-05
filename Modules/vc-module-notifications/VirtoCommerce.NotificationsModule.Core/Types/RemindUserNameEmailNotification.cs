using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Types
{
    public class RemindUserNameEmailNotification : EmailNotification
    {
        //need to the storefront
        public override string Type { get; set; } = "RemindUserNameEmailNotification";
    }
}
