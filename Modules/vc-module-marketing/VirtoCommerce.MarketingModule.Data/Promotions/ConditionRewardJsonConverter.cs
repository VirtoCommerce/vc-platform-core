using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Promotions
{
    public class ConditionRewardJsonConverter : JsonConverter
    {
        public override bool CanWrite { get; } = false;
        public override bool CanRead { get; } = true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(IConditionRewardTree).IsAssignableFrom(objectType) || typeof(PromotionReward).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;
            var obj = JObject.Load(reader);
            var type = objectType.Name;
            var pt = obj["Id"] ?? obj["id"];
            if (pt != null)
            {
                type = pt.Value<string>();
            }

            if (typeof(IConditionRewardTree).IsAssignableFrom(objectType))
            {
                retVal = AbstractTypeFactory<IConditionRewardTree>.TryCreateInstance(type);
            }
            else if (typeof(PromotionReward).IsAssignableFrom(objectType))
            {
                retVal = AbstractTypeFactory<PromotionReward>.TryCreateInstance(type);
            }

            if (retVal == null)
            {
                throw new NotSupportedException("Unknown type: " + type);
            }

            serializer.Populate(obj.CreateReader(), retVal);
            return retVal;
        }
    }
}
