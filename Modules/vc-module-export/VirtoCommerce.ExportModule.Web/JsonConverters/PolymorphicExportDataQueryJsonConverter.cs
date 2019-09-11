using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Web.JsonConverters
{
    public class PolymorphicExportDataQueryJsonConverter : JsonConverter
    {
        private static readonly Type[] _knownTypes = { typeof(ExportDataQuery) };

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return _knownTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var typeName = objectType.Name;
            var exportTypeName = obj["exportTypeName"];
            if (exportTypeName != null)
            {
                typeName = exportTypeName.Value<string>();
            }

            var result = AbstractTypeFactory<ExportDataQuery>.TryCreateInstance(typeName);
            if (result == null)
            {
                throw new NotSupportedException("Unknown ExportDataQuery type: " + typeName);
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
