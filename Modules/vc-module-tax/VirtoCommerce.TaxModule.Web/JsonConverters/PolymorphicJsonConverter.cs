using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model.Search;

namespace VirtoCommerce.TaxModule.Web.JsonConverters
{
    public class PolymorphicJsonConverter : JsonConverter
    {
        private static Type[] _knowTypes = new[] { typeof(TaxEvaluationContext), typeof(TaxProviderSearchCriteria), typeof(TaxLine), typeof(TaxProvider) };

        public override bool CanWrite { get { return false; } }
        public override bool CanRead { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object result = null;
            var obj = JObject.Load(reader);
            if (typeof(TaxProvider).IsAssignableFrom(objectType))
            {
                var typeName = objectType.Name;
                var pt = obj["typeName"];
                if (pt != null)
                {
                    typeName = pt.Value<string>();
                }
                result = AbstractTypeFactory<TaxProvider>.TryCreateInstance(typeName);
                if (result == null)
                {
                    throw new NotSupportedException("Unknown tax provider type: " + typeName);
                }

            }
            else
            {

                var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && x.GetParameters().Length == 0);
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
