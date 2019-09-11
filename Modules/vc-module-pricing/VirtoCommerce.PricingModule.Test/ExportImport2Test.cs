using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.CsvProvider;
using VirtoCommerce.ExportModule.Data.Extensions;
using VirtoCommerce.ExportModule.Data.Services;
using VirtoCommerce.ExportModule.JsonProvider;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.PricingModule.Data.ExportImport;
using Xunit;

namespace VirtoCommerce.PricingModule.Test
{
    public class ExportImport2Test
    {
        [Fact]
        [SuppressMessage("Sonar", "S3966:Objects should not be disposed more than once")]
        public Task PriceJsonExport()
        {
            //Arrange
            IKnownExportTypesRegistrar registrar = new KnownExportTypesService();

            var searchServiceMock = new Mock<IPricingSearchService>();
            searchServiceMock.Setup(x => x.SearchPricesAsync(It.Is<PricesSearchCriteria>(y => y.Skip == 0))).ReturnsAsync(GetTestPriceResult());
            searchServiceMock.Setup(x => x.SearchPricesAsync(It.Is<PricesSearchCriteria>(y => y.Skip > 0))).ReturnsAsync(new PriceSearchResult());

            var priceServiceMock = new Mock<IPricingService>();

            var itemServiceMock = new Mock<IItemService>();

            var metadata = typeof(ExportablePrice).GetPropertyNames();
            var resolver = (IKnownExportTypesResolver)registrar;

            registrar.RegisterType(ExportedTypeDefinitionBuilder.Build<ExportablePrice, PriceExportDataQuery>()
                .WithDataSourceFactory(new PricingExportPagedDataSourceFactory(searchServiceMock.Object, priceServiceMock.Object, itemServiceMock.Object, new Mock<ICatalogService>().Object))
                .WithMetadata(metadata));

            var includedPropertyNames = new string[] { "Currency", "ProductId", "Sale", "List", "MinQuantity", "StartDate", "EndDate", "EffectiveValue" };
            var IncludedProperties = metadata.PropertyInfos.Where(x => includedPropertyNames.Contains(x.FullName, StringComparer.OrdinalIgnoreCase)).ToArray();

            var exportProviderFactories = new[] {
                new Func<ExportDataRequest, IExportProvider>((request) => new JsonExportProvider(request)),
            };

            var dataExporter = new DataExporter(resolver, new ExportProviderFactory(exportProviderFactories));

            //Act
            var result = string.Empty;
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                dataExporter.Export(
                    stream,
                    new ExportDataRequest()
                    {
                        DataQuery = new PriceExportDataQuery()
                        {
                            IncludedProperties = IncludedProperties
                        },
                        ExportTypeName = typeof(ExportablePrice).FullName,
                        ProviderName = nameof(JsonExportProvider)
                    },
                    new Action<ExportProgressInfo>(x => Console.WriteLine(x.Description)),
                    new CancellationTokenWrapper(CancellationToken.None));

                stream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(stream);
                result = reader.ReadToEnd();
            }

            //Assert
            var prices = JsonConvert.DeserializeObject<ExportablePrice[]>(result);

            Assert.NotNull(prices);
            Assert.Equal(3, prices.Length);
            Assert.Equal(2, prices.Count(x => x.List == 3.99m));
            Assert.Equal(1, prices.Count(x => x.ProductId == "d029526eab5948b189694f1bddba8e68"));

            return Task.CompletedTask;
        }


        [Fact]
        [SuppressMessage("Sonar", "S3966:Objects should not be disposed more than once")]
        public Task PricelistJsonExport()
        {
            //Arrange
            IKnownExportTypesRegistrar registrar = new KnownExportTypesService();

            var searchServiceMock = new Mock<IPricingSearchService>();
            searchServiceMock.Setup(x => x.SearchPricelistsAsync(It.Is<PricelistSearchCriteria>(y => y.Skip == 0))).ReturnsAsync(GetTestPricelistResult());
            searchServiceMock.Setup(x => x.SearchPricelistsAsync(It.Is<PricelistSearchCriteria>(y => y.Skip > 0))).ReturnsAsync(new PricelistSearchResult());

            searchServiceMock.Setup(x => x.SearchPricesAsync(It.IsAny<PricesSearchCriteria>())).ReturnsAsync(new PriceSearchResult()
            {
                TotalCount = 0,
                Results = new List<Price>(),
            });

            var priceServiceMock = new Mock<IPricingService>();

            var metadata = typeof(ExportablePricelist).GetPropertyNames();
            var resolver = (IKnownExportTypesResolver)registrar;
            registrar.RegisterType(ExportedTypeDefinitionBuilder.Build<ExportablePricelist, PricelistExportDataQuery>()
                .WithDataSourceFactory(new PricingExportPagedDataSourceFactory(searchServiceMock.Object, priceServiceMock.Object, new Mock<IItemService>().Object, new Mock<ICatalogService>().Object))
                .WithMetadata(metadata));

            var exportProviderFactories = new[] {
                new Func<ExportDataRequest, IExportProvider>((request) => new JsonExportProvider(request)),
            };

            var dataExporter = new DataExporter(resolver, new ExportProviderFactory(exportProviderFactories));
            var includedPropertyNames = new string[] { "Currency", "Id", "Name" };
            var IncludedProperties = metadata.PropertyInfos.Where(x => includedPropertyNames.Contains(x.FullName, StringComparer.OrdinalIgnoreCase)).ToArray();

            //Act
            var result = string.Empty;
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                dataExporter.Export(
                    stream,
                    new ExportDataRequest()
                    {
                        DataQuery = new PricelistExportDataQuery()
                        {
                            IncludedProperties = IncludedProperties
                        },
                        ExportTypeName = typeof(ExportablePricelist).FullName,
                        ProviderName = nameof(JsonExportProvider)
                    },
                    new Action<ExportProgressInfo>(x => Console.WriteLine(x.Description)),
                    new CancellationTokenWrapper(CancellationToken.None));

                stream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(stream);
                result = reader.ReadToEnd();
            }

            //Assert
            var pricelists = JsonConvert.DeserializeObject<ExportablePricelist[]>(result);

            Assert.NotNull(pricelists);
            Assert.NotEmpty(pricelists);

            var pricelist = pricelists.FirstOrDefault(x => x.Name.Equals("BoltsUSD", StringComparison.InvariantCultureIgnoreCase));

            Assert.NotNull(pricelist);
            Assert.Equal("0456b3ebc0a24c0ab7054ec9a5cea78e", pricelist.Id);
            Assert.Equal("USD", pricelist.Currency);

            return Task.CompletedTask;
        }

        [Fact]
        [SuppressMessage("Sonar", "S3966:Objects should not be disposed more than once")]
        public Task PricelistAssignmentJsonExport()
        {
            IKnownExportTypesRegistrar registrar = new KnownExportTypesService();

            var searchServiceMock = new Mock<IPricingSearchService>();
            searchServiceMock.Setup(x => x.SearchPricelistAssignmentsAsync(It.Is<PricelistAssignmentsSearchCriteria>(y => y.Skip == 0))).ReturnsAsync(GetPricelistAssignmentSearchResult());
            searchServiceMock.Setup(x => x.SearchPricelistAssignmentsAsync(It.Is<PricelistAssignmentsSearchCriteria>(y => y.Skip > 0))).ReturnsAsync(new PricelistAssignmentSearchResult());

            var priceServiceMock = new Mock<IPricingService>();
            var catalogServiceMock = new Mock<ICatalogService>();

            var metadata = typeof(ExportablePricelistAssignment).GetPropertyNames();
            var resolver = (IKnownExportTypesResolver)registrar;
            registrar.RegisterType(ExportedTypeDefinitionBuilder.Build<ExportablePricelistAssignment, PricelistAssignmentExportDataQuery>()
                .WithDataSourceFactory(new PricingExportPagedDataSourceFactory(searchServiceMock.Object, priceServiceMock.Object, new Mock<IItemService>().Object, catalogServiceMock.Object))
                .WithMetadata(metadata));

            var exportProviderFactories = new[] {
                new Func<ExportDataRequest, IExportProvider>((request) => new JsonExportProvider(request)),
            };

            var dataExporter = new DataExporter(resolver, new ExportProviderFactory(exportProviderFactories));
            var includedPropertyNames = new string[] { "CatalogId", "PricelistId", "Priority", "Id" };
            var IncludedProperties = metadata.PropertyInfos.Where(x => includedPropertyNames.Contains(x.FullName, StringComparer.OrdinalIgnoreCase)).ToArray();

            //Act
            var result = string.Empty;
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                dataExporter.Export(
                    stream,
                    new ExportDataRequest()
                    {
                        DataQuery = new PricelistAssignmentExportDataQuery()
                        {
                            IncludedProperties = IncludedProperties
                        },
                        ExportTypeName = typeof(ExportablePricelistAssignment).FullName,
                        ProviderName = nameof(JsonExportProvider)
                    },
                    new Action<ExportProgressInfo>(x => Console.WriteLine(x.Description)),
                    new CancellationTokenWrapper(CancellationToken.None));

                stream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(stream);
                result = reader.ReadToEnd();
            }

            //Assert
            var pricelistsAssigments = JsonConvert.DeserializeObject<ExportablePricelistAssignment[]>(result);

            Assert.NotNull(pricelistsAssigments);
            Assert.NotEmpty(pricelistsAssigments);

            var assigment = pricelistsAssigments.FirstOrDefault(x => x.Id == "d4a4e5271046497eaef61ee47efe6215");

            Assert.NotNull(assigment);
            Assert.Null(assigment.Name);
            Assert.Equal("80873a35a34a4cf8997ac87e69cac6d6", assigment.PricelistId);
            Assert.Equal("4974648a41df4e6ea67ef2ad76d7bbd4", assigment.CatalogId);
            Assert.Equal(10, assigment.Priority);

            return Task.CompletedTask;
        }


        [Fact]
        [SuppressMessage("Sonar", "S3966:Objects should not be disposed more than once")]
        public Task PriceCsvExport()
        {
            IKnownExportTypesRegistrar registrar = new KnownExportTypesService();

            var resolver = (IKnownExportTypesResolver)registrar;

            var searchServiceMock = new Mock<IPricingSearchService>();
            searchServiceMock.Setup(x => x.SearchPricesAsync(It.Is<PricesSearchCriteria>(y => y.Skip == 0))).ReturnsAsync(GetTestPriceResult());
            searchServiceMock.Setup(x => x.SearchPricesAsync(It.Is<PricesSearchCriteria>(y => y.Skip > 0))).ReturnsAsync(new PriceSearchResult());

            var priceServiceMock = new Mock<IPricingService>();

            var itemServiceMock = new Mock<IItemService>();

            var metadata = typeof(ExportablePrice).GetPropertyNames();
            registrar.RegisterType(ExportedTypeDefinitionBuilder.Build<ExportablePrice, PriceExportDataQuery>()
                .WithDataSourceFactory(new PricingExportPagedDataSourceFactory(searchServiceMock.Object, priceServiceMock.Object, itemServiceMock.Object, new Mock<ICatalogService>().Object))
                .WithMetadata(metadata)
                .WithTabularMetadata(typeof(TabularPrice).GetPropertyNames()));

            var exportProviderFactories = new[]
            {
                new Func<ExportDataRequest, IExportProvider>((request) => new JsonExportProvider(request)),
                new Func<ExportDataRequest, IExportProvider>((request) => new CsvExportProvider(request)),
            };

            var includedPropertyNames = new string[] { "Currency", "ProductId" };
            var IncludedProperties = metadata.PropertyInfos.Where(x => includedPropertyNames.Contains(x.FullName, StringComparer.OrdinalIgnoreCase)).ToArray();
            var dataExporter = new DataExporter(resolver, new ExportProviderFactory(exportProviderFactories));

            var result = string.Empty;
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                dataExporter.Export(
                    stream,
                    new ExportDataRequest()
                    {
                        DataQuery = new PriceExportDataQuery()
                        {
                            IncludedProperties = IncludedProperties,
                        },
                        ExportTypeName = typeof(ExportablePrice).FullName,
                        ProviderName = nameof(CsvExportProvider)
                    },
                    new Action<ExportProgressInfo>(x => Console.WriteLine(x.Description)),
                    new CancellationTokenWrapper(CancellationToken.None));

                stream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(stream);
                result = reader.ReadToEnd();
            }

            Assert.Equal("Currency,ProductId\r\nUSD,d029526eab5948b189694f1bddba8e68\r\nEUR,85e7aa089a4e4a97a4394d668e37e3f8\r\nEUR,f427108e75ed4676923ddc47632111e3\r\n", result);

            return Task.CompletedTask;
        }

        /// <summary>
        /// There is fake prices set
        /// </summary>
        /// <returns></returns>
        private PriceSearchResult GetTestPriceResult()
        {
            #region resultInJSON
            string resultInJSON = @"
{
   ""TotalCount"": 3,
   ""Results"": [
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""d029526eab5948b189694f1bddba8e68"",
         ""Sale"": null,
         ""List"": 0,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 0,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:46:59.583"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""3ee2570246b0409283ccc7f45878cb55""
      },
      {
         ""PricelistId"": ""39e18ca8ea254296a78d47f3f90d649d"",
         ""Pricelist"": null,
         ""Currency"": ""EUR"",
         ""ProductId"": ""85e7aa089a4e4a97a4394d668e37e3f8"",
         ""Sale"": null,
         ""List"": 3.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 3.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:49.997"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""8304936219b24cfb99c1e4a5496990f5""
      },
      {
         ""PricelistId"": ""39e18ca8ea254296a78d47f3f90d649d"",
         ""Pricelist"": null,
         ""Currency"": ""EUR"",
         ""ProductId"": ""f427108e75ed4676923ddc47632111e3"",
         ""Sale"": null,
         ""List"": 3.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 3.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:49.997"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""9352d110ba58478398cc88c5f11440fb""
      }
   ]
}
";
            #endregion
            return (PriceSearchResult)JsonConvert.DeserializeObject(resultInJSON, typeof(PriceSearchResult));
        }

        private PricelistSearchResult GetTestPricelistResult()
        {
            #region resultInJSON
            string resultInJSON = @"
{
  ""totalCount"": 2,
  ""results"": [
    {
      ""name"": ""BoltsUSD"",
      ""currency"": ""USD"",
      ""assignments"": [
        {
          ""catalogId"": ""7829d35f417e4dd98851f51322f32c23"",
          ""name"": ""BoltsUSD"",
          ""priority"": 1,
          ""createdDate"": ""0001-01-01T00:00:00Z"",
          ""id"": ""ba06645e4605430493d9075ff48edbe9""
        },
        {
          ""catalogId"": ""fc596540864a41bf8ab78734ee7353a3"",
          ""name"": ""B2B-mixed-BOLTS"",
          ""priority"": 1,
          ""createdDate"": ""0001-01-01T00:00:00Z"",
          ""id"": ""cbddca102c54454eb7f203c13419d8c6""
        }
      ],
      ""createdDate"": ""2018-04-27T08:55:16.073Z"",
      ""modifiedDate"": ""2018-04-27T08:55:16.073Z"",
      ""createdBy"": ""admin"",
      ""modifiedBy"": ""admin"",
      ""id"": ""0456b3ebc0a24c0ab7054ec9a5cea78e""
    },
    {
      ""name"": ""Bolts-USD-Special"",
      ""currency"": ""USD"",
      ""assignments"": [
        {
          ""catalogId"": ""fc596540864a41bf8ab78734ee7353a3"",
          ""name"": ""VIPBoltsUSD"",
          ""priority"": 100,
          ""createdDate"": ""0001-01-01T00:00:00Z"",
          ""id"": ""36618d4995474bd0ab58fd518dd090c3""
        }
      ],
      ""createdDate"": ""2018-05-03T14:44:16.227Z"",
      ""modifiedDate"": ""2018-05-03T14:44:16.227Z"",
      ""createdBy"": ""admin"",
      ""modifiedBy"": ""admin"",
      ""id"": ""34efb7152a2b4d018a86878f9a0868bf""
    }
  ]
}
";
            #endregion
            return (PricelistSearchResult)JsonConvert.DeserializeObject(resultInJSON, typeof(PricelistSearchResult), new ConditionJsonConverter());
        }
        private PricelistAssignmentSearchResult GetPricelistAssignmentSearchResult()
        {
            #region resultInJSON
            string resultInJSON = @"
{
  ""totalCount"": 14,
  ""results"": [
    {
      ""catalogId"": ""fc596540864a41bf8ab78734ee7353a3"",
      ""pricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
      ""pricelist"": {
        ""name"": ""Bolts-USD-Special"",
        ""currency"": ""USD"",
        ""createdDate"": ""0001-01-01T00:00:00Z"",
        ""id"": ""34efb7152a2b4d018a86878f9a0868bf""
      },
      ""name"": ""VIPBoltsUSD"",
      ""priority"": 100,
      ""conditionExpression"": ""[{\""All\"":false,\""Not\"":false,\""AvailableChildren\"":[{\""Value\"":0,\""SecondValue\"":0,\""CompareCondition\"":\""AtLeast\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGeoTimeZone\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGeoZipCode\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionStoreSearchedPhrase\""},{\""Value\"":0,\""SecondValue\"":0,\""CompareCondition\"":\""AtLeast\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionAgeIs\""},{\""Value\"":\""female\"",\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGenderIs\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGeoCity\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGeoCountry\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGeoState\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionLanguageIs\""},{\""Group\"":null,\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""UserGroupsContainsCondition\""}],\""Children\"":[{\""Group\"":\""VIP\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""UserGroupsContainsCondition\""}],\""Id\"":\""BlockPricingCondition\""}]"",
      ""predicateVisualTreeSerialized"": ""{\""AvailableChildren\"":null,\""Children\"":[{\""All\"":false,\""Not\"":false,\""AvailableChildren\"":null,\""Children\"":[{\""Group\"":\""VIP\"",\""AvailableChildren\"":null,\""Children\"":[],\""Id\"":\""UserGroupsContainsCondition\""}],\""Id\"":\""BlockPricingCondition\""}],\""Id\"":\""PriceConditionTree\""}"",
      ""createdDate"": ""2018-05-03T14:48:25.263Z"",
      ""modifiedDate"": ""2018-05-03T14:48:25.263Z"",
      ""createdBy"": ""admin"",
      ""modifiedBy"": ""admin"",
      ""id"": ""36618d4995474bd0ab58fd518dd090c3""
    },
    {
      ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
      ""pricelistId"": ""80873a35a34a4cf8997ac87e69cac6d6"",
      ""pricelist"": {
        ""name"": ""ElectronicSpecialUSD"",
        ""currency"": ""USD"",
        ""createdDate"": ""0001-01-01T00:00:00Z"",
        ""id"": ""80873a35a34a4cf8997ac87e69cac6d6""
      },
      ""name"": ""SpecialElectronic"",
      ""priority"": 10,
      ""conditionExpression"": ""[{\""All\"":false,\""Not\"":false,\""AvailableChildren\"":[{\""Value\"":0,\""SecondValue\"":0,\""CompareCondition\"":\""AtLeast\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGeoTimeZone\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGeoZipCode\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionStoreSearchedPhrase\""},{\""Value\"":0,\""SecondValue\"":0,\""CompareCondition\"":\""AtLeast\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionAgeIs\""},{\""Value\"":\""female\"",\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGenderIs\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGeoCity\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGeoCountry\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionGeoState\""},{\""Value\"":null,\""MatchCondition\"":\""Contains\"",\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""ConditionLanguageIs\""},{\""Group\"":null,\""AvailableChildren\"":[],\""Children\"":[],\""Id\"":\""UserGroupsContainsCondition\""}],\""Children\"":[],\""Id\"":\""BlockPricingCondition\""}]"",
      ""predicateVisualTreeSerialized"": ""{\""AvailableChildren\"":null,\""Children\"":[{\""All\"":false,\""Not\"":false,\""AvailableChildren\"":null,\""Children\"":[],\""Id\"":\""BlockPricingCondition\""}],\""Id\"":\""PriceConditionTree\""}"",
      ""createdDate"": ""2016-08-15T12:52:25.723Z"",
      ""modifiedDate"": ""2016-08-15T12:52:25.723Z"",
      ""createdBy"": ""admin"",
      ""modifiedBy"": ""admin"",
      ""id"": ""d4a4e5271046497eaef61ee47efe6215""
    }
  ]
}

 ";
            #endregion
            return JsonConvert.DeserializeObject<PricelistAssignmentSearchResult>(resultInJSON, new ConditionJsonConverter());
        }
    }
}
