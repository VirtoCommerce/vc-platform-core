using System;

namespace VirtoCommerce.SubscriptionModule.Data.Exceptions
{
    public class CreateSubscriptionException : Exception
    {
        public CreateSubscriptionException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }
    }
}
