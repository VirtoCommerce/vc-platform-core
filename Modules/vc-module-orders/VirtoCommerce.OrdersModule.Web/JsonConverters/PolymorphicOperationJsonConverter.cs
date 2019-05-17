using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Payment;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Web.JsonConverters
{
    public class PolymorphicOperationJsonConverter : JsonConverter
    {
        private static readonly Type[] _knowTypes = { typeof(IOperation), typeof(LineItem), typeof(CustomerOrderSearchCriteria), typeof(PaymentMethod) };

        private readonly IPaymentMethodsRegistrar _paymentMethodsRegistrar;
        public PolymorphicOperationJsonConverter(IPaymentMethodsRegistrar paymentMethodsRegistrar)
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
            object retVal;
            var obj = JObject.Load(reader);

            if (objectType == typeof(PaymentMethod))
            {
                var paymentGatewayCode = obj["code"].Value<string>();
                retVal = _paymentMethodsRegistrar.GetAllPaymentMethods().FirstOrDefault(x => x.Code.EqualsInvariant(paymentGatewayCode));
            }
            else
            {
                var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && x.GetParameters().Length == 0);
                retVal = tryCreateInstance?.Invoke(null, null);
            }
            //Reset ChildrenOperations property to prevent polymorphic deserialization  error
            var operation = retVal as IOperation;
            if (operation != null)
            {
                obj.Remove("childrenOperations");
                obj.Remove("ChildrenOperations");
            }

            var payment = operation as PaymentIn;
            if (payment != null)
            {
                var paymentStatus = (obj["paymentStatus"] ?? obj["PaymentStatus"]).Value<string>();
                var hasStatusValue = Enum.IsDefined(typeof(PaymentStatus), paymentStatus);
                if (!hasStatusValue)
                {
                    obj["paymentStatus"] = PaymentStatus.Custom.ToString();
                }
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
