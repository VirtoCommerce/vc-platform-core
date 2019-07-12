using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.JsonConverters
{
    public class SearchCriteriaJsonConverter : JsonConverter
    {
        private readonly Type[] _knowTypes = new[] { typeof(CatalogProduct), typeof(ProductIndexedSearchCriteria), typeof(CategoryIndexedSearchCriteria), typeof(CatalogSearchCriteria), typeof(CategorySearchCriteria), typeof(ProductSearchCriteria) };

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object result = null;

            var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && x.GetParameters().Length == 0);
            result = tryCreateInstance?.Invoke(null, null);

            var obj = JObject.Load(reader);
            serializer.Populate(obj.CreateReader(), result);

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
