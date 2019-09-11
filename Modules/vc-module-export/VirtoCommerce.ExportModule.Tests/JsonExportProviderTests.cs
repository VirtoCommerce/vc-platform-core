using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.JsonProvider;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class JsonExportProviderTests
    {
        [Fact]
        public Task ExportPricelists_AllPropertiesExported()
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

            //Act
            var deserializedResult = SerializeAndDeserialize(priceList);

            //Assert
            Assert.Single(deserializedResult);

            var resultPricelist = deserializedResult.First();

            Assert.Equal("1", resultPricelist.Id);
            Assert.Equal("PA1", resultPricelist.Name);

            Assert.Equal(2, resultPricelist.Prices.Count);
            Assert.Equal(2, resultPricelist.Assignments.Count);

            Assert.Equal(1, resultPricelist.Prices.Count(x => x.List == 25));
            Assert.Equal(1, resultPricelist.Prices.Count(x => x.List == 26));
            Assert.Equal(2, resultPricelist.Prices.Count(x => x.PricelistId == "1"));
            Assert.Equal(1, resultPricelist.Prices.Count(x => x.Id == "P1"));
            Assert.Equal(1, resultPricelist.Prices.Count(x => x.Id == "P2"));
            Assert.Equal(1, resultPricelist.Prices.Count(x => x.CreatedDate == date && x.EndDate == null));
            Assert.Equal(1, resultPricelist.Prices.Count(x => x.CreatedDate == default(DateTime) && x.EndDate == date));
            Assert.Equal(2, resultPricelist.Prices.Count(x => x.Pricelist == null));

            Assert.Equal(2, resultPricelist.Assignments.Count(x => x.PricelistId == "1"));
            Assert.Equal(1, resultPricelist.Assignments.Count(x => x.Id == "A1"));
            Assert.Equal(1, resultPricelist.Assignments.Count(x => x.Id == "A2"));
            Assert.Equal(2, resultPricelist.Assignments.Count(x => x.Pricelist == null));

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

            //Act
            var deserializedResult = SerializeAndDeserializeMixedObjects(pricelist, price, pricelistAssignment);

            //Assert
            Assert.Equal(3, deserializedResult.Length);

            Assert.Single(deserializedResult.OfType<Pricelist>().ToArray());
            Assert.Single(deserializedResult.OfType<Price>().ToArray());
            Assert.Single(deserializedResult.OfType<PricelistAssignment>().ToArray());

            Assert.Equal("1", deserializedResult.OfType<Pricelist>().First().Id);
            Assert.Equal("P1_Inner", deserializedResult.OfType<Pricelist>().First().Prices.First().Id);
            Assert.Equal("A1_Inner", deserializedResult.OfType<Pricelist>().First().Assignments.First().Id);

            Assert.Equal("P1", deserializedResult.OfType<Price>().First().Id);

            Assert.Equal("A1", deserializedResult.OfType<PricelistAssignment>().First().Id);

            return Task.CompletedTask;
        }


        private T[] SerializeAndDeserialize<T>(T obj) where T : IExportable
        {
            return JsonConvert.DeserializeObject<T[]>(SerializeToString(obj));
        }

        private object[] SerializeAndDeserializeMixedObjects(params IExportable[] objects)
        {
            var deserializedString = SerializeToString(objects);

            var resultArray = JArray.Parse(deserializedString);

            return resultArray.Select(x => x.ToObject(Type.GetType($@"VirtoCommerce.ExportModule.Tests.{x["$discriminator"]?.ToString()}")))
                .ToArray();
        }

        private static string SerializeToString(params IExportable[] objects)
        {
            var exportDataRequest = new ExportDataRequest()
            {
                ProviderName = nameof(JsonExportProvider),
                ProviderConfig = new JsonProviderConfiguration() { Settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore } },
            };

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                using (var jsonExportProvider = new JsonExportProvider(exportDataRequest))
                {
                    foreach (var obj in objects)
                    {
                        jsonExportProvider.WriteRecord(writer, obj);
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            }
        }
    }
}
