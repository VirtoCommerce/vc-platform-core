using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Core.Search
{
    public interface ICatalogSearchService
    {
        Task<CatalogSearchResult> SearchCatalogsAsync(CatalogSearchCriteria criteria);
    }
}
