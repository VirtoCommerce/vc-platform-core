using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Promotions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Web.JsonConverters
{
    public class PolymorphicMarketingJsonConverter : JsonConverter
    {
        private static Type[] _knownTypes = { typeof(Promotion), typeof(DynamicPromotion), typeof(PromotionSearchCriteria), typeof(PromotionEvaluationContext) };

        private readonly IMarketingExtensionManager _marketingExtensionManager;

        public PolymorphicMarketingJsonConverter(IMarketingExtensionManager marketingExtensionManager)
        {
            _marketingExtensionManager = marketingExtensionManager;
        }

        #region Overrides of JsonConverter

        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return _knownTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var serializerClone = CloneSerializerSettings(serializer);
            var jo = JObject.FromObject(value, serializerClone);

            // Workaround for UI: DynamicPromotion type is hardcoded in HTML template
            var promotionType = value.GetType();
            if (typeof(Promotion).IsAssignableFrom(promotionType))
            {
                var dynamicPromotionType = typeof(DynamicPromotion);
                var typeName = dynamicPromotionType.IsAssignableFrom(promotionType)
                    ? dynamicPromotionType.Name
                    : promotionType.Name;
                jo.Add("type", typeName);

                var expressionTree = GetDynamicPromotion(value);
                if (expressionTree != null)
                {
                    jo.Add("dynamicExpression", JToken.FromObject(expressionTree, serializer));
                }

                //manually remove these props because using JsonIgnore breaks import/export
                var ignoredPropertysNames = new[] { "predicateSerialized", "predicateVisualTreeSerialized", "rewardsSerialized" };
                var ignoredProperties = jo.Children<JProperty>().Where(x => ignoredPropertysNames.Contains(x.Name)).ToList();
                foreach (var property in ignoredProperties)
                {
                    property.Remove();
                }
            }

            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object result;
            var jObj = JObject.Load(reader);

            var promoType = jObj["type"]?.Value<string>();
            if (promoType.EqualsInvariant(typeof(DynamicPromotion).Name))
            {
                result = AbstractTypeFactory<DynamicPromotion>.TryCreateInstance();
            }
            else
            {
                var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && !x.GetParameters().Any());
                result = tryCreateInstance?.Invoke(null, null);
            }

            serializer.Populate(jObj.CreateReader(), result);

            var dynamicPromotion = result as DynamicPromotion;
            if (dynamicPromotion != null)
            {
                PopulateDynamicExpression(dynamicPromotion, jObj, serializer);
            }

            return result;
        }

        #endregion

        private void PopulateDynamicExpression(DynamicPromotion dynamicPromotion, JObject jObj, JsonSerializer serializer)
        {
            var dynamicExpressionToken = jObj["dynamicExpression"];
            var dynamicExpression = dynamicExpressionToken?.ToObject<PromotionConditionAndRewardTree>(serializer);
            if (dynamicExpression?.Children != null)
            {
                var conditionExpression = dynamicExpression.GetConditions();
                dynamicPromotion.PredicateSerialized = JsonConvert.SerializeObject(conditionExpression);

                var rewards = dynamicExpression.GetRewards();
                dynamicPromotion.RewardsSerialized = JsonConvert.SerializeObject(rewards);

                // Clear availableElements in expression to decrease size
                dynamicExpression.AvailableChildren = null;
                var allBlocks = ((IConditionTree)dynamicExpression).Traverse(x => x.Children);
                foreach (var block in allBlocks)
                {
                    block.AvailableChildren = null;
                }

                dynamicPromotion.PredicateVisualTreeSerialized = JsonConvert.SerializeObject(dynamicExpression);
            }
        }

        private IConditionTree GetDynamicPromotion(object value)
        {
            IConditionTree result = null;

            var dynamicPromotion = value as DynamicPromotion;
            if (dynamicPromotion?.PredicateVisualTreeSerialized != null)
            {
                result = JsonConvert.DeserializeObject<PromotionConditionAndRewardTree>(
                    dynamicPromotion.PredicateVisualTreeSerialized,
                    new ConditionJsonConverter(), new RewardJsonConverter());
            }

            return result;
        }

        private JsonSerializer CloneSerializerSettings(JsonSerializer jsonSettings)
        {
            var copySettings = new JsonSerializerSettings
            {
                Context = jsonSettings.Context,
                Culture = jsonSettings.Culture,
                ContractResolver = jsonSettings.ContractResolver,
                ConstructorHandling = jsonSettings.ConstructorHandling,
                CheckAdditionalContent = jsonSettings.CheckAdditionalContent,
                DateFormatHandling = jsonSettings.DateFormatHandling,
                DateFormatString = jsonSettings.DateFormatString,
                DateParseHandling = jsonSettings.DateParseHandling,
                DateTimeZoneHandling = jsonSettings.DateTimeZoneHandling,
                DefaultValueHandling = jsonSettings.DefaultValueHandling,
                EqualityComparer = jsonSettings.EqualityComparer,
                FloatFormatHandling = jsonSettings.FloatFormatHandling,
                Formatting = jsonSettings.Formatting,
                FloatParseHandling = jsonSettings.FloatParseHandling,
                MaxDepth = jsonSettings.MaxDepth,
                MetadataPropertyHandling = jsonSettings.MetadataPropertyHandling,
                MissingMemberHandling = jsonSettings.MissingMemberHandling,
                NullValueHandling = jsonSettings.NullValueHandling,
                ObjectCreationHandling = jsonSettings.ObjectCreationHandling,
                PreserveReferencesHandling = jsonSettings.PreserveReferencesHandling,
                ReferenceLoopHandling = jsonSettings.ReferenceLoopHandling,
                StringEscapeHandling = jsonSettings.StringEscapeHandling,
                TraceWriter = jsonSettings.TraceWriter,
                TypeNameHandling = jsonSettings.TypeNameHandling,
                SerializationBinder = jsonSettings.SerializationBinder,
                TypeNameAssemblyFormatHandling = jsonSettings.TypeNameAssemblyFormatHandling,
                Converters = null
            };
            return JsonSerializer.Create(copySettings);
        }
    }
}
