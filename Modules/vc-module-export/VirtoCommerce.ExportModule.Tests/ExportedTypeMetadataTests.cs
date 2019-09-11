using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Data.Extensions;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class ExportedTypeMetadataTests
    {
        [Fact]
        public Task GetFromType_Pricelist_NameBuiltCorrectly()
        {
            var metadata = typeof(Pricelist).GetPropertyNames(nameof(Pricelist.Prices), nameof(Pricelist.Assignments));
            var props = metadata.PropertyInfos.Select(x => x.FullName);

            // Check if all own property detected
            Assert.Contains("Name", props);
            Assert.Contains("Description", props);
            Assert.Contains("Currency", props);
            Assert.Contains("CreatedDate", props);
            Assert.Contains("ModifiedDate", props);
            Assert.Contains("CreatedBy", props);
            Assert.Contains("ModifiedBy", props);
            Assert.Contains("Id", props);

            // Reference properties
            Assert.Contains("Prices.Id", props);
            Assert.Contains("Prices.PricelistId", props);
            Assert.Contains("Assignments.Id", props);
            Assert.Contains("Assignments.PricelistId", props);

            // Check it doesn't contains recursive links
            Assert.DoesNotContain("Prices.Pricelist", props);
            Assert.DoesNotContain("Prices.Pricelist.Id", props);
            Assert.DoesNotContain("Assignments.Pricelist", props);
            Assert.DoesNotContain("Assignments", props);

            return Task.CompletedTask;
        }

        [Fact]
        public Task GetFromType_Pricelist_ExportNameBuiltCorrectly()
        {
            var metadata = typeof(Pricelist).GetPropertyNames(nameof(Pricelist.Prices), nameof(Pricelist.Assignments));
            var props = metadata.PropertyInfos.Select(x => x.DisplayName);

            // Check if all own property detected
            Assert.Contains("Name", props);
            Assert.Contains("Description", props);
            Assert.Contains("Currency", props);
            Assert.Contains("CreatedDate", props);
            Assert.Contains("ModifiedDate", props);
            Assert.Contains("CreatedBy", props);
            Assert.Contains("ModifiedBy", props);
            Assert.Contains("Id", props);

            // Reference properties
            Assert.Contains("Prices.Id", props);
            Assert.Contains("Prices.PricelistId", props);
            Assert.Contains("Assignments.Id", props);
            Assert.Contains("Assignments.PricelistId", props);

            // Check it doesn't contains recursive links
            Assert.DoesNotContain("Prices.Pricelist", props);
            Assert.DoesNotContain("Prices.Pricelist.Id", props);
            Assert.DoesNotContain("Assignments.Pricelist", props);
            Assert.DoesNotContain("Assignments", props);

            return Task.CompletedTask;
        }


        [Fact]
        public Task GetFromType_Pricelist_WithoutReferences_BuiltCorrectly()
        {
            var metadata = typeof(Pricelist).GetPropertyNames();
            var props = metadata.PropertyInfos.Select(x => x.FullName);

            // Check if all own property detected
            Assert.Contains("Name", props);
            Assert.Contains("Description", props);
            Assert.Contains("Currency", props);
            Assert.Contains("CreatedDate", props);
            Assert.Contains("ModifiedDate", props);
            Assert.Contains("CreatedBy", props);
            Assert.Contains("ModifiedBy", props);
            Assert.Contains("Id", props);

            // And does not contain nested properties
            Assert.DoesNotContain("Prices", props);
            Assert.DoesNotContain("Prices.Id", props);
            Assert.DoesNotContain("Prices.PricelistId", props);
            Assert.DoesNotContain("Assignments.Id", props);
            Assert.DoesNotContain("Assignments.PricelistId", props);

            return Task.CompletedTask;
        }
    }
}
