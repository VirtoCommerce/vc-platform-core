using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotificationParameterAttribute : Attribute
    {
        public NotificationParameterAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}
