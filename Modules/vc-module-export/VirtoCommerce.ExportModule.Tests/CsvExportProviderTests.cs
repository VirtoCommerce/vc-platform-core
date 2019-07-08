using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.ExportModule.Data.Services;
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

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Price.Id),
                        ExportName = nameof(Price.Id),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Price.PricelistId),
                        ExportName = nameof(Price.PricelistId),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Price.List),
                        ExportName = nameof(Price.List),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Price.CreatedDate),
                        ExportName = nameof(Price.CreatedDate),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Price.EndDate),
                        ExportName = nameof(Price.EndDate),
                    },
                }
            };

            //Act
            var filteredPricesString = SerializeAndRead(metadata, prices);

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

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Price.Id),
                        ExportName = $"{nameof(Price)}.{nameof(Price.Id)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Price.PricelistId),
                        ExportName = $"{nameof(Price)}.{nameof(Price.PricelistId)}",
                    },
                }
            };

            //Act
            var filteredPricesString = SerializeAndRead(metadata, prices);

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

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(PricelistAssignment.Id),
                        ExportName = nameof(PricelistAssignment.Id),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(PricelistAssignment.Description),
                        ExportName = nameof(PricelistAssignment.Description),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(PricelistAssignment.EndDate),
                        ExportName = nameof(PricelistAssignment.EndDate),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(PricelistAssignment.Priority),
                        ExportName = nameof(PricelistAssignment.Priority),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(PricelistAssignment.ConditionExpression),
                        ExportName = nameof(PricelistAssignment.ConditionExpression),
                    },
                }
            };

            //Act
            var filteredPricesString = SerializeAndRead(metadata, assignments);

            //Assets
            Assert.Equal("Id,Description,EndDate,Priority,ConditionExpression\r\nPA1,PA1 Description,,1,\r\nPA2,PA2 Description,10/20/2020 00:00:00,0,<expression/>\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelists_FilterPlainMembers()
        {
            var pricelists = CreatePricelists();

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Id),
                        ExportName = nameof(Pricelist.Id),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Name),
                        ExportName = nameof(Pricelist.Name),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Currency),
                        ExportName = nameof(Pricelist.Currency),
                    },
                }
            };

            //Act
            var filteredPricesString = SerializeAndRead(metadata, pricelists);

            //Assets
            Assert.Equal("Id,Name,Currency\r\nPL1,Pricelist 1,USD\r\nPL2,Pricelist 2,EUR\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelists_FilterNestedMembers()
        {
            var pricelists = CreatePricelists();

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Id),
                        ExportName = nameof(Pricelist.Id),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Name),
                        ExportName = nameof(Pricelist.Name),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Currency),
                        ExportName = nameof(Pricelist.Currency),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                        ExportName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                        ExportName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                        ExportName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                    },
                }
            };

            //Act
            var filteredPricesString = SerializeAndRead(metadata, pricelists);

            //Assets
            Assert.Equal("Id,Name,Currency,ActiveAssignment.Id,ActiveAssignment.Name,ActiveAssignment.Priority\r\nPL1,Pricelist 1,USD,PA1_1,PA1_1 Name,1\r\nPL2,Pricelist 2,EUR,PA2_1,PA2_1 Name,11\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelists_DoesNotWriteEnumerableProperties()
        {
            var pricelists = CreatePricelists();

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Id),
                        ExportName = nameof(Pricelist.Id),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Name),
                        ExportName = nameof(Pricelist.Name),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Currency),
                        ExportName = nameof(Pricelist.Currency),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.Prices)}.{nameof(Price.Id)}",
                        ExportName = $"{nameof(Pricelist.Prices)}.{nameof(Price.Id)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.Prices)}.{nameof(Price.List)}",
                        ExportName = $"{nameof(Pricelist.Prices)}.{nameof(Price.List)}",
                    },
                }
            };

            //Act
            var filteredPricesString = SerializeAndRead(metadata, pricelists);

            //Assets
            Assert.Equal("Id,Name,Currency\r\nPL1,Pricelist 1,USD\r\nPL2,Pricelist 2,EUR\r\n", filteredPricesString);

            return Task.CompletedTask;
        }


        [Fact]
        public Task ExportPricelists_UseCustomMap()
        {
            var pricelists = CreatePricelists();

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Id),
                        ExportName = nameof(Pricelist.Id),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Name),
                        ExportName = nameof(Pricelist.Name),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Currency),
                        ExportName = nameof(Pricelist.Name),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                        ExportName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                        ExportName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                        ExportName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.Prices)}.{nameof(Price.Id)}",
                        ExportName = $"{nameof(Pricelist.Prices)}.{nameof(Price.Id)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.Prices)}.{nameof(Price.List)}",
                        ExportName = $"{nameof(Pricelist.Prices)}.{nameof(Price.List)}",
                    },
                }
            };

            var configuration = new Configuration();
            configuration.RegisterClassMap<PricelistTestMapping>();

            //Act
            var filteredPricesString = SerializeAndRead(metadata, pricelists, configuration);

            //Assets
            Assert.Equal("Id,Name,Currency,ActiveAssignment.Id,ActiveAssignment.Name\r\nPL1,Pricelist 1,USD,PA1_1,PA1_1 Name\r\nPL2,Pricelist 2,EUR,PA2_1,PA2_1 Name\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelists_SameTypePropertiesMapping()
        {
            var pricelists = CreatePricelists();

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Id),
                        ExportName = nameof(Pricelist.Id),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = nameof(Pricelist.Name),
                        ExportName = nameof(Pricelist.Name),
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                        ExportName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                        ExportName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                        ExportName = $"{nameof(Pricelist.ActiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                        ExportName = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Id)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                        ExportName = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Name)}",
                    },
                    new ExportedTypeColumnInfo()
                    {
                        Name = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                        ExportName = $"{nameof(Pricelist.InactiveAssignment)}.{nameof(PricelistAssignment.Priority)}",
                    },
                }
            };

            //Act
            var filteredPricesString = SerializeAndRead(metadata, pricelists);

            //Assets
            Assert.Equal("Id,Name,ActiveAssignment.Id,ActiveAssignment.Name,ActiveAssignment.Priority,InactiveAssignment.Id,InactiveAssignment.Name,InactiveAssignment.Priority\r\nPL1,Pricelist 1,PA1_1,PA1_1 Name,1,PA1_2,PA1_2 Name,2\r\nPL2,Pricelist 2,PA2_1,PA2_1 Name,11,PA2_2,PA2_2 Name,12\r\n", filteredPricesString);

            return Task.CompletedTask;

        }

        private string SerializeAndRead(ExportedTypeMetadata metadata, IEnumerable items, Configuration configuration = null)
        {
            var csvConfiguration = new CsvProviderConfiguration() { Configuration = configuration ?? new Configuration() };

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                using (var csvExportProvider = new CsvExportProvider(csvConfiguration))
                {
                    csvExportProvider.Metadata = metadata;

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
