using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Data.Extensions;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class ExportedTypeMetadataTests
    {
        [Fact]
        public Task GetPropertyNames_Plain_BuiltCorrectly()
        {
            var metadata = typeof(Pricelist).GetPropertyNames();
            var props = metadata.PropertyInfos.Select(x => x.FullName).ToArray();

            // Check if all own property detected
            Assert.Contains("Name", props);
            Assert.Contains("Description", props);
            Assert.Contains("Currency", props);
            Assert.Contains("CreatedDate", props);
            Assert.Contains("ModifiedDate", props);
            Assert.Contains("CreatedBy", props);
            Assert.Contains("ModifiedBy", props);
            Assert.Contains("Id", props);

            Assert.Equal(8, props.Length);

            return Task.CompletedTask;
        }

        [Fact]
        public Task GetNestedPropertyNames_BuiltCorrectly()
        {
            var metadata = typeof(Pricelist).GetNestedPropertyNames(nameof(Pricelist.Prices));
            var props = metadata.PropertyInfos.Select(x => x.FullName).ToArray();

            Assert.Contains("PricelistId", props);
            Assert.Contains("Currency", props);
            Assert.Contains("ProductId", props);
            Assert.Contains("Sale", props);
            Assert.Contains("List", props);
            Assert.Contains("MinQuantity", props);
            Assert.Contains("StartDate", props);
            Assert.Contains("EndDate", props);
            Assert.Contains("CreatedDate", props);
            Assert.Contains("ModifiedDate", props);
            Assert.Contains("CreatedBy", props);
            Assert.Contains("ModifiedBy", props);
            Assert.Contains("Id", props);

            Assert.Equal(13, props.Length);

            return Task.CompletedTask;
        }
    }
}
