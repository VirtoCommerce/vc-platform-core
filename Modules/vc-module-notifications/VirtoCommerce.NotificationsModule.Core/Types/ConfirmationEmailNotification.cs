using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Types
{
    public class ConfirmationEmailNotification : EmailNotification
    {
        //need to the storefront
        public override string Type { get; set; } = "EmailConfirmationNotification";
        public string Url { get; set; }
    }
}
