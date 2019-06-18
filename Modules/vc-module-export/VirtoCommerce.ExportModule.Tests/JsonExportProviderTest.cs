using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.ExportModule.Data.Services;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class JsonExportProviderTest
    {
        [Fact]
        public async Task ClearCircleReferencesTest()
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
                PropertiesInfo = new[]
                {
                    new ExportTypePropertyInfo() { Name = "Name"} ,
                    new ExportTypePropertyInfo() { Name = "Prices.Id"} ,
                    new ExportTypePropertyInfo() { Name = "Id"}
                }
            };
            //Act

            var filteredPricelist = SerializeAndDeserialize(metadata, pricelist);

            //Assert
            Assert.Null(filteredPricelist.Prices.First().Pricelist);
        }

        [Fact]
        public async Task DoNotExportAssignmentsTest()
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
                PropertiesInfo = new[]
                {
                    new ExportTypePropertyInfo() { Name = "Name"} ,
                    new ExportTypePropertyInfo() { Name = "Prices.Id"} ,
                    new ExportTypePropertyInfo() { Name = "Id"}
                }
            };

            //Act
            var filteredPricelist = SerializeAndDeserialize(metadata, priceList);

            //Assets
            Assert.NotNull(filteredPricelist);
            Assert.Equal(2, filteredPricelist.Prices.Count);
            Assert.Null(filteredPricelist.Assignments);
        }

        [Fact]
        public async Task FilteringValuesTest()
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
                PropertiesInfo = new[]
                {
                    new ExportTypePropertyInfo() { Name = "Name"} ,
                    new ExportTypePropertyInfo() { Name = "Id"},
                    new ExportTypePropertyInfo() { Name = "Prices.Id"} ,
                    new ExportTypePropertyInfo() { Name = "Prices.PricelistId"} ,
                    new ExportTypePropertyInfo() { Name = "Prices.List"} ,
                    new ExportTypePropertyInfo() { Name = "Prices.CreatedDate"} ,
                    new ExportTypePropertyInfo() { Name = "Assignments.Id"} ,
                    new ExportTypePropertyInfo() { Name = "Assignments.PricelistId"} ,
                }
            };

            //Act
            var filteredPricelist = SerializeAndDeserialize(metadata, priceList);

            //Assets
            Assert.Equal(2, filteredPricelist.Prices.Count);
            Assert.Equal(1, filteredPricelist.Prices.Count(x => x.List == 25));
            Assert.Equal(1, filteredPricelist.Prices.Count(x => x.List == 26));
            Assert.Equal(1, filteredPricelist.Prices.Count(x => x.CreatedDate == date));
            Assert.Equal(0, filteredPricelist.Prices.Count(x => x.EndDate == date));

            Assert.Equal(2, filteredPricelist.Assignments.Count(x => string.IsNullOrEmpty(x.Name)));
            Assert.Equal(2, filteredPricelist.Assignments.Count(x => x.EndDate == null));
        }

        [Fact]
        public async Task ExportOnlyPriceTest()
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
                PropertiesInfo = new[]
                {
                    new ExportTypePropertyInfo() { Name = "Prices.Id"} ,
                    new ExportTypePropertyInfo() { Name = "Prices.PricelistId"} ,
                    new ExportTypePropertyInfo() { Name = "Prices.List"} ,
                    new ExportTypePropertyInfo() { Name = "Prices.CreatedDate"} ,
                }
            };

            //Act
            var filteredPricelist = SerializeAndDeserialize(metadata, priceList);

            //Assets
            Assert.True(string.IsNullOrEmpty(filteredPricelist.Name));
            Assert.True(string.IsNullOrEmpty(filteredPricelist.Id));

            Assert.Equal(1, filteredPricelist.Prices.Count(x => x.List == 25));
            Assert.Equal(1, filteredPricelist.Prices.Count(x => x.List == 26));
            Assert.Equal(1, filteredPricelist.Prices.Count(x => x.CreatedDate == date));
            Assert.Equal(0, filteredPricelist.Prices.Count(x => x.EndDate == date));
            Assert.Equal(2, filteredPricelist.Prices.Count(x => x.Pricelist == null));

            Assert.Null(filteredPricelist.Assignments);
        }

        [Fact]
        public async Task ExportOnlyAssignmentsTest()
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
                PropertiesInfo = new[]
                {
                    new ExportTypePropertyInfo() { Name = "Assignments.Id"} ,
                    new ExportTypePropertyInfo() { Name = "Assignments.PricelistId"} ,
                }
            };

            //Act
            var filteredPricelist = SerializeAndDeserialize(metadata, priceList);

            //Assets
            Assert.True(string.IsNullOrEmpty(filteredPricelist.Name));
            Assert.True(string.IsNullOrEmpty(filteredPricelist.Id));

            Assert.Equal(1, filteredPricelist.Assignments.Count(x => x.Id == "A1"));
            Assert.Equal(1, filteredPricelist.Assignments.Count(x => x.Id == "A2"));
            Assert.Equal(2, filteredPricelist.Assignments.Count(x => x.Pricelist == null));

            Assert.Null(filteredPricelist.Prices);
        }

        [Fact]
        public async Task ExportPriceTest()
        {
            //Arrange
            var pricelist = new Pricelist() { Id = "1", Name = "Name" };
            var price = new Price() { Id = "P1", PricelistId = "1", Pricelist = pricelist };

            var metadata = new ExportedTypeMetadata()
            {
                PropertiesInfo = new[]
                {
                    new ExportTypePropertyInfo() { Name = "Id"} ,
                    new ExportTypePropertyInfo() { Name = "Pricelist.Id"} ,
                }
            };

            //Act
            var filteredPrice = SerializeAndDeserialize(metadata, price);

            //Assets

            Assert.Equal("P1", filteredPrice.Id);
            Assert.NotNull(filteredPrice.Pricelist);
            Assert.Equal("1", price.Pricelist.Id);
            Assert.Null(price.Pricelist.Name);
        }


        private T SerializeAndDeserialize<T>(ExportedTypeMetadata metadata, T obj)
        {
            var jsonConfiguration = new JsonProviderConfiguration() { Settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore } };

            using (Stream stream = new MemoryStream())
            {
                var jsonExportProvider = new JsonExportProvider(stream, jsonConfiguration);
                jsonExportProvider.Metadata = metadata;
                jsonExportProvider.WriteRecord(obj);

                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
        }
    }
}
