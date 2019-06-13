using System.Threading.Tasks;
using Xunit;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;
using System.Collections;
using VirtoCommerce.ExportModule.Core.Services;
using Moq;

namespace VirtoCommerce.ExportModule.Tests
{

    public class PriceExportPagedDataSource : IPagedDataSource
    {
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public ExportDataQuery DataQuery { get; set; }

        public IEnumerable FetchNextPage()
        {
            throw new System.NotImplementedException();
        }

        public int GetTotalCount()
        {
            throw new System.NotImplementedException();
        }
    }

    public class PriceExportDataQuery : ExportDataQuery
    {

    }

    public class ExportTests
    {
        [Fact]
        public async Task ExportTest()
        {
            var mock = new Mock<IKnownExportTypesResolver>();
            mock.Setup(x => x.ResolveExportedTypeDefinition(It.IsAny<string>())).Returns(new ExportedTypeDefinition() { TypeName = typeof(VirtoCommerce.PricingModule.Core.Model.Price).FullName });
            ExportedTypeDefinition exportedTypeDefinition = mock.Object.ResolveExportedTypeDefinition("");

            exportedTypeDefinition
                .WithDataSourceFactory(dataQuery => new PriceExportPagedDataSource() { DataQuery = dataQuery })
                .WithMetadata(ExportedTypeMetadata.GetFromType<VirtoCommerce.PricingModule.Core.Model.Price>());

        }
    }
}
