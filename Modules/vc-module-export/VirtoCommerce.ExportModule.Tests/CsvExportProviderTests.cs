using System;
using System.Collections.Generic;
using System.IO;
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
        public Task ExportPrices_FilterFields()
        {
            var date = new DateTime(2020, 10, 20);
            var prices = new List<Price>()
            {
                new Price {Id = "P1", PricelistId = "1", List = 25, CreatedDate = date},
                new Price {Id = "P2", PricelistId = "1", List = 26, EndDate = date},
            };

            var metadata = new ExportedTypeMetadata()
            {
                PropertiesInfo = new[]
                {
                    new ExportTypePropertyInfo() { Name = nameof(Price.Id), MemberInfo = typeof(Price).GetProperty(nameof(Price.Id)) },
                    new ExportTypePropertyInfo() { Name = nameof(Price.PricelistId), MemberInfo = typeof(Price).GetProperty(nameof(Price.PricelistId)) },
                    new ExportTypePropertyInfo() { Name = nameof(Price.List), MemberInfo = typeof(Price).GetProperty(nameof(Price.List)) },
                    new ExportTypePropertyInfo() { Name = nameof(Price.CreatedDate), MemberInfo = typeof(Price).GetProperty(nameof(Price.CreatedDate)) },
                    new ExportTypePropertyInfo() { Name = nameof(Price.EndDate), MemberInfo = typeof(Price).GetProperty(nameof(Price.EndDate)) },
                }
            };

            //Act
            var filteredPricesString = SerializeAndRead(metadata, prices);

            //Assets
            Assert.NotNull(filteredPricesString);
            Assert.Contains("P1", filteredPricesString);
            Assert.Contains("P2", filteredPricesString);

            return Task.CompletedTask;
        }

        private string SerializeAndRead<T>(ExportedTypeMetadata metadata, IEnumerable<T> items)
        {
            var csvConfiguration = new CsvProviderConfiguration() { Configuration = new Configuration() };

            using (Stream stream = new MemoryStream())
            {
                using (var csvExportProvider = new CsvExportProvider(stream, csvConfiguration))
                {
                    csvExportProvider.Metadata = metadata;

                    foreach (var item in items)
                    {
                        csvExportProvider.WriteRecord(item);
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);

                return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}
