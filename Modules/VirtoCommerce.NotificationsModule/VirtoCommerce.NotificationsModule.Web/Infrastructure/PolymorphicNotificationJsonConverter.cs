using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Web.Infrastructure
{
    public class PolymorphicNotificationJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;
            var obj = JObject.Load(reader);
            if (typeof(Notification).IsAssignableFrom(objectType))
            {
                var notificationType = objectType.Name;
                var pt = obj["kind"];
                if (pt != null)
                {
                    notificationType = pt.Value<string>();
                }
                retVal = AbstractTypeFactory<Notification>.TryCreateInstance(notificationType);
                if (retVal == null)
                {
                    throw new NotSupportedException("Unknown kind: " + notificationType);
                }
            }
            else if (objectType == typeof(NotificationSearchCriteria))
            {
                retVal = AbstractTypeFactory<NotificationSearchCriteria>.TryCreateInstance();
            }
            serializer.Populate(obj.CreateReader(), retVal);


            return retVal;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Notification).IsAssignableFrom(objectType) || objectType == typeof(NotificationSearchCriteria);
        }
    }
}
