using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Core.Model;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class ExportedTypeMetadataTests
    {
        [Fact]
        public async Task GetFromType_Pricelist_BuiltCorrectly()
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

            // Check it doesn't contains recursive links
            Assert.DoesNotContain("Prices.Pricelist", props);
            Assert.DoesNotContain("Prices.Pricelist.Id", props);
            Assert.DoesNotContain("Assignments.Pricelist", props);

        }
    }
}
