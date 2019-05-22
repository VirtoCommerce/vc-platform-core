using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;

namespace VirtoCommerce.ShippingModule.Web.JsonConverters
{
    public class PolymorphicJsonConverter : JsonConverter
    {
        private static readonly Type[] _knowTypes = { typeof(ShippingMethod), typeof(ShippingMethodsSearchCriteria) };

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object result;
            var obj = JObject.Load(reader);

            if (typeof(ShippingMethod).IsAssignableFrom(objectType))
            {
                var typeName = objectType.Name;
                var pt = obj["typeName"];

                if (pt != null)
                {
                    typeName = pt.Value<string>();
                }

                result = AbstractTypeFactory<ShippingMethod>.TryCreateInstance(typeName);
                if (result == null)
                {
                    throw new NotSupportedException("Unknown shipping method type: " + typeName);
                }
            }
            else
            {
                var tryCreateInstance = typeof(AbstractTypeFactory<>)
                                        .MakeGenericType(objectType)
                                        .GetMethods()
                                        .FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && x.GetParameters().Length == 0);

                result = tryCreateInstance?.Invoke(null, null);
            }

            serializer.Populate(obj.CreateReader(), result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
