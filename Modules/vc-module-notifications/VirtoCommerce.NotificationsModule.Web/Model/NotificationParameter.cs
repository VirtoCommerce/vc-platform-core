using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VirtoCommerce.NotificationsModule.Web.Model
{
    public class NotificationParameter
    {
        [JsonProperty(PropertyName = "parameterName")]
        public string ParameterName { get; set; }
        [JsonProperty(PropertyName = "parameterDescription")]
        public string ParameterDescription { get; set; }
        [JsonProperty(PropertyName = "parameterCodeInView")]
        public string ParameterCodeInView { get; set; }
        [JsonProperty(PropertyName = "isDictionary")]
        public bool IsDictionary { get; set; }
        [JsonProperty(PropertyName = "isArray")]
        public bool IsArray { get; set; }
        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public NotificationParameterValueType Type { get; set; }
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }
    }
}
