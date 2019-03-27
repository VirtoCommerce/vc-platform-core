using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Core.Search
{
    public interface IProductIndexedSearchService
    {
        Task<ProductIndexedSearchResult> SearchAsync(ProductIndexedSearchCriteria criteria);
    }
}
