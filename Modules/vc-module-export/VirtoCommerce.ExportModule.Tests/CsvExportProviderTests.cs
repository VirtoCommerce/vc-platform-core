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
        public Task ExportPrices_FilterPlainMembers()
        {
            var date = new DateTime(2020, 10, 20);
            var prices = new List<Price>()
            {
                new Price {Id = "P1", PricelistId = "1", List = 25, CreatedDate = date, Currency = "USD" },
                new Price {Id = "P2", PricelistId = "1", List = 26, EndDate = date, ModifiedDate = date, Pricelist = new Pricelist() { Id = "1", Name = "Pricelist 1" } },
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
            Assert.Equal("Id,PricelistId,List,CreatedDate,EndDate\r\nP1,1,25,10/20/2020 00:00:00,\r\nP2,1,26,01/01/0001 00:00:00,10/20/2020 00:00:00\r\n", filteredPricesString);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelistAssignments_FilterPlainMembers()
        {
            var date = new DateTime(2020, 10, 20);
            var assignments = new List<PricelistAssignment>()
            {
                new PricelistAssignment {Id = "PA1", PricelistId = "1", Description = "PA1 Description", Priority = 1, Pricelist = new Pricelist() { Name = "Pricelist1"} },
                new PricelistAssignment {Id = "PA2", PricelistId = "2", EndDate = date, Description = "PA2 Description", ConditionExpression = "<expression/>"},
            };

            var metadata = new ExportedTypeMetadata()
            {
                PropertiesInfo = new[]
                {
                    new ExportTypePropertyInfo() { Name = nameof(PricelistAssignment.Id), MemberInfo = typeof(PricelistAssignment).GetProperty(nameof(PricelistAssignment.Id)) },
                    new ExportTypePropertyInfo() { Name = nameof(PricelistAssignment.Description), MemberInfo = typeof(PricelistAssignment).GetProperty(nameof(PricelistAssignment.Description)) },
                    new ExportTypePropertyInfo() { Name = nameof(PricelistAssignment.EndDate), MemberInfo = typeof(PricelistAssignment).GetProperty(nameof(PricelistAssignment.EndDate)) },
                    new ExportTypePropertyInfo() { Name = nameof(PricelistAssignment.Priority), MemberInfo = typeof(PricelistAssignment).GetProperty(nameof(PricelistAssignment.Priority)) },
                    new ExportTypePropertyInfo() { Name = nameof(PricelistAssignment.ConditionExpression), MemberInfo = typeof(PricelistAssignment).GetProperty(nameof(PricelistAssignment.ConditionExpression)) },
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
            var pricelists = new List<Pricelist>()
            {
                new Pricelist
                {
                    Id = "PL1",
                    Name = "Pricelist 1",
                    Currency = "USD",
                    Assignments = new List<PricelistAssignment>()
                    {
                        new PricelistAssignment {Id = "PA1_1", Description = "PA1_1 Description", Priority = 1 },
                        new PricelistAssignment {Id = "PA1_2", Description = "PA1_2 Description", Priority = 2 },
                    },
                    Prices = new List<Price>()
                    {
                        new Price {Id = "P1_1", List = 25 },
                        new Price {Id = "P1_2", List = 26 },
                    },
                },
                new Pricelist
                {
                    Id = "PL2",
                    Name = "Pricelist 2",
                    Currency = "EUR",
                    Assignments = new List<PricelistAssignment>()
                    {
                        new PricelistAssignment {Id = "PA2_1", Description = "PA2_1 Description", Priority = 1, CatalogId = "Cat1" },
                        new PricelistAssignment {Id = "PA2_2", Description = "PA2_2 Description", Priority = 2, CatalogId = "Cat2" },
                        new PricelistAssignment {Id = "PA2_3", Description = "PA2_3 Description", Priority = 10 },
                    },
                    Prices = new List<Price>()
                    {
                        new Price {Id = "P2_1", List = 12 },
                        new Price {Id = "P2_2", List = 17 },
                        new Price {Id = "P2_3", List = 19 },
                    },
                },
            };
            foreach (var pricelist in pricelists)
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
            }

            var metadata = new ExportedTypeMetadata()
            {
                PropertiesInfo = new[]
                {
                    new ExportTypePropertyInfo() { Name = nameof(Pricelist.Id), MemberInfo = typeof(Pricelist).GetProperty(nameof(Pricelist.Id)) },
                    new ExportTypePropertyInfo() { Name = nameof(Pricelist.Name), MemberInfo = typeof(Pricelist).GetProperty(nameof(Pricelist.Name)) },
                    new ExportTypePropertyInfo() { Name = nameof(Pricelist.Currency), MemberInfo = typeof(Pricelist).GetProperty(nameof(Pricelist.Currency)) },
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
            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportPricelists_UseCustomMap()
        {
            return Task.CompletedTask;
        }

        private string SerializeAndRead<T>(ExportedTypeMetadata metadata, IEnumerable<T> items, Configuration configuration = null)
        {
            var csvConfiguration = new CsvProviderConfiguration() { Configuration = configuration ?? new Configuration() };

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
