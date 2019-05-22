using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;

namespace VirtoCommerce.StoreModule.Web.JsonConverters
{
    public class PolymorphicStoreJsonConverter : JsonConverter
    {
        private static Type[] _knowTypes = new[] { typeof(Store), typeof(StoreSearchCriteria), };



        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;
            var obj = JObject.Load(reader);

            if (typeof(Store).IsAssignableFrom(objectType))
            {
                retVal = AbstractTypeFactory<Store>.TryCreateInstance();
            }
            else if (typeof(StoreSearchCriteria).IsAssignableFrom(objectType))
            {
                retVal = AbstractTypeFactory<StoreSearchCriteria>.TryCreateInstance();
            }
           
            serializer.Populate(obj.CreateReader(), retVal);
            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
