using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Payment;
using VirtoCommerce.CoreModule.Core.Shipping;
using VirtoCommerce.OrderModule.Core.Model;
using VirtoCommerce.OrderModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrderModule.Web.JsonConverters
{
    public class PolymorphicOperationJsonConverter : JsonConverter
    {
        private static readonly Type[] _knowTypes = { typeof(IOperation), typeof(LineItem), typeof(CustomerOrderSearchCriteria), typeof(PaymentMethod), typeof(ShippingMethod) };

        private readonly IPaymentMethodsRegistrar _paymentMethodsRegistrar;
        private readonly IShippingMethodsRegistrar _shippingMethodsRegistrar;
        public PolymorphicOperationJsonConverter(IPaymentMethodsRegistrar paymentMethodsRegistrar, IShippingMethodsRegistrar shippingMethodsRegistrar)
        {
            _paymentMethodsRegistrar = paymentMethodsRegistrar;
            _shippingMethodsRegistrar = shippingMethodsRegistrar;
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
            else if (objectType == typeof(ShippingMethod))
            {
                var shippingGatewayCode = obj["code"].Value<string>();
                retVal = _shippingMethodsRegistrar.GetAllShippingMethods().FirstOrDefault(x => x.Code.EqualsInvariant(shippingGatewayCode));
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
            }

            var payment = operation as PaymentIn;
            if (payment != null)
            {
                var paymentStatus = obj["paymentStatus"].Value<string>();
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
