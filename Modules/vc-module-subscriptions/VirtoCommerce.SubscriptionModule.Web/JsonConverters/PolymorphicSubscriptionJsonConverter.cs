using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;

namespace VirtoCommerce.SubscriptionModule.Web.JsonConverters
{
    public class PolymorphicSubscriptionJsonConverter : JsonConverter
    {
        private static Type[] _knowTypes = new[] { typeof(Subscription), typeof(PaymentPlan), typeof(SubscriptionSearchCriteria) };

        public override bool CanWrite { get { return false; } }
        public override bool CanRead { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;
            var obj = JObject.Load(reader);

            if (typeof(Subscription).IsAssignableFrom(objectType))
            {
                retVal = AbstractTypeFactory<Subscription>.TryCreateInstance();
            }
            else if (typeof(PaymentPlan).IsAssignableFrom(objectType))
            {
                retVal = AbstractTypeFactory<PaymentPlan>.TryCreateInstance();
            }
            else if (typeof(SubscriptionSearchCriteria).IsAssignableFrom(objectType))
            {
                retVal = AbstractTypeFactory<SubscriptionSearchCriteria>.TryCreateInstance();
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