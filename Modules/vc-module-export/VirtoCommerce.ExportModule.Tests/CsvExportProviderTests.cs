using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.ExportModule.Data.Services;
using VirtoCommerce.ExportModule.Tests.PricingModuleModelMocks;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class CsvExportProviderTests
    {
        [Fact]
        public Task ExportPrices_FilterPlainMembers()
        {
            var date = new DateTime(2020, 10, 20);
            var prices = new List<Price>()
            {
                new Price
                {
                    Id = "P1",
                    PricelistId = "1",
                    List = 25,
                    CreatedDate = date,
                    Currency = "USD",
                },
                new Price
                {
                    Id = "P2",
                    PricelistId = "1",
                    List = 26,
                    EndDate = date,
                    ModifiedDate = date,
                    Pricelist = new Pricelist() { Id = "1", Name = "Pricelist 1" },
                },
            };

            var propertyInfos = new[]
            {
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Price.Id),
                    DisplayName = nameof(Price.Id),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Price.PricelistId),
                    DisplayName = nameof(Price.PricelistId),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Price.List),
                    DisplayName = nameof(Price.List),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Price.CreatedDate),
                    DisplayName = nameof(Price.CreatedDate),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Price.EndDate),
                    DisplayName = nameof(Price.EndDate),
                },
            };
            var dataQuery = new PricelistExportDataQuery() { IncludedProperties = propertyInfos };
            var exportedDataRequest = CreatExportDataRequest(dataQuery);

            //Act
            var filteredPricesString = SerializeAndRead(exportedDataRequest, prices);

            //Assets
            Assert.Equal("Id,PricelistId,List,CreatedDate,EndDate\r\nP1,1,25,10/20/2020 00:00:00,\r\nP2,1,26,01/01/0001 00:00:00,10/20/2020 00:00:00\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPrices_ExportNameTest()
        {
            var date = new DateTime(2020, 10, 20);
            var prices = new List<Price>()
            {
                new Price
                {
                    Id = "P1",
                    PricelistId = "1",
                    List = 25,
                    CreatedDate = date,
                    Currency = "USD",
                },
                new Price
                {
                    Id = "P2",
                    PricelistId = "1",
                    List = 26,
                    EndDate = date,
                    ModifiedDate = date,
                    Pricelist = new Pricelist() { Id = "1", Name = "Pricelist 1" },
                },
            };

            var propertyInfos = new[]
            {
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Price.Id),
                    DisplayName = $"{nameof(Price)}.{nameof(Price.Id)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Price.PricelistId),
                    DisplayName = $"{nameof(Price)}.{nameof(Price.PricelistId)}",
                },
            };
            var dataQuery = new PricelistExportDataQuery() { IncludedProperties = propertyInfos };
            var exportedDataRequest = CreatExportDataRequest(dataQuery);

            //Act
            var filteredPricesString = SerializeAndRead(exportedDataRequest, prices);

            //Assets
            Assert.Equal("Price.Id,Price.PricelistId\r\nP1,1\r\nP2,1\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelistAssignments_FilterPlainMembers()
        {
            var date = new DateTime(2020, 10, 20);
            var assignments = new List<PricelistAssignment>()
            {
                new PricelistAssignment
                {
                    Id = "PA1",
                    PricelistId = "1",
                    Description = "PA1 Description",
                    Priority = 1,
                    Pricelist = new Pricelist() { Name = "Pricelist1"},
                },
                new PricelistAssignment
                {
                    Id = "PA2",
                    PricelistId = "2",
                    EndDate = date,
                    Description = "PA2 Description",
                    ConditionExpression = "<expression/>",
                },
            };

            var propertyInfos = new[]
            {
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(PricelistAssignment.Id),
                    DisplayName = nameof(PricelistAssignment.Id),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(PricelistAssignment.Description),
                    DisplayName = nameof(PricelistAssignment.Description),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(PricelistAssignment.EndDate),
                    DisplayName = nameof(PricelistAssignment.EndDate),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(PricelistAssignment.Priority),
                    DisplayName = nameof(PricelistAssignment.Priority),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(PricelistAssignment.ConditionExpression),
                    DisplayName = nameof(PricelistAssignment.ConditionExpression),
                },
            };
            var dataQuery = new PricelistExportDataQuery() { IncludedProperties = propertyInfos };
            var exportedDataRequest = CreatExportDataRequest(dataQuery);

            //Act
            var filteredPricesString = SerializeAndRead(exportedDataRequest, assignments);

            //Assets
            Assert.Equal("Id,Description,EndDate,Priority,ConditionExpression\r\nPA1,PA1 Description,,1,\r\nPA2,PA2 Description,10/20/2020 00:00:00,0,<expression/>\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelists_FilterPlainMembers()
        {
            var pricelists = CreatePricelists();

            var propertyInfos = new[]
            {
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Id),
                    DisplayName = nameof(Pricelist.Id),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Name),
                    DisplayName = nameof(Pricelist.Name),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Currency),
                    DisplayName = nameof(Pricelist.Currency),
                },
            };
            var dataQuery = new PricelistExportDataQuery() { IncludedProperties = propertyInfos };
            var exportedDataRequest = CreatExportDataRequest(dataQuery);

            //Act
            var filteredPricesString = SerializeAndRead(exportedDataRequest, pricelists);

            //Assets
            Assert.Equal("Id,Name,Currency\r\nPL1,Pricelist 1,USD\r\nPL2,Pricelist 2,EUR\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelists_FilterNestedMembers()
        {
            var pricelists = CreatePricelists();

            var propertyInfos = new[]
            {
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Id),
                    DisplayName = nameof(Pricelist.Id),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Name),
                    DisplayName = nameof(Pricelist.Name),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Currency),
                    DisplayName = nameof(Pricelist.Currency),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                    DisplayName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                    DisplayName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                    DisplayName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                },
            };
            var dataQuery = new PricelistExportDataQuery() { IncludedProperties = propertyInfos };
            var exportedDataRequest = CreatExportDataRequest(dataQuery);

            //Act
            var filteredPricesString = SerializeAndRead(exportedDataRequest, pricelists);

            //Assets
            Assert.Equal("Id,Name,Currency,ActiveAssignment.Id,ActiveAssignment.Name,ActiveAssignment.Priority\r\nPL1,Pricelist 1,USD,PA1_1,PA1_1 Name,1\r\nPL2,Pricelist 2,EUR,PA2_1,PA2_1 Name,11\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelists_DoesNotWriteEnumerableProperties()
        {
            var pricelists = CreatePricelists();

            var propertyInfos = new[]
            {
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Id),
                    DisplayName = nameof(Pricelist.Id),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Name),
                    DisplayName = nameof(Pricelist.Name),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Currency),
                    DisplayName = nameof(Pricelist.Currency),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.Prices)}.{nameof(Price.Id)}",
                    DisplayName = $"{nameof(Pricelist.Prices)}.{nameof(Price.Id)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.Prices)}.{nameof(Price.List)}",
                    DisplayName = $"{nameof(Pricelist.Prices)}.{nameof(Price.List)}",
                },
            };
            var dataQuery = new PricelistExportDataQuery() { IncludedProperties = propertyInfos };
            var exportedDataRequest = CreatExportDataRequest(dataQuery);

            //Act
            var filteredPricesString = SerializeAndRead(exportedDataRequest, pricelists);

            //Assets
            Assert.Equal("Id,Name,Currency\r\nPL1,Pricelist 1,USD\r\nPL2,Pricelist 2,EUR\r\n", filteredPricesString);

            return Task.CompletedTask;
        }


        [Fact]
        public Task ExportPricelists_UseCustomMap()
        {
            var pricelists = CreatePricelists();

            var propertyInfos = new[]
            {
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Id),
                    DisplayName = nameof(Pricelist.Id),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Name),
                    DisplayName = nameof(Pricelist.Name),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Currency),
                    DisplayName = nameof(Pricelist.Name),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                    DisplayName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                    DisplayName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                    DisplayName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.Prices)}.{nameof(Price.Id)}",
                    DisplayName = $"{nameof(Pricelist.Prices)}.{nameof(Price.Id)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.Prices)}.{nameof(Price.List)}",
                    DisplayName = $"{nameof(Pricelist.Prices)}.{nameof(Price.List)}",
                },
            };
            var dataQuery = new PricelistExportDataQuery() { IncludedProperties = propertyInfos };

            var configuration = new Configuration(cultureInfo: CultureInfo.InvariantCulture);
            configuration.RegisterClassMap<PricelistTestMapping>();

            var exportDataRequest = CreatExportDataRequest(dataQuery, new CsvProviderConfiguration() { Configuration = configuration });

            //Act
            var filteredPricesString = SerializeAndRead(exportDataRequest, pricelists);

            //Assets
            Assert.Equal("Id,Name,Currency,ActiveAssignment.Id,ActiveAssignment.Name\r\nPL1,Pricelist 1,USD,PA1_1,PA1_1 Name\r\nPL2,Pricelist 2,EUR,PA2_1,PA2_1 Name\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelists_SameTypePropertiesMapping()
        {
            var pricelists = CreatePricelists();
            var propertyInfos = new[]
            {
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Id),
                    DisplayName = nameof(Pricelist.Id),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = nameof(Pricelist.Name),
                    DisplayName = nameof(Pricelist.Name),
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                    DisplayName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                    DisplayName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                    DisplayName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                    DisplayName = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                    DisplayName = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                },
                new ExportedTypePropertyInfo()
                {
                    FullName = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                    DisplayName = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                },
            };
            var dataQuery = new PricelistExportDataQuery() { IncludedProperties = propertyInfos };

            var exportDataRequest = CreatExportDataRequest(dataQuery);

            //Act
            var filteredPricesString = SerializeAndRead(exportDataRequest, pricelists);

            //Assets
            Assert.Equal("Id,Name,ActiveAssignment.Id,ActiveAssignment.Name,ActiveAssignment.Priority,InactiveAssignment.Id,InactiveAssignment.Name,InactiveAssignment.Priority\r\nPL1,Pricelist 1,PA1_1,PA1_1 Name,1,PA1_2,PA1_2 Name,2\r\nPL2,Pricelist 2,PA2_1,PA2_1 Name,11,PA2_2,PA2_2 Name,12\r\n", filteredPricesString);

            return Task.CompletedTask;

        }

        private string SerializeAndRead<T>(ExportDataRequest exportDataRequest, IEnumerable<T> items) where T : IExportable
        {
            exportDataRequest.ProviderConfig = exportDataRequest.ProviderConfig ?? new CsvProviderConfiguration() { };

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                using (var csvExportProvider = new CsvExportProvider(exportDataRequest))
                {
                    foreach (var item in items)
                    {
                        csvExportProvider.WriteRecord(writer, item);
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);

                return new StreamReader(stream).ReadToEnd();
            }
        }

        private static List<Pricelist> CreatePricelists()
        {
            var result = new List<Pricelist>()
            {
                new Pricelist
                {
                    Id = "PL1",
                    Name = "Pricelist 1",
                    Currency = "USD",
                    Assignments = new List<PricelistAssignment>()
                    {
                        new PricelistAssignment
                        {
                            Id = "PA1_1",
                            Name = "PA1_1 Name",
                            Priority = 1,
                        },
                        new PricelistAssignment
                        {
                            Id = "PA1_2",
                            Name = "PA1_2 Name",
                            Priority = 2,
                        },
                    },
                    Prices = new List<Price>()
                    {
                        new Price
                        {
                            Id = "P1_1",
                            List = 25,
                        },
                        new Price
                        {
                            Id = "P1_2",
                            List = 26,
                        },
                    },
                },
                new Pricelist
                {
                    Id = "PL2",
                    Name = "Pricelist 2",
                    Currency = "EUR",
                    Assignments = new List<PricelistAssignment>()
                    {
                        new PricelistAssignment
                        {
                            Id = "PA2_1",
                            Name = "PA2_1 Name",
                            Priority = 11,
                            CatalogId = "Cat1",
                        },
                        new PricelistAssignment
                        {
                            Id = "PA2_2",
                            Name = "PA2_2 Name",
                            Priority = 12,
                            CatalogId = "Cat2",
                        },
                        new PricelistAssignment
                        {
                            Id = "PA2_3",
                            Name = "PA2_3 Name",
                            Priority = 13,
                        },
                    },
                    Prices = new List<Price>()
                    {
                        new Price
                        {
                            Id = "P2_1",
                            List = 12,
                        },
                        new Price
                        {
                            Id = "P2_2",
                            List = 17,
                        },
                        new Price
                        {
                            Id = "P2_3",
                            List = 19,
                        },
                    },
                },
            };

            foreach (var pricelist in result)
            {
                foreach (var assignment in pricelist.Assignments)
                {
                    assignment.Pricelist = pricelist;
                    assignment.PricelistId = pricelist.Id;
                }

                foreach (var price in pricelist.Prices)
                {
                    price.Pricelist = pricelist;
                    price.PricelistId = pricelist.Id;
                }

                pricelist.ActiveAssignment = pricelist?.Assignments?.FirstOrDefault();
                pricelist.InactiveAssignment = pricelist?.Assignments?.Skip(1).First();
            }

            return result;
        }

        private ExportDataRequest CreatExportDataRequest(ExportDataQuery dataQuery, IExportProviderConfiguration providerConfig = null)
        {
            var result = new ExportDataRequest()
            {
                ProviderName = nameof(CsvExportProvider),
                DataQuery = dataQuery,
                ProviderConfig = providerConfig,
            };

            return result;
        }
    }

    public class PricelistTestMapping : ClassMap<Pricelist>
    {
        public PricelistTestMapping()
        {
            Map(x => x.Id).Index(0);
            Map(x => x.Name).Index(1);
            Map(x => x.Currency).Index(2);
            Map(x => x.ActiveAssignment.Id).Name("ActiveAssignment.Id").Index(3);
            Map(x => x.ActiveAssignment.Name).Name("ActiveAssignment.Name").Index(4);
            // Equals to following, but without indexes set explicitly
            // ReferenceMaps.Add(new MemberReferenceMap(typeof(Pricelist).GetProperty(nameof(Pricelist.ActiveAssignment)), new PricelistAssignmentTestMapping(nameof(Pricelist.ActiveAssignment))));
        }
    }

    public class PricelistAssignmentTestMapping : ClassMap<PricelistAssignment>
    {
        public PricelistAssignmentTestMapping(string pathName)
        {
            Map(x => x.Id).Name($"{pathName}.{nameof(PricelistAssignment.Id)}");
            Map(x => x.Name).Name($"{pathName}.{nameof(PricelistAssignment.Name)}");
        }
    }
}
