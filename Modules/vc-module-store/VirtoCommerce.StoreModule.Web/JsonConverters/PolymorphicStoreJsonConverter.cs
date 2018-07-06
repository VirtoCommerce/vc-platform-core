using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CoreModule.Core.Model.Payment;
using VirtoCommerce.CoreModule.Core.Model.Shipping;
using VirtoCommerce.CoreModule.Core.Model.Tax;
using VirtoCommerce.CoreModule.Core.Registrars;
using VirtoCommerce.CoreModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;

namespace VirtoCommerce.StoreModule.Web.JsonConverters
{
    public class PolymorphicStoreJsonConverter : JsonConverter
    {
        private static Type[] _knowTypes = new[] { typeof(Store), typeof(StoreSearchCriteria), typeof(PaymentMethod), typeof(ShippingMethod), typeof(TaxProvider) };

        private readonly IPaymentMethodsRegistrar _paymentMethodsService;
        private readonly IShippingMethodsRegistrar _shippingMethodsService;
        private readonly ITaxRegistrar _taxService;
        public PolymorphicStoreJsonConverter(IPaymentMethodsRegistrar paymentMethodsService, IShippingMethodsRegistrar shippingMethodsService, ITaxRegistrar taxService)
        {
            _paymentMethodsService = paymentMethodsService;
            _shippingMethodsService = shippingMethodsService;
            _taxService = taxService;
        }

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
            else if (objectType == typeof(ShippingMethod))
            {
                var shippingGatewayCode = obj["code"].Value<string>();
                retVal = _shippingMethodsService.GetAllShippingMethods().FirstOrDefault(x => x.Code.EqualsInvariant(shippingGatewayCode));
            }
            else if (objectType == typeof(TaxProvider))
            {
                var taxProviderCode = obj["code"].Value<string>();
                retVal = _taxService.GetAllTaxProviders().FirstOrDefault(x => x.Code.EqualsInvariant(taxProviderCode));
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
