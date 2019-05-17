using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PaymentModule.Web.JsonConverters
{
    public class PolymorphicJsonConverter : JsonConverter
    {
        private static readonly Type[] _knowTypes = { typeof(PaymentMethod), typeof(PaymentMethodsSearchCriteria) };

        private readonly IPaymentMethodsRegistrar _paymentMethodsRegistrar;
        public PolymorphicJsonConverter(IPaymentMethodsRegistrar paymentMethodsRegistrar)
        {
            _paymentMethodsRegistrar = paymentMethodsRegistrar;
        }

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

            if (typeof(PaymentMethod).IsAssignableFrom(objectType))
            {
                var typeName = objectType.Name;
                var pt = obj["typeName"];
                if (pt != null)
                {
                    typeName = pt.Value<string>();
                }

                result = AbstractTypeFactory<PaymentMethod>.TryCreateInstance(typeName);
                if (result == null)
                {
                    throw new NotSupportedException("Unknown payment method type: " + typeName);
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
