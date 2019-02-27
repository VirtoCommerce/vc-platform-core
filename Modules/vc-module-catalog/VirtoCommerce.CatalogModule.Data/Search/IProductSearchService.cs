using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public interface IProductSearchService
    {
        Task<ProductSearchResult> SearchAsync(ProductSearchCriteria criteria);
    }
}
