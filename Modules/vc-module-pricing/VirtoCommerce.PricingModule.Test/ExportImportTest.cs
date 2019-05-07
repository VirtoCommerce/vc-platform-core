using System.IO;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.PricingModule.Data.ExportImport;
using Xunit;

namespace VirtoCommerce.PricingModule.Test
{
    public class ExportImportTest
    {
        [Fact]
        public async Task TestArbitraryImport()
        {
            var data = GetSampleDataStream();

            var pricingService = GetPricingService();

            var settingsManager = GetSettingsManager();

            settingsManager.Setup(s => s.GetObjectSettingAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ObjectSettingEntry { Value = 2 });

            var importProcessor = GetImportExportProcessor(pricingService.Object, settingsManager.Object);

            var cancellationTokenMock = new Mock<ICancellationToken>();
            await importProcessor.DoImportAsync(data, GetProgressCallback, cancellationTokenMock.Object);

            pricingService.Verify(p => p.SavePricesAsync(It.IsAny<Price[]>()), Times.Exactly(2));
            pricingService.Verify(p => p.SavePricelistsAsync(It.IsAny<Pricelist[]>()), Times.Exactly(1));
            pricingService.Verify(p => p.SavePricelistAssignmentsAsync(It.IsAny<PricelistAssignment[]>()), Times.Exactly(1));
        }

        private PricingExportImport GetImportExportProcessor(IPricingService pricingService, ISettingsManager settingsManager)
        {
            return new PricingExportImport(pricingService, GetPricingSearchService(), settingsManager, GetJsonSerializer());
        }

        private Mock<IPricingService> GetPricingService()
        {
            return new Mock<IPricingService>();
        }

        private IPricingSearchService GetPricingSearchService()
        {
            return new Mock<IPricingSearchService>().Object;
        }

        private Mock<ISettingsManager> GetSettingsManager()
        {
            return new Mock<ISettingsManager>();
        }

        private Stream GetStreamFromString(string value)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static void GetProgressCallback(ExportImportProgressInfo exportImportProgressInfo)
        {
        }

        private JsonSerializer GetJsonSerializer()
        {
            return JsonSerializer.Create(new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }

        private Stream GetSampleDataStream()
        {
            var data = @"{
            ""Prices"": [
            {
                ""PricelistId"": ""39e18ca8ea254296a78d47f3f90d649d"",
                ""Currency"": ""EUR"",
                ""ProductId"": ""85e7aa089a4e4a97a4394d668e37e3f8"",
                ""List"": 3.99,
                ""MinQuantity"": 1,
                ""EffectiveValue"": 3.99,
                ""CreatedDate"": ""0001-01-01T00:00:00"",
                ""ModifiedDate"": ""2015-10-06T22:39:49.997"",
                ""ModifiedBy"": ""unknown"",
                ""Id"": ""8304936219b24cfb99c1e4a5496990f5""
            },
            {
                ""PricelistId"": ""39e18ca8ea254296a78d47f3f90d649d"",
                ""Currency"": ""EUR"",
                ""ProductId"": ""f427108e75ed4676923ddc47632111e3"",
                ""List"": 3.99,
                ""MinQuantity"": 1,
                ""EffectiveValue"": 3.99,
                ""CreatedDate"": ""0001-01-01T00:00:00"",
                ""ModifiedDate"": ""2015-10-06T22:39:49.997"",
                ""ModifiedBy"": ""unknown"",
                ""Id"": ""9352d110ba58478398cc88c5f11440fb""
            },
            {
                ""PricelistId"": ""39e18ca8ea254296a78d47f3f90d649d"",
                ""Currency"": ""EUR"",
                ""ProductId"": ""31bb21bf0b144b68a9d731374c1a95e8"",
                ""List"": 28.00,
                ""MinQuantity"": 1,
                ""EffectiveValue"": 28.00,
                ""CreatedDate"": ""0001-01-01T00:00:00"",
                ""ModifiedDate"": ""2015-10-06T22:39:49.997"",
                ""ModifiedBy"": ""unknown"",
                ""Id"": ""d8e00b05241b463cbda119cbd11972c5""
            }], ""Assignments"": [
                {
                ""CatalogId"": ""b61aa9d1d0024bc4be12d79bf5786e9f"",
                ""PricelistId"": ""6a861ff44eb140bebb516a6dc240868c"",
                ""Name"": ""Clothing-Clothing USD"",
                ""Priority"": 0,
                ""ConditionExpression"": ""<LambdaExpression NodeType=\""Lambda\"" Name=\""\"" TailCall=\""false\"" CanReduce=\""false\"">\r\n  <Type>\r\n    <Type Name=\""System.Func`2\"">\r\n      <Type Name=\""VirtoCommerce.Domain.Common.IEvaluationContext\"" />\r\n      <Type Name=\""System.Boolean\"" />\r\n    </Type>\r\n  </Type>\r\n  <Parameters>\r\n    <ParameterExpression NodeType=\""Parameter\"" Name=\""f\"" IsByRef=\""false\"" CanReduce=\""false\"">\r\n      <Type>\r\n        <Type Name=\""VirtoCommerce.Domain.Common.IEvaluationContext\"" />\r\n      </Type>\r\n    </ParameterExpression>\r\n  </Parameters>\r\n  <Body>\r\n    <ConstantExpression NodeType=\""Constant\"" CanReduce=\""false\"">\r\n      <Type>\r\n        <Type Name=\""System.Boolean\"" />\r\n      </Type>\r\n      <Value>True</Value>\r\n    </ConstantExpression>\r\n  </Body>\r\n  <ReturnType>\r\n    <Type Name=\""System.Boolean\"" />\r\n  </ReturnType>\r\n</LambdaExpression>"",
                ""PredicateVisualTreeSerialized"": ""{\""Id\"":\""ConditionExpressionTree\"",\""AvailableChildren\"":null,\""Children\"":[{\""$type\"":\""VirtoCommerce.DynamicExpressionsModule.Data.Pricing.BlockPricingCondition, VirtoCommerce.DynamicExpressionsModule.Data\"",\""All\"":false,\""Id\"":\""BlockPricingCondition\"",\""AvailableChildren\"":null,\""Children\"":[]}]}"",
                ""CreatedDate"": ""2015-10-06T22:39:50.313"",
                ""ModifiedDate"": ""2015-10-06T22:39:50.313"",
                ""CreatedBy"": ""unknown"",
                ""ModifiedBy"": ""unknown"",
               ""Id"": ""2e282397bc9f4541a15c6ddf953e72bd""
            }
            ], ""Pricelists"": [
            {
                ""Name"": ""ClothingEUR"",
                ""sCurrency"": ""EUR"",
                ""CreatedDate"": ""2015 -10-06T22:39:49.997"",
                ""ModifiedDate"": ""2015 -10-06T22:39:49.997"",
                ""CreatedBy"": ""unknown"",
                ""ModifiedBy"": ""unknown"",
                ""Id"": ""39e18ca8ea254296a78d47f3f90d649d""
            }]
            }";

            return GetStreamFromString(data);
        }
    }
}
