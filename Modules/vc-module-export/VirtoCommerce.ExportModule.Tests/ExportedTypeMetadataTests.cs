using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class ExportedTypeMetadataTests
    {
        [Fact]
        public async Task GetFromType_Pricelist_NameBuiltCorrectly()
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
            Assert.Contains("Prices.Id", props);
            Assert.Contains("Assignments.Id", props);


            // Check it doesn't contains recursive links
            Assert.DoesNotContain("Prices.Pricelist", props);
            Assert.DoesNotContain("Prices.Pricelist.Id", props);
            Assert.DoesNotContain("Assignments.Pricelist", props);
            Assert.DoesNotContain("Assignments", props);
        }

        [Fact]
        public async Task GetFromType_Pricelist_ExportNameBuiltCorrectly()
        {
            var metadata = ExportedTypeMetadata.GetFromType<Pricelist>();
            var props = metadata.PropertiesInfo.Select(x => x.ExportName);

            // Check if all own property detected
            Assert.Contains("Pricelist.Name", props);
            Assert.Contains("Pricelist.Description", props);
            Assert.Contains("Pricelist.Currency", props);
            Assert.Contains("Pricelist.CreatedDate", props);
            Assert.Contains("Pricelist.ModifiedDate", props);
            Assert.Contains("Pricelist.CreatedBy", props);
            Assert.Contains("Pricelist.ModifiedBy", props);
            Assert.Contains("Pricelist.ShouldSerializeAuditableProperties", props);
            Assert.Contains("Pricelist.Id", props);

            // Reference properties
            Assert.Contains("Pricelist.Prices.Id", props);
            Assert.Contains("Pricelist.Assignments.Id", props);

            // Check it doesn't contains recursive links
            Assert.DoesNotContain("Pricelist.Prices.Pricelist", props);
            Assert.DoesNotContain("Pricelist.Prices.Pricelist.Id", props);
            Assert.DoesNotContain("Pricelist.Assignments.Pricelist", props);
            Assert.DoesNotContain("Assignments", props);
        }


        [Fact]
        public async Task GetFromType_Pricelist_MemberTypeBuiltCorrectly()
        {
            var metadata = ExportedTypeMetadata.GetFromType<Pricelist>();
            var props = metadata.PropertiesInfo;

            Assert.Equal(1, props.Count(x => x.MemberInfo == typeof(Pricelist).GetProperty("Name")));
            Assert.Equal(1, props.Count(x => x.MemberInfo == typeof(Price).GetProperty("Id")));
            Assert.Equal(1, props.Count(x => x.MemberInfo == typeof(PricelistAssignment).GetProperty("Id")));
        }
    }
}
