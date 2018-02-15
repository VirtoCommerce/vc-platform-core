using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Web.Infrastructure
{
    public class PolymorphicNotificationTemplateJsonConverter : JsonConverter
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
            if (typeof(NotificationTemplate).IsAssignableFrom(objectType))
            {
                var notificationType = objectType.Name;
                var pt = obj["kind"];
                if (pt != null)
                {
                    notificationType = pt.Value<string>();
                }
                retVal = AbstractTypeFactory<NotificationTemplate>.TryCreateInstance(notificationType);
                if (retVal == null)
                {
                    throw new NotSupportedException("Unknown kind: " + notificationType);
                }
            }
            serializer.Populate(obj.CreateReader(), retVal);


            return retVal;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(NotificationTemplate).IsAssignableFrom(objectType);
        }
    }
}
