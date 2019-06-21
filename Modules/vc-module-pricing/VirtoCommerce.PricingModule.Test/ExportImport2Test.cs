using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
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
        public async Task TestExport()
        {
            // This is temporary (for debug purposes) full test of prices export

            IKnownExportTypesRegistrar registrar = new KnownExportTypesService();
            registrar.RegisterType<Price>();
            registrar.RegisterType<Pricelist>();
            registrar.RegisterType<PricelistAssignment>();

            var resolver = (IKnownExportTypesResolver)registrar;

            var searchServiceMock = new Mock<IPricingSearchService>();
            searchServiceMock.Setup(x => x.SearchPricesAsync(It.IsAny<PricesSearchCriteria>())).ReturnsAsync(GetTestPriceResult());
            searchServiceMock.Setup(x => x.SearchPricelistsAsync(It.IsAny<PricelistSearchCriteria>())).ReturnsAsync(GetTestPricelistResult());
            searchServiceMock.Setup(x => x.SearchPricelistAssignmentsAsync(It.IsAny<PricelistAssignmentsSearchCriteria>())).ReturnsAsync(GetPricelistAssignmentSearchResult());

            resolver.ResolveExportedTypeDefinition(typeof(Price).FullName)
                .WithDataSourceFactory(dataQuery => new PriceExportPagedDataSource(searchServiceMock.Object) { DataQuery = dataQuery })
                .WithMetadata(ExportedTypeMetadata.GetFromType<Price>());

            resolver.ResolveExportedTypeDefinition(typeof(Pricelist).FullName)
                .WithDataSourceFactory(dataQuery => new PricelistExportPagedDataSource(searchServiceMock.Object) { DataQuery = dataQuery })
                .WithMetadata(ExportedTypeMetadata.GetFromType<Pricelist>());

            resolver.ResolveExportedTypeDefinition(typeof(PricelistAssignment).FullName)
                .WithDataSourceFactory(dataQuery => new PricelistAssignmentExportPagedDataSource(searchServiceMock.Object) { DataQuery = dataQuery })
                .WithMetadata(ExportedTypeMetadata.GetFromType<PricelistAssignment>());

            var exportProviderFactories = new[] {
                new Func<IExportProviderConfiguration, Stream, IExportProvider>((config, stream) => new JsonExportProvider(stream, config)),
                new Func<IExportProviderConfiguration, Stream, IExportProvider>((config, stream) => new CsvExportProvider(stream, config)),
            };

            var dataExporter = new DataExporter(resolver, new ExportProviderFactory(exportProviderFactories));

            using (var ms = new MemoryStream())
            {
                dataExporter.Export(
                    ms,
                    new ExportDataRequest()
                    {
                        DataQuery = new PriceExportDataQuery()
                        {
                            IncludedProperties = new string[] { "Currency", "ProductId", "Sale", "List", "MinQuantity", "StartDate", "EndDate", "EffectiveValue" }
                        },
                        ExportTypeName = typeof(Price).FullName,
                        ProviderName = nameof(CsvExportProvider)
                    },
                    new Action<ExportImportProgressInfo>(x => Console.WriteLine(x.Description)),
                    new CancellationTokenWrapper(CancellationToken.None));

                var resultcsv = Encoding.UTF8.GetString(ms.ToArray());
            }


            using (var ms = new MemoryStream())
            {
                dataExporter.Export(
                    ms,
                    new ExportDataRequest()
                    {
                        DataQuery = new PricelistExportDataQuery()
                        {
                            IncludedProperties = new string[] { "Currency", "Id", "Name" }
                        },
                        ExportTypeName = typeof(Pricelist).FullName,
                        ProviderName = nameof(JsonExportProvider)
                    },
                    new Action<ExportImportProgressInfo>(x => Console.WriteLine(x.Description)),
                    new CancellationTokenWrapper(CancellationToken.None));

                var resultcsv = Encoding.UTF8.GetString(ms.ToArray());
            }


            using (var ms = new MemoryStream())
            {
                dataExporter.Export(
                    ms,
                    new ExportDataRequest()
                    {
                        DataQuery = new PricelistAssignmentExportDataQuery()
                        {
                            IncludedProperties = new string[] { "CatalogId", "PricelistId", "Priority", "Id" }
                        },
                        ExportTypeName = typeof(PricelistAssignment).FullName,
                        ProviderName = nameof(JsonExportProvider)
                    },
                    new Action<ExportImportProgressInfo>(x => Console.WriteLine(x.Description)),
                    new CancellationTokenWrapper(CancellationToken.None));

                var resultcsv = Encoding.UTF8.GetString(ms.ToArray());
            }


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
   ""TotalCount"": 341,
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
      },
      {
         ""PricelistId"": ""0456b3ebc0a24c0ab7054ec9a5cea78e"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""439218e0d5ca4e748cabdd7190e9ccc2"",
         ""Sale"": null,
         ""List"": 4.4,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 4.4,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-04-27T16:52:02.427"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""db0320b1233449cdb741d17963293dc7""
      },
      {
         ""PricelistId"": ""39e18ca8ea254296a78d47f3f90d649d"",
         ""Pricelist"": null,
         ""Currency"": ""EUR"",
         ""ProductId"": ""198d4ad4d5be42aea8d9546885a3bd99"",
         ""Sale"": null,
         ""List"": 4.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 4.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:49.997"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""9322820d75c24c6681cc0f0a0c6c0b99""
      },
      {
         ""PricelistId"": ""6a861ff44eb140bebb516a6dc240868c"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""85e7aa089a4e4a97a4394d668e37e3f8"",
         ""Sale"": null,
         ""List"": 4.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 4.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:50.313"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""cc2dc3e5c5cc427e953e2d307d9473be""
      },
      {
         ""PricelistId"": ""39e18ca8ea254296a78d47f3f90d649d"",
         ""Pricelist"": null,
         ""Currency"": ""EUR"",
         ""ProductId"": ""aa4e28878b8a4c2280010be9e08ad3e1"",
         ""Sale"": null,
         ""List"": 4.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 4.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:49.997"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""54f9a84600fe4b3db99093f4c7e70bc1""
      },
      {
         ""PricelistId"": ""6a861ff44eb140bebb516a6dc240868c"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""f427108e75ed4676923ddc47632111e3"",
         ""Sale"": null,
         ""List"": 5,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 5,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:50.313"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""d0d1e516444646d4a1d0e5a80d13e2ad""
      },
      {
         ""PricelistId"": ""0456b3ebc0a24c0ab7054ec9a5cea78e"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""120bd04d270a42f1b6f490f0cafd4ca7"",
         ""Sale"": null,
         ""List"": 5.4,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 5.4,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-04-27T16:23:49.047"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""ee362d8b16c6455db74637de9e923494""
      },
      {
         ""PricelistId"": ""6a861ff44eb140bebb516a6dc240868c"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""198d4ad4d5be42aea8d9546885a3bd99"",
         ""Sale"": null,
         ""List"": 6.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 6.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:50.313"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""fab38736bcf84d38a1b6d42cfd6ba62b""
      },
      {
         ""PricelistId"": ""39e18ca8ea254296a78d47f3f90d649d"",
         ""Pricelist"": null,
         ""Currency"": ""EUR"",
         ""ProductId"": ""f9507f8ffa9c48eca64807f2fc005ab4"",
         ""Sale"": null,
         ""List"": 6.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 6.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:49.997"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""5ae59dfa8b3f43589831493b09ee48b2""
      },
      {
         ""PricelistId"": ""6a861ff44eb140bebb516a6dc240868c"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""aa4e28878b8a4c2280010be9e08ad3e1"",
         ""Sale"": null,
         ""List"": 6.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 6.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:50.313"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""48d0a165212141f4a1f25b8ab42e05e9""
      },
      {
         ""PricelistId"": ""0456b3ebc0a24c0ab7054ec9a5cea78e"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""24234aafeb0f4dbda72ffd977b32befb"",
         ""Sale"": null,
         ""List"": 7.5,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 7.5,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-04-27T16:13:58.697"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""c75bb35a82d3433a9a0feeb687cde66a""
      },
      {
         ""PricelistId"": ""0456b3ebc0a24c0ab7054ec9a5cea78e"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""cd2f18d2dba742dd832f45c82508f16e"",
         ""Sale"": null,
         ""List"": 8.3,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 8.3,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-04-27T17:53:05.557"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""5b361f4785244674bfe8b9012217f845""
      },
      {
         ""PricelistId"": ""0456b3ebc0a24c0ab7054ec9a5cea78e"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""4f3086bd2526411f9b8d3e2f12e899d1"",
         ""Sale"": null,
         ""List"": 8.85,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 8.85,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-04-27T17:02:11.917"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""74134ebcb9fc4715a031122d95d09d7c""
      },
      {
         ""PricelistId"": ""0456b3ebc0a24c0ab7054ec9a5cea78e"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""74f89c449ad44d52a148123e587212c2"",
         ""Sale"": null,
         ""List"": 8.85,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 8.85,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-04-27T16:46:50.703"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""7cc797218413437d89421a8612e616d4""
      },
      {
         ""PricelistId"": ""6a861ff44eb140bebb516a6dc240868c"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""f9507f8ffa9c48eca64807f2fc005ab4"",
         ""Sale"": null,
         ""List"": 8.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 8.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:50.313"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""a103c2d717eb400885fe23fe7829fdf4""
      },
      {
         ""PricelistId"": ""39e18ca8ea254296a78d47f3f90d649d"",
         ""Pricelist"": null,
         ""Currency"": ""EUR"",
         ""ProductId"": ""fed7b7dd567b4d01a9033d88774c9bdc"",
         ""Sale"": null,
         ""List"": 9,
         ""MinQuantity"": 10,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:49.997"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""c19acffee4fd4e46af3daf052db320f2""
      },
      {
         ""PricelistId"": ""39e18ca8ea254296a78d47f3f90d649d"",
         ""Pricelist"": null,
         ""Currency"": ""EUR"",
         ""ProductId"": ""2747604575ac4c8f901cbcd37022add9"",
         ""Sale"": null,
         ""List"": 9,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:49.997"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""ed670aa0543343abb96ae213ec9992af""
      },
      {
         ""PricelistId"": ""0456b3ebc0a24c0ab7054ec9a5cea78e"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""41ca88f579a747159380efc4e85b56ff"",
         ""Sale"": null,
         ""List"": 9.3,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.3,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-04-27T17:57:41.053"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""e040fd4e8ea44edbadd648e4627847a8""
      },
      {
         ""PricelistId"": ""0456b3ebc0a24c0ab7054ec9a5cea78e"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""6d4f5835f5f8459590da29528b175ff7"",
         ""Sale"": null,
         ""List"": 9.3,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.3,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-04-27T18:09:22.6"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""04f8a097520d4a36969d66498e10f183""
      },
      {
         ""PricelistId"": ""0456b3ebc0a24c0ab7054ec9a5cea78e"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""6a44c625667e4dca85768b5b18428e42"",
         ""Sale"": null,
         ""List"": 9.8,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.8,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-04-27T16:56:38.807"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""bcd61096cc4f47df98cca9ca48389136""
      },
      {
         ""PricelistId"": ""6a861ff44eb140bebb516a6dc240868c"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""fed7b7dd567b4d01a9033d88774c9bdc"",
         ""Sale"": null,
         ""List"": 9.9,
         ""MinQuantity"": 10,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.9,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:50.313"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""81c446ec24154a71860f37822ad68a8e""
      },
      {
         ""PricelistId"": ""6a861ff44eb140bebb516a6dc240868c"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""2747604575ac4c8f901cbcd37022add9"",
         ""Sale"": null,
         ""List"": 9.9,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.9,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2015-10-06T22:39:50.313"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""unknown"",
         ""Id"": ""29751b441376414291dae809689136b3""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""6a44c625667e4dca85768b5b18428e42"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:46:07.83"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""253e0904e276460db970558b15fbbc2a""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""ca255c81f8254865956c1d714f2ef457"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:51:37.867"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""2fe12d947e154be3a3f1d9151424e660""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""14f8279fc25d4e509c017f66f09ff562"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:51:42.49"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""357772aaf2f845c4a87a46c01440f70b""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""fe37ad69f82a4b7d960ffab740409d39"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:51:27.243"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""0a6ac659f0314a81b241af0077acc4e1""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""dae730451bc745bfa642870bdf22f150"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:45:05.853"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""00d2605b6003496a95cc01e32a3aa5c1""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""4f3086bd2526411f9b8d3e2f12e899d1"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:46:02"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""028b605464104234a03eaca282b8a8a3""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""508d2a0584ad4e0e9811577f00b735c8"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:47:07.103"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""0e8ae4ff6af041268c33cc15ca5bf551""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""24cd1f338dfc4d89ad68633932a4225e"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:45:10.967"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""13e033d826604f2081ebbf107ae7d1b9""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""28a40a7733614e45a22a9f2386b1db3e"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:51:33.363"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""163e4ff47b564dbfbbb9051cdef8764f""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""ec235043d51848249e90ef170c371a1c"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:45:22.897"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""717989c438bc4f34b8f38d92d8830c9f""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""e310d2bdb58445cfbd5b5d8cb2002d65"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:52:13.67"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""6b141f966da24e47921694868a2660e5""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""5512e3a5201541769e1d81fc5217490c"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:45:17.993"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""701e175839fe49299d31ed3d8eec2959""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""ac2595d0a2584a6682185d1a7969fca7"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:52:08.47"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""6207192c136d469ab9df4251891a276d""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""3f604bc4a3d147358a4e5e77ae064a2b"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:47:12.447"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""6685f201ff7d4ba3aa9226c1fd5c12be""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""129eec41c8d042e2801f8245fb558fef"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:52:03.493"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""593e7ebeb86e4532bba834b1f185a2b5""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""74f89c449ad44d52a148123e587212c2"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:46:13.883"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""557e65b398424de08a70af30d7c82f37""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""0342f8ee6b3e43828a22dec6f4f0d08e"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:51:57.847"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""50e87d7fc5ce4d22a1def30437638197""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""14664e8c918a4201ba9482465b5bd2f0"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:51:52.383"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""48ccbf6fc40d4e2aaf1131cae2fc0331""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""7e0d0fe29323433b8c6421dc6bf185cf"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:51:47.013"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""3f74b2d3f3d14ecdb01b0ec38b474b7e""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""1c2eaea0a391492ca1045a42d598692e"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:52:22.883"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""807e045a730247e5b37eb47be6f470e4""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""fe9d268c91b84a8fa5dfc51cb28afbb7"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:52:27.287"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""8396aa03e1ec4319a73145dc727a1e60""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""baa4931161214690ad51c50787b1ed94"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:45:27.88"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""8772c20df1a24b6f87db611c2cf102fe""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""3bc23ef43ea14731b0ea08f97e2679ce"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:52:18.133"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""73974bb5f71040b98684204cd793c490""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""0b726c8546e24c2d81edf3cd777e6316"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:47:17.807"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""79ff75129ee4409d8b537cd8790b6ebc""
      },
      {
         ""PricelistId"": ""34efb7152a2b4d018a86878f9a0868bf"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""92a1b7f7705e4448a796cc419492220a"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:46:19.673"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""921ef6aa15ef462ca93a72aa20e24737""
      },
      {
         ""PricelistId"": ""d4c9f26bc63148f18e281fece5644ed3"",
         ""Pricelist"": null,
         ""Currency"": ""USD"",
         ""ProductId"": ""78d65d4ff8904c6393d9887344082270"",
         ""Sale"": null,
         ""List"": 9.99,
         ""MinQuantity"": 1,
         ""StartDate"": null,
         ""EndDate"": null,
         ""EffectiveValue"": 9.99,
         ""CreatedDate"": ""0001-01-01T00:00:00"",
         ""ModifiedDate"": ""2018-05-03T14:52:45.737"",
         ""CreatedBy"": null,
         ""ModifiedBy"": ""admin"",
         ""Id"": ""944d7684c5714f459dce8412d0576641""
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

        private PriceSearchResult GetTestProductPriceResult()
        {
            #region test data string
            var resultInJSON = @"{
   ""totalCount"": 25,
   ""results"": [
      {
         ""productId"": ""69117ea0f37243a08b3441316d40d0e1"",
         ""product"": {
            ""code"": ""ZRS-45331109"",
            ""name"": ""xFold CINEMA X12 RTF U7"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""outline"": ""45d3fc9a913d4610a5c7d0470558c4b2/e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""path"": ""Camcorders/Aerial Imaging & Drones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T12:29:55.9Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/ZRS-45331109/1432147816000_1151519.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ZRS-45331109/1432147816000_1151519.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ZRS-45331109/1432147816000_1151519.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1432147816000_1151519.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""3c8fa608572440248338f35721f08561""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ZRS-45331109/1432314938000_IMG_497697.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ZRS-45331109/1432314938000_IMG_497697.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1432314938000_IMG_497697.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""64ef8a95eedf4c8d8e2f395de295e976""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:28.59Z"",
            ""modifiedDate"": ""2018-06-12T06:16:36.813Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""69117ea0f37243a08b3441316d40d0e1""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""69117ea0f37243a08b3441316d40d0e1"",
               ""list"": 633.09,
               ""minQuantity"": 1,
               ""effectiveValue"": 633.09,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:03:50.063Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""1333ef9ea3b348768fa181450cecce08""
            }
         ]
      },
      {
         ""productId"": ""0f7a77cc1b9a46a29f6a159e5cd49ad1"",
         ""product"": {
            ""code"": ""16785001"",
            ""name"": ""Beats by Dre Solo 2 On-ear Headphones"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""4b50b398ff584af9b6d69c082c94844b"",
            ""outline"": ""4b50b398ff584af9b6d69c082c94844b"",
            ""path"": ""Headphones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-13T15:34:39.55Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/16785001/Beats-by-Dre-Solo-2-On-ear-Headphones-29f63e23-2a63-4c7a-bf44-99615e2b80bb.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/16785001/Beats-by-Dre-Solo-2-On-ear-Headphones-29f63e23-2a63-4c7a-bf44-99615e2b80bb.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/16785001/Beats-by-Dre-Solo-2-On-ear-Headphones-29f63e23-2a63-4c7a-bf44-99615e2b80bb.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dre-Solo-2-On-ear-Headphones-29f63e23-2a63-4c7a-bf44-99615e2b80bb.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""2d79af4d083c46189d44278aa26e47a6""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/16785001/Beats-by-Dre-Solo-2-On-ear-Headphones-7e91558b-ca88-443f-97b0-bca06a6f1acc.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/16785001/Beats-by-Dre-Solo-2-On-ear-Headphones-7e91558b-ca88-443f-97b0-bca06a6f1acc.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dre-Solo-2-On-ear-Headphones-7e91558b-ca88-443f-97b0-bca06a6f1acc.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""8dd9e3fa9c184dfd8a588d37e5ef1a6f""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:33.713Z"",
            ""modifiedDate"": ""2018-06-12T06:16:34.89Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""0f7a77cc1b9a46a29f6a159e5cd49ad1""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""0f7a77cc1b9a46a29f6a159e5cd49ad1"",
               ""list"": 33.55,
               ""minQuantity"": 1,
               ""effectiveValue"": 33.55,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:05:46.74Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""2407d650d5ed4052a1ed04ecf4d5c045""
            }
         ]
      },
      {
         ""productId"": ""7c835a9b1c8e4445aa118dae659231c3"",
         ""product"": {
            ""code"": ""SAG920F32GBB"",
            ""name"": ""Samsung Galaxy S6 SM-G920F 32GB"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""outline"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""path"": ""Cell phones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""vendor"": ""94597fe09f634914a5fdf76c6ba86b04"",
            ""startDate"": ""2015-08-13T13:25:33.207Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/SAG920F32GBB/1431530477000_1147139.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SAG920F32GBB/1431530477000_1147139.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SAG920F32GBB/1431530477000_1147139.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431530477000_1147139.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""d0bdb21fa3a54c019d7a60d5471238f2""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SAG920F32GBB/1433874697000_IMG_502816.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SAG920F32GBB/1433874697000_IMG_502816.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1433874697000_IMG_502816.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""29b3ab72a057474f94cc99798048dddd""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:29.02Z"",
            ""modifiedDate"": ""2018-06-12T06:16:36.813Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""7c835a9b1c8e4445aa118dae659231c3""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""7c835a9b1c8e4445aa118dae659231c3"",
               ""list"": 234,
               ""minQuantity"": 1,
               ""effectiveValue"": 234,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:05:14.637Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""254f12c9cc254e9da6e94f4a1246836f""
            }
         ]
      },
      {
         ""productId"": ""ac8da6c50cef42ad97d6f1effe2abaee"",
         ""product"": {
            ""code"": ""EFL10450"",
            ""name"": ""E-flite Carbon-Z Cub BNF Basic"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""outline"": ""45d3fc9a913d4610a5c7d0470558c4b2/e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""path"": ""Camcorders/Aerial Imaging & Drones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T12:35:39.267Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/EFL10450/1393539874000_1033301.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/EFL10450/1393539874000_1033301.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/EFL10450/1393539874000_1033301.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1393539874000_1033301.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""9864cc0c50d146678524eb8e55fb3a63""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:29.523Z"",
            ""modifiedDate"": ""2018-06-12T06:16:35.69Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""ac8da6c50cef42ad97d6f1effe2abaee""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""ac8da6c50cef42ad97d6f1effe2abaee"",
               ""list"": 422.66,
               ""minQuantity"": 1,
               ""effectiveValue"": 422.66,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:03:50.063Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""2a4a07ebbd2d466184a3b3aa1a898e3a""
            }
         ]
      },
      {
         ""productId"": ""f9330eb5ed78427abb4dc4089bc37d9f"",
         ""product"": {
            ""code"": ""ASZF264GBSL"",
            ""name"": ""ASUS ZenFone 2 ZE551ML 64GB Smartphone"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""outline"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""path"": ""Cell phones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-13T10:30:09.897Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431971520000_1134360.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431971520000_1134360.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431971520000_1134360.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431971520000_1134360.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""0d66631807414c8183993675eee6fca9""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484296.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484296.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484296.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""c05cda7d50f848e28f5e9003623fe73f""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484297.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484297.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484297.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""7acee96fe30a45eab751647de566f606""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484298.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484298.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484298.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""7a8d41ee86a84c52bef114a2e45fb5a8""
               },
               {
                  ""sortOrder"": 4,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484299.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484299.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484299.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""491089240f2b4ecf983271901057b72a""
               },
               {
                  ""sortOrder"": 5,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484300.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484300.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484300.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""ea02a99da7c840319eb848fc59483191""
               },
               {
                  ""sortOrder"": 6,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484301.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484301.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484301.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""83d6027fc66b4437ab15578b44f4bf14""
               },
               {
                  ""sortOrder"": 7,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484302.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484302.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484302.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""350e7f88f9bc4230a2da685293364d51""
               },
               {
                  ""sortOrder"": 8,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484303.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484303.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484303.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""e0467bc7316c472d9e6697e591c83d0f""
               },
               {
                  ""sortOrder"": 9,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484304.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484304.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484304.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""793ddd8038df48dba82abf8a258671f6""
               },
               {
                  ""sortOrder"": 10,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484305.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484305.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484305.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""91e3e90661304baf9a65fec664b753f3""
               },
               {
                  ""sortOrder"": 11,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484306.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484306.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484306.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""6d1f9afe2f89472e9f670106aeb246e0""
               },
               {
                  ""sortOrder"": 12,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484307.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484307.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484307.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""c66650085b84439c954033401e0bbfaa""
               },
               {
                  ""sortOrder"": 13,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484308.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484308.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484308.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""7c56c77db6624e1580d23df7883ad5f9""
               },
               {
                  ""sortOrder"": 14,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484309.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484309.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484309.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""19aad10b24764600b46e3ce594356e85""
               },
               {
                  ""sortOrder"": 15,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484310.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484310.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484310.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""d154dac4e44c4d5281728d1f2d2d3b6e""
               },
               {
                  ""sortOrder"": 16,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484311.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484311.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484311.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""67474e43435148f9a48f77a9f0999041""
               },
               {
                  ""sortOrder"": 17,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484312.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484312.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484312.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""4e42d2c732a141969f1bfa13861dd9ba""
               },
               {
                  ""sortOrder"": 18,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484313.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484313.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484313.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""c532bd7e866c49d6889a1012e7de44d8""
               },
               {
                  ""sortOrder"": 19,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484314.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484314.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484314.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""e8cd97822e8e49458669d07927d0d54d""
               },
               {
                  ""sortOrder"": 20,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484315.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF264GBSL/1431973396000_IMG_484315.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484315.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""3c6d41245e7047d9a67a4846334d7949""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:34.987Z"",
            ""modifiedDate"": ""2018-06-12T06:16:34.89Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""f9330eb5ed78427abb4dc4089bc37d9f""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""f9330eb5ed78427abb4dc4089bc37d9f"",
               ""list"": 44,
               ""minQuantity"": 1,
               ""effectiveValue"": 44,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:05:14.637Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""2c742fa6e4774c2da762dd018899de26""
            }
         ]
      },
      {
         ""productId"": ""b4a2edc523db4114a5e8ccbff5747667"",
         ""product"": {
            ""code"": ""PAHCVX870"",
            ""name"": ""Panasonic HC-VX870K 4K Ultra HD Camcorder"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""53e239451c844442a3b2fe9aa82d95c8"",
            ""outline"": ""45d3fc9a913d4610a5c7d0470558c4b2/53e239451c844442a3b2fe9aa82d95c8"",
            ""path"": ""Camcorders/Consumer camcorders"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T13:07:10.46Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483149000_1109406.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483149000_1109406.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483149000_1109406.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1420483149000_1109406.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""9ecae97228b84b18a8a1acc383950946""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451986.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451986.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1420483204000_IMG_451986.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""f38526209b20453eb95ca8d4bf4900bf""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451987.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451987.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1420483204000_IMG_451987.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""7a57498062e14553b66f90f173616ca9""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451988.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451988.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1420483204000_IMG_451988.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""ad76c13a5e634f6887e2e2ec91efa02a""
               },
               {
                  ""sortOrder"": 4,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451989.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451989.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1420483204000_IMG_451989.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""5ffc834e60b341cea08d59e0ec49dc49""
               },
               {
                  ""sortOrder"": 5,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451990.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451990.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1420483204000_IMG_451990.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""a4e60184243946d994e1ef0ff503c6aa""
               },
               {
                  ""sortOrder"": 6,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451991.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451991.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1420483204000_IMG_451991.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""040da2ceefa440319c8961d26fa10ee5""
               },
               {
                  ""sortOrder"": 7,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451992.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451992.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1420483204000_IMG_451992.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""2667fd27345c4592bd47ca84b77b05c6""
               },
               {
                  ""sortOrder"": 8,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451993.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451993.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1420483204000_IMG_451993.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""c4db43c8a4944aaca6464158837ab2fc""
               },
               {
                  ""sortOrder"": 9,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451994.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAHCVX870/1420483204000_IMG_451994.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1420483204000_IMG_451994.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""8bd47e7283eb4849996318263347856d""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:29.843Z"",
            ""modifiedDate"": ""2018-06-12T06:16:36.813Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""b4a2edc523db4114a5e8ccbff5747667""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""b4a2edc523db4114a5e8ccbff5747667"",
               ""list"": 333,
               ""minQuantity"": 1,
               ""effectiveValue"": 333,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:04:31.423Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""2eb194b999ef42ab8fcbe336d93916bf""
            }
         ]
      },
      {
         ""productId"": ""143e3eb3d1ee4a2bbf8fa0ecacfd1222"",
         ""product"": {
            ""code"": ""17070626"",
            ""name"": ""Beats by Dre Studio Red Over-ear Active Noise Canceling"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""4b50b398ff584af9b6d69c082c94844b"",
            ""outline"": ""4b50b398ff584af9b6d69c082c94844b"",
            ""path"": ""Headphones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-13T15:48:16.503Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/17070626/Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-64462ddb-48b2-4790-8adb-2486e87074bb.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/17070626/Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-64462ddb-48b2-4790-8adb-2486e87074bb.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/17070626/Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-64462ddb-48b2-4790-8adb-2486e87074bb.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-64462ddb-48b2-4790-8adb-2486e87074bb.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""54ce6d509b4b4ff5997b62016fbc87ca""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/17070626/Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-bcbe2323-9907-4c60-abcc-ffe4a81de39b.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/17070626/Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-bcbe2323-9907-4c60-abcc-ffe4a81de39b.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-bcbe2323-9907-4c60-abcc-ffe4a81de39b.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""4cda1d4a36b04233b69ff67b790da689""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/17070626/Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-07e85bc9-2078-45c5-8552-801d0f90e286.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/17070626/Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-07e85bc9-2078-45c5-8552-801d0f90e286.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-07e85bc9-2078-45c5-8552-801d0f90e286.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""72f4a12b1e6b4b2a9ec23842b30f0baa""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/17070626/Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-16662c59-1eff-4ae1-a794-88bd989437ab.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/17070626/Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-16662c59-1eff-4ae1-a794-88bd989437ab.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dr.-Dre-Studio-High-Definition-Active-Noise-Canceling-Headphones-Red-16662c59-1eff-4ae1-a794-88bd989437ab.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""e9f6961bbd254a41a6f0a754da74e54f""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:33.87Z"",
            ""modifiedDate"": ""2018-06-12T06:16:34.89Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""143e3eb3d1ee4a2bbf8fa0ecacfd1222""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""143e3eb3d1ee4a2bbf8fa0ecacfd1222"",
               ""sale"": 22,
               ""list"": 22,
               ""minQuantity"": 1,
               ""effectiveValue"": 22,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:05:46.74Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""367d31ed23544b3fba15c182211363d0""
            }
         ]
      },
      {
         ""productId"": ""8db64bd60a354c4c96e25e61d7361565"",
         ""product"": {
            ""code"": ""LG65EG9600"",
            ""name"": ""LG EG9600 Series 65\""-Class 4K Smart Curved OLED 3D TV"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""44a33af73d1042aca0c8cee8b5120bd0"",
            ""outline"": ""c76774f9047d4f18a916b38681c50557/44a33af73d1042aca0c8cee8b5120bd0"",
            ""path"": ""Televisions/Lg"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T08:33:07.247Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433168408000_1119811.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433168408000_1119811.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433168408000_1119811.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1433168408000_1119811.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""842a6c521fdc469686fb140a4847722e""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499708.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499708.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1433169097000_IMG_499708.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""a6784a70bc404589862eb2d0acf25962""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499709.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499709.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1433169097000_IMG_499709.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""319e9b6d1aca44a2badd55a01d78c839""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499710.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499710.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1433169097000_IMG_499710.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""e9fdeaccab7945d1a12424008074e43c""
               },
               {
                  ""sortOrder"": 4,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499711.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499711.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1433169097000_IMG_499711.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""79b74fbc13da4a06a68ad5cb43c71f4f""
               },
               {
                  ""sortOrder"": 5,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499712.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499712.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1433169097000_IMG_499712.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""227717b35e86486d9eecd4446f6d213c""
               },
               {
                  ""sortOrder"": 6,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499713.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499713.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1433169097000_IMG_499713.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""2cffac9661b54a3fb17eee37977e177f""
               },
               {
                  ""sortOrder"": 7,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499714.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499714.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1433169097000_IMG_499714.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""f53c5926db1148238ba8f98045452654""
               },
               {
                  ""sortOrder"": 8,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499715.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG65EG9600/1433169097000_IMG_499715.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1433169097000_IMG_499715.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""b94102a098e642c687865562737d1402""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:29.17Z"",
            ""modifiedDate"": ""2018-06-12T06:16:35.69Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""8db64bd60a354c4c96e25e61d7361565""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""8db64bd60a354c4c96e25e61d7361565"",
               ""list"": 666,
               ""minQuantity"": 1,
               ""effectiveValue"": 666,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:06:54.84Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""4c403dbddaa6427297019ac5a2af854f""
            }
         ]
      },
      {
         ""productId"": ""e7eee66223da43109502891b54bc33d3"",
         ""product"": {
            ""code"": ""DJS900SWOCWK"",
            ""name"": ""3DR X8-M Octocopter for Visual-Spectrum Aerial Maps (915 MHz)"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""outline"": ""45d3fc9a913d4610a5c7d0470558c4b2/e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""path"": ""Camcorders/Aerial Imaging & Drones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T12:19:09.723Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/DJS900SWOCWK/1419605414000_1106906.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/DJS900SWOCWK/1419605414000_1106906.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/DJS900SWOCWK/1419605414000_1106906.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1419605414000_1106906.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""a4cfdae12cba423a9c47e32854fb742c""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:34.907Z"",
            ""modifiedDate"": ""2018-06-12T06:16:34.89Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""e7eee66223da43109502891b54bc33d3""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""e7eee66223da43109502891b54bc33d3"",
               ""sale"": 55.66,
               ""list"": 55.66,
               ""minQuantity"": 1,
               ""effectiveValue"": 55.66,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:02:04.47Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""5227cc094eb345c5bb85af6b013f1aaa""
            }
         ]
      },
      {
         ""productId"": ""b7fbcb4e4efb4b1bbe79482a20e80a3d"",
         ""product"": {
            ""code"": ""PAPF724000"",
            ""name"": ""Parrot Jumping Sumo MiniDrone"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""outline"": ""45d3fc9a913d4610a5c7d0470558c4b2/e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""path"": ""Camcorders/Aerial Imaging & Drones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T12:42:44.007Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177916000_1073346.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177916000_1073346.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177916000_1073346.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177916000_1073346.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""74cf136ec4bd48a0b7bff7d8592b38b5""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411928.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411928.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411928.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""7cbe08ad538e48c2bff66175d124ac9e""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411929.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411929.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411929.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""1e98f1363d3045a791d0844f5efe7e82""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411930.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411930.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411930.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""b9e00f98f0eb49a09aa25ed04a9b3591""
               },
               {
                  ""sortOrder"": 4,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411931.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411931.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411931.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""03d9189496334b0fb43b2730c032c77b""
               },
               {
                  ""sortOrder"": 5,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411932.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411932.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411932.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""38c72d3c72cb4ab7bebf3f8c84f9867d""
               },
               {
                  ""sortOrder"": 6,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411933.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411933.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411933.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""fceaeca63cde4e51b95417f8814e7df7""
               },
               {
                  ""sortOrder"": 7,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411934.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411934.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411934.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""4d0f6a272cfb4753a11c957d5d92482e""
               },
               {
                  ""sortOrder"": 8,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411935.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411935.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411935.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""6e57220d1afe452d8010ad42c4ffdd83""
               },
               {
                  ""sortOrder"": 9,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411936.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411936.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411936.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""fda0d59c94864e5cb9adcf5c10b5c903""
               },
               {
                  ""sortOrder"": 10,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411937.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411937.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411937.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""97bfcfab43484eb2a4bd98b3b073288f""
               },
               {
                  ""sortOrder"": 11,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411938.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411938.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411938.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""f3e996cd2c214a65aca297d5eecb975b""
               },
               {
                  ""sortOrder"": 12,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411939.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/PAPF724000/1407177972000_IMG_411939.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1407177972000_IMG_411939.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""27b7ef853e6e4bc2873b4d95b92a1d9b""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:29.93Z"",
            ""modifiedDate"": ""2018-06-12T06:16:36.813Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""b7fbcb4e4efb4b1bbe79482a20e80a3d""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""b7fbcb4e4efb4b1bbe79482a20e80a3d"",
               ""list"": 876.99,
               ""minQuantity"": 1,
               ""effectiveValue"": 876.99,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:03:50.063Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""736a924742e64a94a648676ea328333d""
            }
         ]
      },
      {
         ""productId"": ""9cbd8f316e254a679ba34a900fccb076"",
         ""product"": {
            ""code"": ""3DRSOLO"",
            ""name"": ""3DR Solo Quadcopter (No Gimbal)"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""outline"": ""45d3fc9a913d4610a5c7d0470558c4b2/e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""path"": ""Camcorders/Aerial Imaging & Drones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T10:41:45.183Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428965138000_1133723.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428965138000_1133723.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428965138000_1133723.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1428965138000_1133723.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""6793ad64afb24a31926bafe709afc258""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428963815000_IMG_481913.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428963815000_IMG_481913.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1428963815000_IMG_481913.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""da4f44e7212e45c096ca7987c855cc6a""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428963815000_IMG_484935.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428963815000_IMG_484935.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1428963815000_IMG_484935.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""b7bd0b1c2e744b448efd6da1315ce682""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428963815000_IMG_484936.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428963815000_IMG_484936.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1428963815000_IMG_484936.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""0b1345f35ab3489fa8b1c1fc454291b1""
               },
               {
                  ""sortOrder"": 4,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428964877000_IMG_485455.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428964877000_IMG_485455.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1428964877000_IMG_485455.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""ecaa0dbbb66d46c6a20b5ec49c24652a""
               },
               {
                  ""sortOrder"": 5,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428964877000_IMG_485456.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428964877000_IMG_485456.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1428964877000_IMG_485456.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""ad5e5d7ba15841b69bf596cfd30a046c""
               },
               {
                  ""sortOrder"": 6,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428964877000_IMG_485457.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428964877000_IMG_485457.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1428964877000_IMG_485457.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""eabf778c8a794d4aa676bc5233f4f707""
               },
               {
                  ""sortOrder"": 7,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428964877000_IMG_485458.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/3DRSOLO/1428964877000_IMG_485458.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1428964877000_IMG_485458.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""4c61423e979743fbb4b9ead55fd6949c""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:34.657Z"",
            ""modifiedDate"": ""2018-06-12T06:16:34.89Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""9cbd8f316e254a679ba34a900fccb076""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""9cbd8f316e254a679ba34a900fccb076"",
               ""sale"": 12,
               ""list"": 33,
               ""minQuantity"": 1,
               ""effectiveValue"": 12,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:02:04.47Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""78de10e47aff49458a152118422197d4""
            }
         ]
      },
      {
         ""productId"": ""f1b26974b7634abaa0900e575a99476f"",
         ""product"": {
            ""code"": ""LG55EG9600"",
            ""name"": ""LG EG9600 Series 55\""-Class 4K Smart Curved OLED 3D TV"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""44a33af73d1042aca0c8cee8b5120bd0"",
            ""outline"": ""c76774f9047d4f18a916b38681c50557/44a33af73d1042aca0c8cee8b5120bd0"",
            ""path"": ""Televisions/Lg"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T08:38:40.437Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446771000_1119832.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446771000_1119832.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446771000_1119832.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431446771000_1119832.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""a40b05e231ba4be0893bd4bbcfb92376""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494510.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494510.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431446523000_IMG_494510.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""cf94b46a173f4abbba05ec95daa7667a""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494511.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494511.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431446523000_IMG_494511.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""ba078fbf6c154d7abd5815e1fe39e9e3""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494512.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494512.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431446523000_IMG_494512.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""1c690c8ba7df40e0be88f97526714cca""
               },
               {
                  ""sortOrder"": 4,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494513.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494513.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431446523000_IMG_494513.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""eec7e530573648f3b0fd0c7e0b913c04""
               },
               {
                  ""sortOrder"": 5,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494514.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494514.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431446523000_IMG_494514.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""38d1205400114cb786222fb06c08adf6""
               },
               {
                  ""sortOrder"": 6,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494515.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494515.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431446523000_IMG_494515.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""878ba0c4ae9044c795a6d781ef669915""
               },
               {
                  ""sortOrder"": 7,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494516.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494516.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431446523000_IMG_494516.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""6cf7574adeed4abca3047d98e639806c""
               },
               {
                  ""sortOrder"": 8,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494517.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494517.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431446523000_IMG_494517.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""5f6c25cb296c4fbc8d8a43a6cebe56c8""
               },
               {
                  ""sortOrder"": 9,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494526.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/LG55EG9600/1431446523000_IMG_494526.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431446523000_IMG_494526.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""175d1421ebe84d1dba6639eb740d7b6c""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:30.573Z"",
            ""modifiedDate"": ""2018-06-12T06:16:35.69Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""f1b26974b7634abaa0900e575a99476f""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""f1b26974b7634abaa0900e575a99476f"",
               ""list"": 32,
               ""minQuantity"": 1,
               ""effectiveValue"": 32,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:06:54.84Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""7f77f4cbdbfa48a4a57be2a083f28240""
            }
         ]
      },
      {
         ""productId"": ""1486f5a1a25f48a999189c081792a379"",
         ""product"": {
            ""code"": ""MIL640X4GLWH"",
            ""name"": ""Microsoft Lumia 640 XL RM-1065 8GB Dual SIM"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""outline"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""path"": ""Cell phones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-13T14:43:51.227Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/MIL640X4GLWH/1432753636000_1148740.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/MIL640X4GLWH/1432753636000_1148740.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/MIL640X4GLWH/1432753636000_1148740.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1432753636000_1148740.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""51ad6baa26c940a58336d59e308c8c0c""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:27.457Z"",
            ""modifiedDate"": ""2018-06-12T06:16:35.69Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""1486f5a1a25f48a999189c081792a379""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""1486f5a1a25f48a999189c081792a379"",
               ""list"": 666,
               ""minQuantity"": 1,
               ""effectiveValue"": 666,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:05:14.637Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""8acbc6af22364f3cb628781101a779e3""
            }
         ]
      },
      {
         ""productId"": ""d154d30d76d548fb8505f5124d18c1f3"",
         ""product"": {
            ""code"": ""BLX150QWHT"",
            ""name"": ""BLU Win HD LTE X150Q 8GB"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""outline"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""path"": ""Cell phones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-13T14:56:36.07Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/BLX150QWHT/1435269990000_1163371.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/BLX150QWHT/1435269990000_1163371.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/BLX150QWHT/1435269990000_1163371.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1435269990000_1163371.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""9068b98eae4b46a4be570ce539480387""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/BLX150QWHT/1435269697000_IMG_508203.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/BLX150QWHT/1435269697000_IMG_508203.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1435269697000_IMG_508203.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""0ef87af983de43838762b209775a35b4""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/BLX150QWHT/1435269697000_IMG_508204.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/BLX150QWHT/1435269697000_IMG_508204.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1435269697000_IMG_508204.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""b685461dc54247bf80e5def0f622cb4a""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/BLX150QWHT/1435269697000_IMG_508205.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/BLX150QWHT/1435269697000_IMG_508205.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1435269697000_IMG_508205.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""ae0eb5088b134eaa86792c7ac84bf70d""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:34.747Z"",
            ""modifiedDate"": ""2018-06-12T06:16:34.89Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""d154d30d76d548fb8505f5124d18c1f3""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""d154d30d76d548fb8505f5124d18c1f3"",
               ""list"": 655,
               ""minQuantity"": 1,
               ""effectiveValue"": 655,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:05:14.637Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""9addd5d84cb2481da0521369bc5e58e3""
            }
         ]
      },
      {
         ""productId"": ""6e7a31c35c814fb389dc2574aa142b63"",
         ""product"": {
            ""code"": ""16993868"",
            ""name"": ""Beats by Dre Powerbeats 2 In-ear Bluetooth Wireless Sport"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""4b50b398ff584af9b6d69c082c94844b"",
            ""outline"": ""4b50b398ff584af9b6d69c082c94844b"",
            ""path"": ""Headphones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-13T15:19:51.59Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-f216d237-779c-48c5-82a8-51a0de248169.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-f216d237-779c-48c5-82a8-51a0de248169.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-f216d237-779c-48c5-82a8-51a0de248169.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-f216d237-779c-48c5-82a8-51a0de248169.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""aff7b34b89c44aa497dd1b2425e4936d""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-2cedce6d-d2ed-4f10-ab4a-ae452dad20d5.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-2cedce6d-d2ed-4f10-ab4a-ae452dad20d5.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-2cedce6d-d2ed-4f10-ab4a-ae452dad20d5.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""2c634d8d361e4feabc80efff5bf9d65a""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-6cc83c0b-9f2f-4d9e-875b-e4b0724400bc.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-6cc83c0b-9f2f-4d9e-875b-e4b0724400bc.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-6cc83c0b-9f2f-4d9e-875b-e4b0724400bc.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""931d8481bf4e4a27b5577376a50ea114""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-9feb5aa3-d394-4676-b911-690fcae7d8e2.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-9feb5aa3-d394-4676-b911-690fcae7d8e2.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-9feb5aa3-d394-4676-b911-690fcae7d8e2.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""3980caade9734e5b9c71699a4cb5ca92""
               },
               {
                  ""sortOrder"": 4,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-964ae2e2-4b9b-4b1e-a150-b01b95f6be99.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-964ae2e2-4b9b-4b1e-a150-b01b95f6be99.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-964ae2e2-4b9b-4b1e-a150-b01b95f6be99.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""83934b238ba34ccb9a33aaafcedd36f2""
               },
               {
                  ""sortOrder"": 5,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-e712b2fd-f8ad-49d4-a697-bf3eb37e37d6.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/16993868/Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-e712b2fd-f8ad-49d4-a697-bf3eb37e37d6.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""Beats-by-Dre-Powerbeats-2-In-ear-Bluetooth-Wireless-Sport-Headphones-e712b2fd-f8ad-49d4-a697-bf3eb37e37d6.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""73788d1a001a40de9fd7cf61209680ee""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:33.943Z"",
            ""modifiedDate"": ""2018-06-12T06:16:34.89Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""6e7a31c35c814fb389dc2574aa142b63""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""6e7a31c35c814fb389dc2574aa142b63"",
               ""list"": 22.55,
               ""minQuantity"": 1,
               ""effectiveValue"": 22.55,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:05:46.74Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""9c338bd6df8b4f9892db4fc8ab3ef260""
            }
         ]
      },
      {
         ""productId"": ""8b7b07c165924a879392f4f51a6f7ce0"",
         ""product"": {
            ""code"": ""ASZF216GBSL"",
            ""name"": ""ASUS ZenFone 2 ZE551ML 16GB Smartphone"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""outline"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""path"": ""Cell phones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-13T13:14:35.867Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431971520000_1134360.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431971520000_1134360.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431971520000_1134360.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431971520000_1134360.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""cb3ead0bd85b4dddb486a953dfb830c2""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484296.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484296.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484296.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""b220c8d895f845f0aec04157836ba7d2""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484297.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484297.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484297.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""11f01ca2045d4a6bb6515f2a47881318""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484298.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484298.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484298.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""433a70580b994d62a5eaeebbfb37c4f2""
               },
               {
                  ""sortOrder"": 4,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484299.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484299.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484299.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""bbfa1dce3c2d4db88f7af2011784e85d""
               },
               {
                  ""sortOrder"": 5,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484300.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484300.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484300.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""3e6cab47886047349ecfcb2a3b121996""
               },
               {
                  ""sortOrder"": 6,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484301.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484301.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484301.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""1b632b337e5149f4979b8c315ebd33b9""
               },
               {
                  ""sortOrder"": 7,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484302.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484302.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484302.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""dd0b4e1e6f5d423495b337c311bae16e""
               },
               {
                  ""sortOrder"": 8,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484303.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484303.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484303.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""7b619b0c5f9d4fe8aa3023b1a82308ac""
               },
               {
                  ""sortOrder"": 9,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484304.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484304.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484304.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""ec41d05db1424f5fb7401555aa1cd3e5""
               },
               {
                  ""sortOrder"": 10,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484305.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484305.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484305.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""29b9e69642b84a1fa31745aebb6d206d""
               },
               {
                  ""sortOrder"": 11,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484306.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484306.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484306.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""62e9e8f8298545ef9eb7d0790d11eff8""
               },
               {
                  ""sortOrder"": 12,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484307.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484307.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484307.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""2234844cbeaf45059e4ee9f6cc0b5daf""
               },
               {
                  ""sortOrder"": 13,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484308.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484308.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484308.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""14c657fb69c34655be80da71ea83abfe""
               },
               {
                  ""sortOrder"": 14,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484309.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484309.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484309.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""cda85cf67baf4f21987ed992b1c12e80""
               },
               {
                  ""sortOrder"": 15,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484310.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484310.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484310.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""56203ea6d74d4074a5db7925cda9a185""
               },
               {
                  ""sortOrder"": 16,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484311.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484311.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484311.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""492f5efab67741f9a082bd1e8d1ceb12""
               },
               {
                  ""sortOrder"": 17,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484312.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484312.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484312.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""cc42370f7ed04d1e81c5d63c89851416""
               },
               {
                  ""sortOrder"": 18,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484313.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484313.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484313.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""bb5f45c1b4484c0bbff7778ab5e292a3""
               },
               {
                  ""sortOrder"": 19,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484314.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484314.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484314.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""0765c63c852542eca41da82e3552d76d""
               },
               {
                  ""sortOrder"": 20,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484315.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ASZF216GBSL/1431973396000_IMG_484315.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1431973396000_IMG_484315.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""00270f9952dc4cfdb9049ac44f77ba41""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:34.097Z"",
            ""modifiedDate"": ""2018-06-12T06:16:34.89Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""8b7b07c165924a879392f4f51a6f7ce0""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""8b7b07c165924a879392f4f51a6f7ce0"",
               ""list"": 112,
               ""minQuantity"": 1,
               ""effectiveValue"": 112,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:05:14.637Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""9e644c0d0216465c94bd47df7a7967bb""
            }
         ]
      },
      {
         ""productId"": ""7384ecc9cba84f2eb755c5136736bb9f"",
         ""product"": {
            ""code"": ""WATALIH500"",
            ""name"": ""Walkera TALI H500 Hexacopter"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""outline"": ""45d3fc9a913d4610a5c7d0470558c4b2/e51b5f9eea094a44939c11d4d4fa3bb1"",
            ""path"": ""Camcorders/Aerial Imaging & Drones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T12:08:56.093Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218859000_1087071.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218859000_1087071.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218859000_1087071.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218859000_1087071.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""dc81d51075e34c51a451df5e9aa46825""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436598.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436598.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436598.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""5b8bf33963a24f9bb6e9e824a77181b7""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436602.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436602.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436602.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""11ffefc9884345908e8124a76dae5f76""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436603.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436603.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436603.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""b339d1c4f83543db8933f3eb84aadcb4""
               },
               {
                  ""sortOrder"": 4,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436604.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436604.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436604.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""c344762b7bff4bbc8e53607666fcc65c""
               },
               {
                  ""sortOrder"": 5,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436605.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436605.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436605.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""63371171939e48af9df6306688da626b""
               },
               {
                  ""sortOrder"": 6,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436606.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436606.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436606.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""942cbb223c8446d395632fbbbec6ccbf""
               },
               {
                  ""sortOrder"": 7,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436607.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436607.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436607.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""7cb48e8aaa61400e92596d293f9f5ea6""
               },
               {
                  ""sortOrder"": 8,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436608.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436608.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436608.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""cc258485748c4ff1aaf0a3c17f356281""
               },
               {
                  ""sortOrder"": 9,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436609.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436609.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436609.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""573d10cac2c640d4a72a4704c452555f""
               },
               {
                  ""sortOrder"": 10,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436610.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436610.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436610.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""207e3052718a42d6ae1f3bc7aafad7fd""
               },
               {
                  ""sortOrder"": 11,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436611.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436611.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436611.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""64fd3c97b5c743268e265f07a1d1f27d""
               },
               {
                  ""sortOrder"": 12,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436612.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436612.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436612.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""8a64701885b043c5b57f6abf7b49530a""
               },
               {
                  ""sortOrder"": 13,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436613.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436613.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436613.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""a6e445a18d08430fb5289c622a08ed0e""
               },
               {
                  ""sortOrder"": 14,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436614.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436614.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436614.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""d014634a2c7c4a959e192db2a0745146""
               },
               {
                  ""sortOrder"": 15,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436615.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436615.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436615.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""6dbac5dec82c486a9f1e08eabeb0bce1""
               },
               {
                  ""sortOrder"": 16,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436616.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415218777000_IMG_436616.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415218777000_IMG_436616.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""be553ea1ba1f4d998d0ff1774e817f95""
               },
               {
                  ""sortOrder"": 17,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436592.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436592.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415373315000_IMG_436592.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""0203c9e6095a424e92b3313d56a8c782""
               },
               {
                  ""sortOrder"": 18,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436593.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436593.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415373315000_IMG_436593.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""58965eecbe894dc7bb7ad48bf25241d5""
               },
               {
                  ""sortOrder"": 19,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436594.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436594.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415373315000_IMG_436594.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""818d8517f5b045b39375daeae707bc3f""
               },
               {
                  ""sortOrder"": 20,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436595.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436595.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415373315000_IMG_436595.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""684a114c7cb7463e90a1ff74f5a686e2""
               },
               {
                  ""sortOrder"": 21,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436596.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436596.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415373315000_IMG_436596.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""630851c2e1e3496892033aae8f1fafe0""
               },
               {
                  ""sortOrder"": 22,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436597.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373315000_IMG_436597.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415373315000_IMG_436597.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""06b6c64d61754bbc8064562cae11d651""
               },
               {
                  ""sortOrder"": 23,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373316000_IMG_436599.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373316000_IMG_436599.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415373316000_IMG_436599.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""2aa69ec4d7584a8f950abd601a3c54f2""
               },
               {
                  ""sortOrder"": 24,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373316000_IMG_436600.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373316000_IMG_436600.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415373316000_IMG_436600.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""69d347598cf24b7ca69e43183eff729d""
               },
               {
                  ""sortOrder"": 25,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373316000_IMG_436601.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/WATALIH500/1415373316000_IMG_436601.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1415373316000_IMG_436601.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""dae1cce4b5634db6982ab5ac56413a69""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:28.757Z"",
            ""modifiedDate"": ""2018-06-12T06:16:36.813Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""7384ecc9cba84f2eb755c5136736bb9f""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""7384ecc9cba84f2eb755c5136736bb9f"",
               ""list"": 788.88,
               ""minQuantity"": 1,
               ""effectiveValue"": 788.88,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:03:50.063Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""a0f9f8d0d6d5407d99ebab8c89823c26""
            }
         ]
      },
      {
         ""productId"": ""f0133829838f4b1e8019ff045bb31cd7"",
         ""product"": {
            ""code"": ""SOFDRAX100"",
            ""name"": ""Sony FDR-AX100 4K Ultra HD Camcorder"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""53e239451c844442a3b2fe9aa82d95c8"",
            ""outline"": ""45d3fc9a913d4610a5c7d0470558c4b2/53e239451c844442a3b2fe9aa82d95c8"",
            ""path"": ""Camcorders/Consumer camcorders"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T13:00:40.967Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360504.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360504.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360504.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198276000_IMG_360504.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""1cf33b65f90d4807b5941214e8dc6b1f""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360505.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360505.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198276000_IMG_360505.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""b5b7943326d349f0bb2cda88b037c51c""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360507.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360507.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198276000_IMG_360507.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""ae946d5ab3a04030977f3206647d7d1d""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360508.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360508.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198276000_IMG_360508.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""75ad3e778c664864b5689e9226306528""
               },
               {
                  ""sortOrder"": 4,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360509.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360509.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198276000_IMG_360509.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""8d48e7b5fca841adbeefeb14b154ceb2""
               },
               {
                  ""sortOrder"": 5,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360510.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360510.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198276000_IMG_360510.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""ad379a04614c4fb78ae12c851f57314c""
               },
               {
                  ""sortOrder"": 6,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360511.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360511.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198276000_IMG_360511.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""9c879b7a24ad4cee952b7dfe4f4f8a1f""
               },
               {
                  ""sortOrder"": 7,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360512.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198276000_IMG_360512.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198276000_IMG_360512.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""b469bfdfc7964ac88dc9071d8159fe2d""
               },
               {
                  ""sortOrder"": 8,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198277000_IMG_360513.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198277000_IMG_360513.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198277000_IMG_360513.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""fe526eff32f14b0fb2aa1275bd2c1bfd""
               },
               {
                  ""sortOrder"": 9,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198277000_IMG_360514.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198277000_IMG_360514.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198277000_IMG_360514.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""737325b344a343b7aa08c0f93582bfa9""
               },
               {
                  ""sortOrder"": 10,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198277000_IMG_360515.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198277000_IMG_360515.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198277000_IMG_360515.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""325817bbf06b4b20a9a940b359e30ce6""
               },
               {
                  ""sortOrder"": 11,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198277000_IMG_360516.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198277000_IMG_360516.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198277000_IMG_360516.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""d467c341dd3b492787c4b62c777dcafd""
               },
               {
                  ""sortOrder"": 12,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198621000_IMG_362316.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198621000_IMG_362316.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198621000_IMG_362316.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""775a57a8cd364bf7b10259210a9582ef""
               },
               {
                  ""sortOrder"": 13,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198974000_1022653.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SOFDRAX100/1389198974000_1022653.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1389198974000_1022653.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""309d13a23d5a4f95836ca9203b3afbf8""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:30.48Z"",
            ""modifiedDate"": ""2018-06-12T06:16:36.813Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""f0133829838f4b1e8019ff045bb31cd7""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""f0133829838f4b1e8019ff045bb31cd7"",
               ""sale"": 443,
               ""list"": 555.77,
               ""minQuantity"": 1,
               ""effectiveValue"": 443,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:04:31.423Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""aaf10c9172614efe8ceb4a92cb7b7f82""
            }
         ]
      },
      {
         ""productId"": ""ad4ae79ffdbc4c97959139a0c387c72e"",
         ""product"": {
            ""code"": ""SAGN4N910CBK"",
            ""name"": ""Samsung Galaxy Note 4 SM-N910C 32GB"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""outline"": ""0d4ad9bab9184d69a6e586effdf9c2ea"",
            ""path"": ""Cell phones"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""vendor"": ""94597fe09f634914a5fdf76c6ba86b04"",
            ""startDate"": ""2015-08-13T13:50:40.727Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/SAGN4N910CBK/1416164841000_1097106.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SAGN4N910CBK/1416164841000_1097106.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SAGN4N910CBK/1416164841000_1097106.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1416164841000_1097106.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""7735f688556d4069bdb442308eb1c34f""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/SAGN4N910CBK/1416165324000_IMG_440494.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/SAGN4N910CBK/1416165324000_IMG_440494.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1416165324000_IMG_440494.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""6e31eb631ade48b69ced452a8bf7260a""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:29.61Z"",
            ""modifiedDate"": ""2018-06-12T06:16:36.813Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""ad4ae79ffdbc4c97959139a0c387c72e""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""ad4ae79ffdbc4c97959139a0c387c72e"",
               ""list"": 866,
               ""minQuantity"": 1,
               ""effectiveValue"": 866,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:05:14.637Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""b40ae5ca30444c688893682ec41827ed""
            }
         ]
      },
      {
         ""productId"": ""7ae66c41242c4020a3328d5e841bda49"",
         ""product"": {
            ""code"": ""ONHTS9700THX"",
            ""name"": ""Onkyo HT-S9700THX 7.1-Channel Network Home Theater System"",
            ""catalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
            ""categoryId"": ""b1c093973bb24179bf130886b0477a18"",
            ""outline"": ""b1c093973bb24179bf130886b0477a18"",
            ""path"": ""Home Theater"",
            ""isBuyable"": true,
            ""isActive"": true,
            ""trackInventory"": true,
            ""maxQuantity"": 0,
            ""minQuantity"": 1,
            ""productType"": ""Physical"",
            ""startDate"": ""2015-08-14T10:21:02.26Z"",
            ""priority"": 0,
            ""imgSrc"": ""http://localhost:10645/assets/catalog/ONHTS9700THX/1423606907000_1119309.jpg"",
            ""images"": [
               {
                  ""sortOrder"": 0,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ONHTS9700THX/1423606907000_1119309.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ONHTS9700THX/1423606907000_1119309.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1423606907000_1119309.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""1e086de2f28748879160a1b6fa47edfb""
               },
               {
                  ""sortOrder"": 1,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ONHTS9700THX/1423607169000_IMG_466190.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ONHTS9700THX/1423607169000_IMG_466190.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1423607169000_IMG_466190.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""6686a537b1cf43dc9f824e251fbc7112""
               },
               {
                  ""sortOrder"": 2,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ONHTS9700THX/1423607169000_IMG_466191.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ONHTS9700THX/1423607169000_IMG_466191.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1423607169000_IMG_466191.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""f282ac9414f8434a8879a25f36190718""
               },
               {
                  ""sortOrder"": 3,
                  ""relativeUrl"": ""http://localhost:10645/assets/catalog/ONHTS9700THX/1423607169000_IMG_466192.jpg"",
                  ""url"": ""http://localhost:10645/assets/catalog/ONHTS9700THX/1423607169000_IMG_466192.jpg"",
                  ""typeId"": ""Image"",
                  ""group"": ""images"",
                  ""name"": ""1423607169000_IMG_466192.jpg"",
                  ""isInherited"": false,
                  ""seoObjectType"": ""Image"",
                  ""id"": ""329e1ddd060142099cbc5c1539eeb0e1""
               }
            ],
            ""seoObjectType"": ""CatalogProduct"",
            ""isInherited"": false,
            ""createdDate"": ""2015-10-06T22:39:28.947Z"",
            ""modifiedDate"": ""2018-06-12T06:16:36.813Z"",
            ""createdBy"": ""unknown"",
            ""modifiedBy"": ""unknown"",
            ""id"": ""7ae66c41242c4020a3328d5e841bda49""
         },
         ""prices"": [
            {
               ""pricelistId"": ""97abc1f99a08429e868f4528bf48eba5"",
               ""currency"": ""EUR"",
               ""productId"": ""7ae66c41242c4020a3328d5e841bda49"",
               ""list"": 111,
               ""minQuantity"": 1,
               ""effectiveValue"": 111,
               ""createdDate"": ""0001-01-01T00:00:00Z"",
               ""modifiedDate"": ""2015-12-22T12:06:18.23Z"",
               ""modifiedBy"": ""admin"",
               ""id"": ""c423eb70c53f4cb5b55ee16901d10085""
            }
         ]
      }
   ]
}
";
            #endregion 

            return (PriceSearchResult)JsonConvert.DeserializeObject(resultInJSON, typeof(PriceSearchResult));

        }
    }
}
