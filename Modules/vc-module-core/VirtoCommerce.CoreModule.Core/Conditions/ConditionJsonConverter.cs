using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions
{
    public class ConditionJsonConverter : JsonConverter
    {
        private readonly bool _doNotSerializeAvailCondition;
        public ConditionJsonConverter(bool doNotSerializeAvailCondition = false)
        {
            _doNotSerializeAvailCondition = doNotSerializeAvailCondition;
        }
        public override bool CanWrite
        {
            get
            {
                return _doNotSerializeAvailCondition;
            }
        }
        public override bool CanRead { get; } = true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(IConditionTree).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //need to remove "AvailableChildren" from resulting tree Json to reduce size
            if (value != null)
            {
                var result = JObject.FromObject(value);
                if (result != null)
                {
                    result.Remove(nameof(IConditionTree.AvailableChildren));
                    result.WriteTo(writer, this);
                }
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var type = objectType.Name;
            var pt = obj.GetValue("Id", StringComparison.InvariantCultureIgnoreCase);
            if (pt != null)
            {
                type = pt.Value<string>();
            }
            object result;
            //First check what condition is registered in AbstractTypeFactory<IConditionTree> otherwise use AbstractTypeFactory<T> factory for create an instance 
            if (AbstractTypeFactory<IConditionTree>.FindTypeInfoByName(type) != null)
            {
                result = AbstractTypeFactory<IConditionTree>.TryCreateInstance(type);
            }
            else
            {
                var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && x.GetParameters().Length == 0);
                result = tryCreateInstance?.Invoke(null, null);
            }

            if (result == null)
            {
                throw new NotSupportedException("Unknown type: " + type);
            }

            serializer.Populate(obj.CreateReader(), result);
            return result;
        }       
    }
}
