using System.Threading.Tasks;

namespace VirtoCommerce.ExportModule.Tests.ComplexExportPagedDataSourceTests.Mocks
{
    public interface IComplexDataSearchService
    {
        Task<ComplexSearchResult> SearchAsync(ComplexSearchCriteria searchCriteria);
    }
}
