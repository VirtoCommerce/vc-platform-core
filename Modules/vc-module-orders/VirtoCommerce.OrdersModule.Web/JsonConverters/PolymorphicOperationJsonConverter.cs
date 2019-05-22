using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CoreModule.Core.Common;

using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.OrdersModule.Web.JsonConverters
{
    public class PolymorphicOperationJsonConverter : JsonConverter
    {
        private static readonly Type[] _knowTypes = { typeof(IOperation), typeof(LineItem), typeof(CustomerOrderSearchCriteria), typeof(ShippingMethod), typeof(PaymentMethod) };


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


            var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && x.GetParameters().Length == 0);
            retVal = tryCreateInstance?.Invoke(null, null);


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
