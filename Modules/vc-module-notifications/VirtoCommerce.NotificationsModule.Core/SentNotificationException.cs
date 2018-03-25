using System;

namespace VirtoCommerce.NotificationsModule.Core
{
    public class SentNotificationException : ApplicationException
    {
        public SentNotificationException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
