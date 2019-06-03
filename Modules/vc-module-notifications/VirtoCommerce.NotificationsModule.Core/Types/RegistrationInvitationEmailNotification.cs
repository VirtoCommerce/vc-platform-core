using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Types
{
    public class RegistrationInvitationEmailNotification : EmailNotification
    {
        public override string Type { get; set; } = "RegistrationInvitationNotification";
        public string InviteUrl { get; set; }
        public string Message { get; set; }
    }
}
