using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Common.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Data.DynamicExpressions.Pricing;
using VirtoCommerce.PricingModule.Data.Services;
using Xunit;

namespace VirtoCommerce.PricingModule.Test
{
    public class XmlExpressionSerializerTests
    {
        private readonly XmlExpressionSerializer _serializer;

        public XmlExpressionSerializerTests()
        {
            _serializer = new XmlExpressionSerializer();
        }

        private static async Task<string> ReadTextFromEmbeddedResourceAsync(string filePath)
        {
            var currentAssembly = typeof(XmlExpressionSerializerTests).Assembly;
            var resourcePath = $"{currentAssembly.GetName().Name}.{filePath}";

            using (var resourceStream = currentAssembly.GetManifestResourceStream(resourcePath))
            using (var textReader = new StreamReader(resourceStream))
            {
                return await textReader.ReadToEndAsync();
            }
        }

        [Fact]
        public async Task TestExpressionSerialization()
        {
            //TODO
            // Arrange
            var condition = new BlockPricingCondition
            {
                //All = false,
                //Children = new List<DynamicExpression>
                //{
                //    new ConditionStoreSearchedPhrase
                //    {
                //        MatchCondition = ExpressionConstants.ConditionOperation.Contains,
                //        Value = "test"
                //    },
                //    new ConditionGenderIs
                //    {
                //        MatchCondition = ExpressionConstants.ConditionOperation.Matching,
                //        Value = "male"
                //    },
                //    new ConditionAgeIs
                //    {
                //        CompareCondition = ExpressionConstants.ConditionOperation.IsGreaterThanOrEqual,
                //        Value = 18
                //    }
                //}
            };

            var expressionTree = new ConditionTree
            {
                Children = new List<IConditionTree> { condition }
            };

            //var sourceExpression = expressionTree.();

            //// NOTE: Visual Studio automatically inserts line break to the end of the file, but actual expression doesn't have it.
            ////       So the Trim() call here eliminates that line break.
            //var expectedResult = (await ReadTextFromEmbeddedResourceAsync("Resources.TestSerializedExpression.xml"))?.Trim();

            //// Act
            //var actualResult = _serializer.SerializeExpression(sourceExpression);

            // Assert
            //Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task TestExpressionDeserialization()
        {
            // Arrange
            var serializedExpression = (await ReadTextFromEmbeddedResourceAsync("Resources.TestSerializedExpression.xml"))?.Trim();

            // Act
            var result = _serializer.DeserializeExpression<Func<IEvaluationContext, bool>>(serializedExpression);

            // Assert
            // NOTE: Since we have no way to explore result function and assert that it does expected checks,
            //       let's just feed it with some evaluation contexts and check if it returns expected results.
            // The function should return true if any of these conditions is true:
            // - the customer searched in stores for something containing 'test' string;
            // - the customer is male;
            // - the customer is at least 18 years old.

            // 1. Context does not match the expression at all, so the function must return false.
            var context = new PriceEvaluationContext()
            {
                ShopperSearchedPhraseInStore = "some query",
                ShopperGender = "female",
                ShopperAge = 17
            };
            Assert.False(result(context));

            // 2. ShopperSearchedPhraseInStore contains "test", so the result must be true.
            context.ShopperSearchedPhraseInStore = "some test query";
            Assert.True(result(context));

            // 3. ShopperGender is male, so the result must be true again.
            context.ShopperSearchedPhraseInStore = "some query";
            context.ShopperGender = "male";
            Assert.True(result(context));

            // 4. ShopperAge exceeds 18, so the result must be true again.
            context.ShopperGender = "female";
            context.ShopperAge = 18;
            Assert.True(result(context));

            context.ShopperAge = 21;
            Assert.True(result(context));
        }
    }
}
