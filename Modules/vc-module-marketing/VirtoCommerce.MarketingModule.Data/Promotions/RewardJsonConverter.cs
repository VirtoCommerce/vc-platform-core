using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Promotions
{
    public class RewardJsonConverter : JsonConverter
    {
        public override bool CanWrite { get; } = false;
        public override bool CanRead { get; } = true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(PromotionReward).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var type = objectType.Name;
            var pt = obj["Id"] ?? obj["id"];
            if (pt != null)
            {
                type = pt.Value<string>();
            }

            object retVal = AbstractTypeFactory<PromotionReward>.TryCreateInstance(type);

            if (retVal == null)
            {
                throw new NotSupportedException("Unknown type: " + type);
            }

            serializer.Populate(obj.CreateReader(), retVal);
            return retVal;
        }
    }
}
