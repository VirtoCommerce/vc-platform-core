using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CoreModule.Core.Payment;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;

namespace VirtoCommerce.StoreModule.Web.JsonConverters
{
    public class PolymorphicStoreJsonConverter : JsonConverter
    {
        private static Type[] _knowTypes = { typeof(Store), typeof(StoreSearchCriteria), typeof(PaymentMethod) };

        private readonly IPaymentMethodsRegistrar _paymentMethodsService;
        public PolymorphicStoreJsonConverter(IPaymentMethodsRegistrar paymentMethodsService)
        {
            _paymentMethodsService = paymentMethodsService;
        }

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
            else if (objectType == typeof(PaymentMethod))
            {
                var paymentGatewayCode = obj["code"].Value<string>();
                retVal = _paymentMethodsService.GetAllPaymentMethods().FirstOrDefault(x => x.Code.EqualsInvariant(paymentGatewayCode));
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
