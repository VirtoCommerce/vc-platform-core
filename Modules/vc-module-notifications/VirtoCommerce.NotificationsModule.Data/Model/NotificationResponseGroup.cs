using System;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    [Flags]
    public enum NotificationResponseGroup
    {
        Default = 0,
        WithTemplates = 1 << 1,
        WithAttachments = 1 << 2,
        WithRecipients = 1 << 3,
        Full = WithTemplates | WithAttachments | WithRecipients
    }
}
