using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICatalogSearchService
    {
        SearchResult Search(SearchCriteria criteria);
    }
}
