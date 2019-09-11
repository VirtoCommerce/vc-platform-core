using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.ExportModule.JsonProvider
{
    public class ObjectDiscriminatorJsonConverter : JsonConverter
    {
        private readonly Type[] _types;
        private readonly JsonSerializer _jsonSerializer;

        public override bool CanRead => false;
        public override bool CanWrite => true;

        public ObjectDiscriminatorJsonConverter(JsonSerializerSettings jsonSerializerSettings, params Type[] types)
        {
            _types = types;
            _jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Here we use another serializer with the same settings to avoid infinity cycle
            var jToken = JToken.FromObject(value, _jsonSerializer);

            if (jToken.Type == JTokenType.Object)
            {
                var jObject = (JObject)jToken;

                jObject.AddFirst(new JProperty("$discriminator", value.GetType().Name));
                jObject.WriteTo(writer);
            }
            else
            {
                jToken.WriteTo(writer);
            }
        }
    }
}
