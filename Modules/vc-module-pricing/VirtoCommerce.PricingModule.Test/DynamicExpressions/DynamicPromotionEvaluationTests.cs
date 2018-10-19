using System.Linq;
using Moq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Serialization;
using VirtoCommerce.PricingModule.Core.Model.CommonExpressions;
using VirtoCommerce.PricingModule.Core.Model.Promotions.Expressions;
using VirtoCommerce.PricingModule.Data.Services;
using Xunit;

namespace VirtoCommerce.PricingModule.Test.DynamicExpressions
{
    public class DynamicPromotionEvaluationTests
    {
        // TODO: this test requires Marketing module services, but is it really OK to mix them into the Pricing module?

//        protected IExpressionSerializer expressionSerializer = new XmlExpressionSerializer();
//        private ICouponService couponService = new Mock<ICouponService>().Object;
//        private IPromotionUsageService promotionUsageService = new Mock<IPromotionUsageService>().Object;

//        [Theory]
//#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant
//        [MemberData(nameof(EvaluationTestDataGenerator.GetConditions), MemberType = typeof(EvaluationTestDataGenerator))]
//#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant
//        public void CheckPromotionValid(IConditionExpression[] conditions, IRewardExpression[] rewards, IEvaluationContext context, DynamicPromotionEvaluationResult evaluationResult)
//        {
//            DynamicPromotion dynamicPromotion = GetDynamicPromotion(conditions, rewards);

//            var result = dynamicPromotion.EvaluatePromotion(context);

//            Assert.Equal(evaluationResult.ValidCount, result.Count(r => r.IsValid));
//            Assert.Equal(evaluationResult.InvalidCount, result.Count(r => !r.IsValid));
//        }

//        private DynamicPromotion GetDynamicPromotion(IConditionExpression[] conditions, IRewardExpression[] rewards)
//        {
//            var dynamicPromotion = new DynamicPromotion(expressionSerializer, couponService, promotionUsageService);
//            dynamicPromotion.PredicateSerialized = GetPredicateSerialized(conditions);
//            dynamicPromotion.RewardsSerialized = GetRewardsSerialized(rewards);

//            return dynamicPromotion;
//        }

//        private string GetPredicateSerialized(IConditionExpression[] conditions)
//        {
//            var predicate = PredicateBuilder.False<IEvaluationContext>();
//            foreach (var expression in conditions.Select(x => x.GetConditionExpression()))
//            {
//                predicate = predicate.Or(expression);
//            }

//            return expressionSerializer.SerializeExpression(predicate);
//        }

//        private string GetRewardsSerialized(IRewardExpression[] rewards)
//        {
//            var promotionRewards = rewards.SelectMany(r => r.GetRewards()).ToArray();
//            return JsonConvert.SerializeObject(promotionRewards, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
//        }
    }
}
