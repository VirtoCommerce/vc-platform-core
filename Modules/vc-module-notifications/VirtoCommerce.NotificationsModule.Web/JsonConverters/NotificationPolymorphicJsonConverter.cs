using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Web.JsonConverters
{
    public class NotificationPolymorphicJsonConverter : JsonConverter
    {
        private static readonly Type[] _knowTypes = { typeof(Notification), typeof(NotificationTemplate), typeof(NotificationMessage), typeof(NotificationSearchCriteria) };

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
                var type = obj.GetValue("type", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                result = AbstractTypeFactory<Notification>.TryCreateInstance(type);
            }
            else if (objectType.IsAssignableFrom(typeof(NotificationTemplate)))
            {
                var kind = obj.GetValue("kind", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                result = AbstractTypeFactory<NotificationTemplate>.TryCreateInstance($"{kind}Template");
            }
            else if (objectType.IsAssignableFrom(typeof(NotificationMessage)))
            {
                var kind = obj.GetValue("kind", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                result = AbstractTypeFactory<NotificationMessage>.TryCreateInstance($"{kind}Message");
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
