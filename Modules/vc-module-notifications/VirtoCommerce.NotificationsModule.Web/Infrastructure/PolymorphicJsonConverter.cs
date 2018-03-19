using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Web.Infrastructure
{
    public class PolymorphicJsonConverter : JsonConverter
    {
        private static readonly Type[] _knowTypes = { typeof(Notification), typeof(NotificationTemplate), typeof(NotificationSearchCriteria) };

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
            if (objectType == typeof(Notification) || objectType == typeof(NotificationTemplate))
            {
                var notificationType = objectType.Name;
                var pt = obj["type"];
                if (pt != null)
                {
                    notificationType = pt.Value<string>();
                }
                if (pt == null)
                {
                    pt = obj["kind"];
                    if (pt != null) notificationType = pt.Value<string>();
                }
                
                if (objectType == typeof(Notification))
                {
                    retVal = AbstractTypeFactory<Notification>.TryCreateInstance(notificationType);
                }
                else if (objectType == typeof(NotificationTemplate))
                {
                    retVal = AbstractTypeFactory<NotificationTemplate>.TryCreateInstance(notificationType);
                }
                else
                {
                    var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods()
                        .FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && x.GetParameters().Length == 0);
                    retVal = tryCreateInstance?.Invoke(null, null);
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
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }
    }
}
