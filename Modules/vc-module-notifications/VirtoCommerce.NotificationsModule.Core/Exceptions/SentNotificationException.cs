using System;
using VirtoCommerce.Platform.Core.Exceptions;

namespace VirtoCommerce.NotificationsModule.Core.Exceptions
{
    public class SentNotificationException : PlatformException
    {
        public SentNotificationException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
