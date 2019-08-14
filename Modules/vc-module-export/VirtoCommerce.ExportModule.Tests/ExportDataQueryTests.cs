using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Extensions;
using VirtoCommerce.ExportModule.Tests.PricingModuleModelMocks;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class ExportDataQueryTests
    {
        private readonly DateTime date = new DateTime(2020, 10, 20);

        [Fact]
        public Task ExportProperties_ClearCircleReferences()
        {
            //Arrange
            var pricelist = CreateSamplePricelist();

            var dataQuery = new PricelistExportDataQuery()
            {
                IncludedProperties = new[]
                {
                    new ExportedTypePropertyInfo() { FullName = "Name"} ,
                    new ExportedTypePropertyInfo() { FullName = "Prices.Id"} ,
                    new ExportedTypePropertyInfo() { FullName = "Id"},
                    new ExportedTypePropertyInfo() { FullName = "Assignments.Id"} ,
                },
            };

            //Act
            dataQuery.FilterProperties(pricelist);

            //Assert
            Assert.Null(pricelist.Prices.First().Pricelist);
            Assert.Null(pricelist.Assignments.First().Pricelist);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportProperties_AllExported()
        {
            //Arrange
            var pricelist = CreateSamplePricelist();

            var dataQuery = new PricelistExportDataQuery()
            {
                IncludedProperties = new[]
                {
                    new ExportedTypePropertyInfo() { FullName = "Id"},
                    new ExportedTypePropertyInfo() { FullName = "Name"},
                    new ExportedTypePropertyInfo() { FullName = "Prices.Id"} ,
                    new ExportedTypePropertyInfo() { FullName = "Prices.PricelistId"} ,
                    new ExportedTypePropertyInfo() { FullName = "Prices.List"} ,
                    new ExportedTypePropertyInfo() { FullName = "Prices.CreatedDate"} ,
                    new ExportedTypePropertyInfo() { FullName = "Assignments.Id"} ,
                    new ExportedTypePropertyInfo() { FullName = "Assignments.PricelistId"} ,
                }
            };

            //Act
            dataQuery.FilterProperties(pricelist);

            //Assert
            Assert.Equal("PL1", pricelist.Name);
            Assert.Equal("1", pricelist.Id);

            Assert.Equal(2, pricelist.Prices.Count);
            Assert.Equal(2, pricelist.Prices.Count(x => x.PricelistId == "1"));
            Assert.Equal(1, pricelist.Prices.Count(x => x.Id == "P1"));
            Assert.Equal(1, pricelist.Prices.Count(x => x.Id == "P2"));
            Assert.Equal(1, pricelist.Prices.Count(x => x.List == 25));
            Assert.Equal(1, pricelist.Prices.Count(x => x.List == 26));
            Assert.Equal(1, pricelist.Prices.Count(x => x.CreatedDate == date));
            Assert.Equal(1, pricelist.Prices.Count(x => x.CreatedDate == default(DateTime)));

            Assert.Equal(2, pricelist.Assignments.Count);
            Assert.Equal(2, pricelist.Assignments.Count(x => x.PricelistId == "1"));
            Assert.Equal(1, pricelist.Assignments.Count(x => x.Id == "A1"));
            Assert.Equal(1, pricelist.Assignments.Count(x => x.Id == "A2"));

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportProperties_FilterPlainProperties()
        {
            //Arrange
            var pricelist = CreateSamplePricelist();

            var dataQuery = new PricelistExportDataQuery()
            {
                IncludedProperties = new[]
                {
                    new ExportedTypePropertyInfo() { FullName = "Id"},
                }
            };

            //Act
            dataQuery.FilterProperties(pricelist);

            //Assert
            Assert.Equal("1", pricelist.Id);
            Assert.Null(pricelist.Name);

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportProperties_FilterCollectionProperties()
        {
            //Arrange
            var pricelist = CreateSamplePricelist();

            var dataQuery = new PricelistExportDataQuery()
            {
                IncludedProperties = new[]
                {
                    new ExportedTypePropertyInfo() { FullName = "Id"},
                    new ExportedTypePropertyInfo() { FullName = "Name"},
                    new ExportedTypePropertyInfo() { FullName = "Assignments.Id"} ,
                    new ExportedTypePropertyInfo() { FullName = "Assignments.PricelistId"} ,
                }
            };

            //Act
            dataQuery.FilterProperties(pricelist);

            //Assert
            Assert.Equal("PL1", pricelist.Name);
            Assert.Equal("1", pricelist.Id);

            Assert.Null(pricelist.Prices);

            Assert.Equal(2, pricelist.Assignments.Count);
            Assert.Equal(2, pricelist.Assignments.Count(x => x.PricelistId == "1"));
            Assert.Equal(1, pricelist.Assignments.Count(x => x.Id == "A1"));
            Assert.Equal(1, pricelist.Assignments.Count(x => x.Id == "A2"));

            return Task.CompletedTask;
        }

        [Fact]
        public Task ExportProperties_FilterCollectionNestedProperties()
        {
            //Arrange
            var pricelist = CreateSamplePricelist();

            var dataQuery = new PricelistExportDataQuery()
            {
                IncludedProperties = new[]
                {
                    new ExportedTypePropertyInfo() { FullName = "Id"},
                    new ExportedTypePropertyInfo() { FullName = "Name"},
                    new ExportedTypePropertyInfo() { FullName = "Prices.List"} ,
                    new ExportedTypePropertyInfo() { FullName = "Assignments.Id"} ,
                }
            };

            //Act
            dataQuery.FilterProperties(pricelist);

            //Assert
            Assert.Equal("PL1", pricelist.Name);
            Assert.Equal("1", pricelist.Id);

            Assert.Equal(2, pricelist.Prices.Count);
            Assert.Equal(1, pricelist.Prices.Count(x => x.List == 25));
            Assert.Equal(1, pricelist.Prices.Count(x => x.List == 26));

            Assert.Equal(2, pricelist.Assignments.Count);
            Assert.Equal(1, pricelist.Assignments.Count(x => x.Id == "A1"));
            Assert.Equal(1, pricelist.Assignments.Count(x => x.Id == "A2"));

            return Task.CompletedTask;
        }


        private Pricelist CreateSamplePricelist()
        {
            var pricelist = new Pricelist()
            {
                Id = "1",
                Name = "PL1",
                Prices = new List<Price>()
                {
                    new Price {Id = "P1", PricelistId = "1", List = 25, CreatedDate = date, },
                    new Price {Id = "P2", PricelistId = "1", List = 26, },
                },
                Assignments = new List<PricelistAssignment>
                {
                    new PricelistAssignment {Id = "A1", PricelistId = "1", },
                    new PricelistAssignment {Id = "A2", PricelistId = "1", },
                },
            };
            foreach (var price in pricelist.Prices)
            {
                price.Pricelist = pricelist;
            }

            foreach (var assignment in pricelist.Assignments)
            {
                assignment.Pricelist = pricelist;
            }

            return pricelist;
        }
    }
}
