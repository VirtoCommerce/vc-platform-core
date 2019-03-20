using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICatalogSearchService
    {
        Task<SearchResult> SearchAsync(CatalogListEntrySearchCriteria criteria);
    }
}
