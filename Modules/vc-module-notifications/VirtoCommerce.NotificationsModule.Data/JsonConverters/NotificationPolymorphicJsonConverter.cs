using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.JsonConverters
{
    public class NotificationPolymorphicJsonConverter : JsonConverter
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
            object result = null;
            var obj = JObject.Load(reader);
            if (objectType == typeof(Notification))
            {
                var type = obj["type"].Value<string>();
                result = AbstractTypeFactory<Notification>.TryCreateInstance(type);
            }
            else if (objectType == typeof(NotificationTemplate))
            {
                var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods()
                    .FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && x.GetParameters().Length == 0);
                result = tryCreateInstance?.Invoke(null, null);
            }
            else if (objectType == typeof(NotificationSearchCriteria))
            {
                result = AbstractTypeFactory<NotificationSearchCriteria>.TryCreateInstance();
            }
            serializer.Populate(obj.CreateReader(), result);


            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }
    }
}
