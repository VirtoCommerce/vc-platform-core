using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions
{
    public class ConditionJsonConverter : JsonConverter
    {
        public override bool CanWrite { get; } = false;
        public override bool CanRead { get; } = true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(IConditionTree).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;
            var obj = JObject.Load(reader);
            var type = objectType.Name;
            var pt = obj.GetValue("Id", StringComparison.InvariantCultureIgnoreCase);
            if (pt != null)
            {
                type = pt.Value<string>();
            }

            retVal = AbstractTypeFactory<IConditionTree>.TryCreateInstance(type);

            if (retVal == null)
            {
                throw new NotSupportedException("Unknown type: " + type);
            }

            serializer.Populate(obj.CreateReader(), retVal);
            return retVal;
        }
    }
}
