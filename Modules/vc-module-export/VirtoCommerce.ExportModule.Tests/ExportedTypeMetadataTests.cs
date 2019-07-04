using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class ExportedTypeMetadataTests
    {
        [Fact]
        public Task GetFromType_Pricelist_NameBuiltCorrectly()
        {
            var metadata = ExportedTypeMetadata.GetFromType<Pricelist>();
            var props = metadata.PropertiesInfo.Select(x => x.Name);

            // Check if all own property detected
            Assert.Contains("Name", props);
            Assert.Contains("Description", props);
            Assert.Contains("Currency", props);
            Assert.Contains("CreatedDate", props);
            Assert.Contains("ModifiedDate", props);
            Assert.Contains("CreatedBy", props);
            Assert.Contains("ModifiedBy", props);
            Assert.Contains("ShouldSerializeAuditableProperties", props);
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
            var metadata = ExportedTypeMetadata.GetFromType<Pricelist>();
            var props = metadata.PropertiesInfo.Select(x => x.ExportName);

            // Check if all own property detected
            Assert.Contains("Name", props);
            Assert.Contains("Description", props);
            Assert.Contains("Currency", props);
            Assert.Contains("CreatedDate", props);
            Assert.Contains("ModifiedDate", props);
            Assert.Contains("CreatedBy", props);
            Assert.Contains("ModifiedBy", props);
            Assert.Contains("ShouldSerializeAuditableProperties", props);
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
        public Task GetFromType_Pricelist_MemberTypeBuiltCorrectly()
        {
            var metadata = ExportedTypeMetadata.GetFromType<Pricelist>();
            var props = metadata.PropertiesInfo;

            Assert.Equal(1, props.Count(x => x.MemberInfo == typeof(Pricelist).GetProperty("Name")));
            Assert.Equal(1, props.Count(x => x.MemberInfo == typeof(Price).GetProperty("Id")));
            Assert.Equal(1, props.Count(x => x.MemberInfo == typeof(PricelistAssignment).GetProperty("Id")));

            return Task.CompletedTask;
        }
    }
}
