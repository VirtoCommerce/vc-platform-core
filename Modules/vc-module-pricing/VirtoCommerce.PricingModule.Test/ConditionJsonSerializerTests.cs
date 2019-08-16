using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions.Browse;
using VirtoCommerce.CoreModule.Core.Conditions.GeoConditions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Conditions;
using Xunit;

namespace VirtoCommerce.PricingModule.Test
{
    public class ConditionJsonSerializerTests
    {
        private static async Task<string> ReadTextFromEmbeddedResourceAsync(string filePath)
        {
            var currentAssembly = typeof(ConditionJsonSerializerTests).Assembly;
            var resourcePath = $"{currentAssembly.GetName().Name}.{filePath}";

            using (var resourceStream = currentAssembly.GetManifestResourceStream(resourcePath))
            using (var textReader = new StreamReader(resourceStream))
            {
                return await textReader.ReadToEndAsync();
            }
        }

        [Fact]
        public async Task TestConditionSerialization()
        {
            // Arrange
            var condition = new BlockPricingCondition
            {
                All = false,
                Children = new List<IConditionTree>
                {
                    new ConditionStoreSearchedPhrase
                    {
                        MatchCondition = ConditionOperation.Contains,
                        Value = "test"
                    },
                    new ConditionGenderIs
                    {
                        MatchCondition = ConditionOperation.Matching,
                        Value = "male"
                    },
                    new ConditionAgeIs
                    {
                        CompareCondition = ConditionOperation.IsGreaterThanOrEqual,
                        Value = 18
                    }
                }
            };

            var conditionTree = new PriceConditionTree
            {
                Children = new List<IConditionTree> { condition }
            };

            var serializedConditionTree = (await ReadTextFromEmbeddedResourceAsync("Resources.TestSerializedCondition.json"))?.Trim();

            //// Act
            var actualResult = JsonConvert.SerializeObject(conditionTree);

            // Assert
            Assert.Equal(serializedConditionTree, actualResult);
        }

        [Fact]
        public async Task TestExpressionDeserialization()
        {
            // Arrange
            var serializedConditionTree = (await ReadTextFromEmbeddedResourceAsync("Resources.TestSerializedCondition.json"))?.Trim();
            RegisterTypes();

            // Act
            var result = JsonConvert.DeserializeObject<PriceConditionTree>(serializedConditionTree, new ConditionJsonConverter());

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
            Assert.False(result.IsSatisfiedBy(context));

            // 2. ShopperSearchedPhraseInStore contains "test", so the result must be true.
            context.ShopperSearchedPhraseInStore = "test";
            Assert.True(result.IsSatisfiedBy(context));

            // 3. ShopperGender is male, so the result must be true again.
            context.ShopperSearchedPhraseInStore = "some query";
            context.ShopperGender = "male";
            Assert.True(result.IsSatisfiedBy(context));

            // 4. ShopperAge exceeds 18, so the result must be true again.
            context.ShopperGender = "female";
            context.ShopperAge = 18;
            Assert.True(result.IsSatisfiedBy(context));

            context.ShopperAge = 21;
            Assert.True(result.IsSatisfiedBy(context));
        }


        private void RegisterTypes()
        {
            AbstractTypeFactory<IConditionTree>.RegisterType<PriceConditionTree>();
            AbstractTypeFactory<IConditionTree>.RegisterType<BlockPricingCondition>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionAgeIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGenderIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionLanguageIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionStoreSearchedPhrase>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionUrlIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoCity>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoCountry>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoState>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoTimeZone>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoZipCode>();
            AbstractTypeFactory<IConditionTree>.RegisterType<UserGroupsContainsCondition>();
        }
    }
}
