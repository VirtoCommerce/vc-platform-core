using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Core.Search
{
    public interface ICategoryIndexedSearchService
    {
        Task<CategoryIndexedSearchResult> SearchAsync(CategoryIndexedSearchCriteria criteria);
    }
}
