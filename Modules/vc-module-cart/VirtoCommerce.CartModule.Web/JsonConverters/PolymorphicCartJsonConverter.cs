using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Web.JsonConverters
{
    public class PolymorphicCartJsonConverter : JsonConverter
    {
        private static readonly Type[] _knowTypes = { typeof(ShoppingCart), typeof(ShoppingCartSearchCriteria), typeof(LineItem), typeof(Shipment), typeof(Payment) };

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && !x.GetParameters().Any());
            var retVal = tryCreateInstance.Invoke(null, null);

            serializer.Populate(obj.CreateReader(), retVal);
            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
