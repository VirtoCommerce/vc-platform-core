using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Web.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Web.Extensions
{
    public static class NotificationExtension
    {
        public static void SetValue(this Notification notification, NotificationParameter param)
        {
            var property = notification.GetType().GetProperty(ConvertPropertyName(param.ParameterName));
            if (property == null) return;

            var jObject = param.Value as JObject;
            var jArray = param.Value as JArray;
            if (jObject != null && param.IsDictionary)
            {
                property.SetValue(notification, jObject.ToObject<Dictionary<string, string>>());
            }
            else
            {
                switch (param.Type)
                {
                    case NotificationParameterValueType.Boolean:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<Boolean[]>());
                        else
                            property.SetValue(notification, param.Value.ToNullable<Boolean>());
                        break;

                    case NotificationParameterValueType.DateTime:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<DateTime[]>());
                        else
                            property.SetValue(notification, param.Value.ToNullable<DateTime>());
                        break;

                    case NotificationParameterValueType.Decimal:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<Decimal[]>());
                        else
                            property.SetValue(notification, Convert.ToDecimal(param.Value));
                        break;

                    case NotificationParameterValueType.Integer:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<Int32[]>());
                        else
                            property.SetValue(notification, param.Value.ToNullable<Int32>());
                        break;

                    case NotificationParameterValueType.String:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<String[]>());
                        else
                            property.SetValue(notification, (string)param.Value);
                        break;

                    default:
                        if (jArray != null && param.IsArray)
                            property.SetValue(notification, jArray.ToObject<String[]>());
                        else
                            property.SetValue(notification, (string)param.Value);
                        break;
                }
            }
        }

        private static string ConvertPropertyName(string propertyName)
        {
            return propertyName
                .Replace("Sender", "From")
                .Replace("Recipient", "To");
        }
    }
}
