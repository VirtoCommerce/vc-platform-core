using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.ExportModule.Data.Services;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class JsonExportProviderTests
    {
        [Fact]
        public Task ClearCircleReferencesTest()
        {
            //Arrange
            var pricelist = new Pricelist()
            {
                Id = "1",
                Name = "PA1",
                Prices = new List<Price>()
                    {
                        new Price {Id = "P1", List = 25, PricelistId = "1"}
                    }
            };
            pricelist.Prices.First().Pricelist = pricelist;

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo() { Name = "Name"} ,
                    new ExportedTypeColumnInfo() { Name = "Prices.Id"} ,
                    new ExportedTypeColumnInfo() { Name = "Id"}
                }
            };
            //Act

            var pricelists = SerializeAndDeserialize(metadata, pricelist);

            //Assert
            Assert.Single(pricelists);
            Assert.Null(pricelists.First().Prices.First().Pricelist);

            return Task.CompletedTask;
        }

        [Fact]
        public Task DoNotExportAssignmentsTest()
        {
            //Arrange
            var priceList = new Pricelist()
            {
                Id = "1",
                Name = "PA1",
                Prices = new List<Price>()
                {
                    new Price {Id = "P1", PricelistId = "1"}, new Price {Id = "P2", PricelistId = "1"},
                },
                Assignments = new List<PricelistAssignment>
                {
                    new PricelistAssignment {Id = "A1", PricelistId = "1"},new PricelistAssignment {Id = "A2", PricelistId = "1"},
                }
            };

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo() { Name = "Name"} ,
                    new ExportedTypeColumnInfo() { Name = "Prices.Id"} ,
                    new ExportedTypeColumnInfo() { Name = "Id"}
                }
            };

            //Act
            var pricelists = SerializeAndDeserialize(metadata, priceList);

            //Assets
            Assert.NotNull(pricelists);
            Assert.Single(pricelists);
            Assert.Equal(2, pricelists.First().Prices.Count);
            Assert.Null(pricelists.First().Assignments);

            return Task.CompletedTask;
        }

        [Fact]
        public Task FilteringValuesTest()
        {
            var date = new DateTime(2020, 10, 20);
            var priceList = new Pricelist()
            {
                Id = "1",
                Name = "PA1",
                Prices = new List<Price>()
                {
                    new Price {Id = "P1", PricelistId = "1", List = 25, CreatedDate = date},
                    new Price {Id = "P2", PricelistId = "1", List = 26, EndDate = date},
                },
                Assignments = new List<PricelistAssignment>
                {
                    new PricelistAssignment {Id = "A1", PricelistId = "1", Name = "name1", EndDate = date},
                    new PricelistAssignment {Id = "A2", PricelistId = "1", Name = "name2", EndDate = date},
                }
            };

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo() { Name = "Name"} ,
                    new ExportedTypeColumnInfo() { Name = "Id"},
                    new ExportedTypeColumnInfo() { Name = "Prices.Id"} ,
                    new ExportedTypeColumnInfo() { Name = "Prices.PricelistId"} ,
                    new ExportedTypeColumnInfo() { Name = "Prices.List"} ,
                    new ExportedTypeColumnInfo() { Name = "Prices.CreatedDate"} ,
                    new ExportedTypeColumnInfo() { Name = "Assignments.Id"} ,
                    new ExportedTypeColumnInfo() { Name = "Assignments.PricelistId"} ,
                }
            };

            //Act
            var pricelists = SerializeAndDeserialize(metadata, priceList);

            //Assets
            Assert.Single(pricelists);

            Assert.Equal(2, pricelists.First().Prices.Count);
            Assert.Equal(1, pricelists.First().Prices.Count(x => x.List == 25));
            Assert.Equal(1, pricelists.First().Prices.Count(x => x.List == 26));
            Assert.Equal(1, pricelists.First().Prices.Count(x => x.CreatedDate == date));
            Assert.Equal(0, pricelists.First().Prices.Count(x => x.EndDate == date));
            Assert.True(pricelists.All(x => x.Prices.All(y => y.PricelistId == "1")));

            Assert.Equal(2, pricelists.First().Assignments.Count(x => string.IsNullOrEmpty(x.Name)));
            Assert.Equal(2, pricelists.First().Assignments.Count(x => x.EndDate == null));
            Assert.True(pricelists.All(x => x.Assignments.All(y => y.PricelistId == "1")));

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportOnlyPriceTest()
        {
            var date = new DateTime(2020, 10, 20);
            var priceList = new Pricelist()
            {
                Id = "1",
                Name = "PA1",
                Prices = new List<Price>()
                {
                    new Price {Id = "P1", PricelistId = "1", List = 25, CreatedDate = date},
                    new Price {Id = "P2", PricelistId = "1", List = 26, EndDate = date},
                },
                Assignments = new List<PricelistAssignment>
                {
                    new PricelistAssignment {Id = "A1", PricelistId = "1" },
                    new PricelistAssignment {Id = "A2", PricelistId = "1" },
                }
            };

            foreach (var price in priceList.Prices)
            {
                price.Pricelist = priceList;
            }

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo() { Name = "Prices.Id"} ,
                    new ExportedTypeColumnInfo() { Name = "Prices.PricelistId"} ,
                    new ExportedTypeColumnInfo() { Name = "Prices.List"} ,
                    new ExportedTypeColumnInfo() { Name = "Prices.CreatedDate"} ,
                }
            };

            //Act
            var pricelists = SerializeAndDeserialize(metadata, priceList);

            //Assets
            Assert.Single(pricelists);


            Assert.True(string.IsNullOrEmpty(pricelists.First().Name));
            Assert.True(string.IsNullOrEmpty(pricelists.First().Id));

            Assert.Equal(1, pricelists.First().Prices.Count(x => x.List == 25));
            Assert.Equal(1, pricelists.First().Prices.Count(x => x.List == 26));
            Assert.Equal(1, pricelists.First().Prices.Count(x => x.CreatedDate == date));
            Assert.Equal(0, pricelists.First().Prices.Count(x => x.EndDate == date));
            Assert.Equal(2, pricelists.First().Prices.Count(x => x.Pricelist == null));

            Assert.Null(pricelists.First().Assignments);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportOnlyAssignmentsTest()
        {
            var date = new DateTime(2020, 10, 20);
            var priceList = new Pricelist()
            {
                Id = "1",
                Name = "PA1",
                Prices = new List<Price>()
                {
                    new Price {Id = "P1", PricelistId = "1", List = 25, CreatedDate = date},
                    new Price {Id = "P2", PricelistId = "1", List = 26, EndDate = date},
                },
                Assignments = new List<PricelistAssignment>
                {
                    new PricelistAssignment {Id = "A1", PricelistId = "1" },
                    new PricelistAssignment {Id = "A2", PricelistId = "1" },
                }
            };

            foreach (var assignment in priceList.Assignments)
            {
                assignment.Pricelist = priceList;
            }

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo() { Name = "Assignments.Id"} ,
                    new ExportedTypeColumnInfo() { Name = "Assignments.PricelistId"} ,
                }
            };

            //Act
            var pricelists = SerializeAndDeserialize(metadata, priceList);

            //Assets
            Assert.Single(pricelists);

            Assert.True(string.IsNullOrEmpty(pricelists.First().Name));
            Assert.True(string.IsNullOrEmpty(pricelists.First().Id));

            Assert.Equal(1, pricelists.First().Assignments.Count(x => x.Id == "A1"));
            Assert.Equal(1, pricelists.First().Assignments.Count(x => x.Id == "A2"));
            Assert.Equal(2, pricelists.First().Assignments.Count(x => x.Pricelist == null));

            Assert.Null(pricelists.First().Prices);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPriceTest()
        {
            //Arrange
            var pricelist = new Pricelist() { Id = "1", Name = "Name" };
            var price = new Price() { Id = "P1", PricelistId = "1", Pricelist = pricelist };

            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo() { Name = "Id"} ,
                    new ExportedTypeColumnInfo() { Name = "Pricelist.Id"} ,
                }
            };

            //Act
            var prices = SerializeAndDeserialize(metadata, price);

            //Assets
            Assert.Single(prices);

            Assert.Equal("P1", prices.First().Id);
            Assert.NotNull(prices.First().Pricelist);
            Assert.Equal("1", price.Pricelist.Id);
            Assert.Null(price.Pricelist.Name);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportMixedTypes()
        {
            //Arrange
            var pricelist = new Pricelist()
            {
                Id = "1",
                Name = "Name",
                Prices = new List<Price>()
                {
                    new Price {Id = "P1_Inner", PricelistId = "1", List = 25},
                },
                Assignments = new List<PricelistAssignment>
                {
                    new PricelistAssignment {Id = "A1_Inner", PricelistId = "1" },
                },
            };
            var price = new Price() { Id = "P1", PricelistId = "1", Pricelist = pricelist };
            var pricelistAssignment = new PricelistAssignment { Id = "A1", PricelistId = "1" };


            var metadata = new ExportedTypeMetadata()
            {
                PropertyInfos = new[]
                {
                    new ExportedTypeColumnInfo() { Name = "Id"} ,
                    new ExportedTypeColumnInfo() { Name = "Name"} ,
                    new ExportedTypeColumnInfo() { Name = "Prices"} ,
                    new ExportedTypeColumnInfo() { Name = "Prices.Id"} ,
                    new ExportedTypeColumnInfo() { Name = "Assignments"} ,
                    new ExportedTypeColumnInfo() { Name = "Assignments.Id"} ,
                    new ExportedTypeColumnInfo() { Name = "PricelistId"} ,
                }
            };

            //Act
            var prices = SerializeAndDeserializeMixedObjects(metadata, pricelist, price, pricelistAssignment);

            //Assets
            Assert.Equal(3, prices.Length);

            Assert.Single(prices.OfType<Pricelist>().ToArray());
            Assert.Single(prices.OfType<Price>().ToArray());
            Assert.Single(prices.OfType<PricelistAssignment>().ToArray());

            Assert.Equal("1", prices.OfType<Pricelist>().First().Id);
            Assert.Equal("P1_Inner", prices.OfType<Pricelist>().First().Prices.First().Id);
            Assert.Equal("A1_Inner", prices.OfType<Pricelist>().First().Assignments.First().Id);

            Assert.Equal("P1", prices.OfType<Price>().First().Id);

            Assert.Equal("A1", prices.OfType<PricelistAssignment>().First().Id);

            return Task.CompletedTask;
        }


        private T[] SerializeAndDeserialize<T>(ExportedTypeMetadata metadata, T obj)
        {
            var jsonConfiguration = new JsonProviderConfiguration() { Settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore } };

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                using (var jsonExportProvider = new JsonExportProvider(jsonConfiguration))
                {
                    jsonExportProvider.Metadata = metadata;
                    jsonExportProvider.WriteRecord(writer, obj);
                }

                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);
                var streamString = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<T[]>(streamString);
            }
        }

        private object[] SerializeAndDeserializeMixedObjects(ExportedTypeMetadata metadata, params object[] objects)
        {
            var jsonConfiguration = new JsonProviderConfiguration() { Settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore } };

            using (Stream stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                using (var jsonExportProvider = new JsonExportProvider(jsonConfiguration))
                {
                    jsonExportProvider.Metadata = metadata;

                    foreach (var obj in objects)
                    {
                        jsonExportProvider.WriteRecord(writer, obj);
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(stream);
                var streamString = reader.ReadToEnd();
                var resultArray = JArray.Parse(streamString);

                return resultArray.Select(x => x.ToObject(Type.GetType(x["$discriminator"]?.ToString())))
                    .ToArray();
            }
        }
    }
}
