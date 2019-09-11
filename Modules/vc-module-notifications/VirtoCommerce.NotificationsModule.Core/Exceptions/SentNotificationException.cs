using System;
using VirtoCommerce.Platform.Core.Exceptions;

namespace VirtoCommerce.NotificationsModule.Core.Exceptions
{
    public class SentNotificationException : PlatformException
    {
        public SentNotificationException(Exception ex) : base(ex.ToString(), ex.InnerException)
        {
        }

        public SentNotificationException(string message) : base(message)
        {

        }
    }
}
