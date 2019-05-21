using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.JsonConverters
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
            object result = null;
            var obj = JObject.Load(reader);
            if (objectType == typeof(Notification))
            {
                var type = obj["type"].Value<string>();
                result = AbstractTypeFactory<Notification>.TryCreateInstance(type);
                PopulateNotification(obj, result);
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

        private void PopulateNotification(JObject obj, object @object)
        {
            if (@object is Notification notification)
            {
                string tenantId = string.Empty;
                if (obj.TryGetValue("objectId", StringComparison.InvariantCultureIgnoreCase, out var tenantIdToken))
                {
                    tenantId = tenantIdToken.Value<string>();
                }

                string tenantTypeId = string.Empty;
                if (obj.TryGetValue("objectTypeId", StringComparison.InvariantCultureIgnoreCase, out var tenantTypeIdToken))
                {
                    tenantTypeId = tenantTypeIdToken.Value<string>();
                }

                notification.TenantIdentity = new TenantIdentity(tenantId, tenantTypeId);
                notification.Id = obj["type"].Value<string>();

                if (obj.ContainsKey("notificationParameters"))
                {
                    var parameters = obj["notificationParameters"].Values<NotificationParameter>();
                    foreach (var parameter in parameters)
                    {
                        SetValue(notification, parameter);
                    }
                }

            }
        }

        private void SetValue(Notification notification, NotificationParameter param)
        {
            var property = notification.GetType().GetProperty(param.ParameterName);
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
    }


    class NotificationParameter
    {
        public string ParameterName { get; set; }
        public bool IsDictionary { get; set; }
        public bool IsArray { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public NotificationParameterValueType Type { get; set; }
        public object Value { get; set; }
    }

    public enum NotificationParameterValueType
    {
        String,
        Integer,
        Decimal,
        DateTime,
        Boolean
    }
}
