using System.Threading.Tasks;
using Xunit;
using VirtoCommerce.ExportModule.Core.Model;
using System.Linq;

namespace VirtoCommerce.ExportModule.Tests
{
    public class ExportedTypeMetadataTests
    {
        [Fact]
        public async Task GetFromType_()
        {
            var metadata = ExportedTypeMetadata.GetFromType<Price>();
            var props = metadata.PropertiesInfo.Select(x => x.Name);

            Assert.Contains("Currency", props); // Check if own property detected
            Assert.Contains("Pricelist.Currency", props); // Check if property is an entity detected
            Assert.Contains("Pricelist.Assignments.Name", props); // Check if nested type is an enumerable of entities and a property of such entity detected too

        }
    }
}
