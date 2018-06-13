using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Domain.Shipping.Model;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Domain.Tax.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.StoreModule.Web.JsonConverters
{
    //Because Platform 2.12.2 version has default global converter StringEnumConverter(camelCase: true) 
    //and current manager UI required settingValueType with capital letter.
    //This converter used only to override global JSON enum policy (camelCase: true) -> camelCase: false
    //And may be removed after platform updated to new version where camelCase: true enum serialization policy will be removed
    public class SettingValueTypeEnumJsonConverter : JsonConverter
    {   
        public override bool CanWrite { get { return true; } }
        public override bool CanRead { get { return false; } }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SettingValueType) || objectType == typeof(StoreState);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}