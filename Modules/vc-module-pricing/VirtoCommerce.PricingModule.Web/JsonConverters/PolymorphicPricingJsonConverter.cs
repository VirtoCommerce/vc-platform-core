using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Web.JsonConverters
{
    public class PolymorphicPricingJsonConverter : JsonConverter
    {
        private static readonly Type[] KnowTypes = { typeof(Price), typeof(Pricelist), typeof(PricelistAssignment),
                                                     typeof(PriceEvaluationContext),
                                                     typeof(PricesSearchCriteria), typeof(PricelistAssignmentsSearchCriteria), typeof(PricelistSearchCriteria) };

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return KnowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods().First(x => x.Name.EqualsInvariant("TryCreateInstance") && !x.GetParameters().Any());
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
