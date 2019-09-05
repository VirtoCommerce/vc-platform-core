using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Tests.ComplexExportPagedDataSourceTests.Mocks
{
    public interface ITestSearchService
    {
        Task<ExportableSearchResult> SearchAsync(TestSearchCriteria searchCriteria);
    }
}
