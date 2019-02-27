using System.Threading.Tasks;
using VirtoCommerce.Domain.Catalog.Model.Search;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public interface IProductSearchService
    {
        Task<ProductSearchResult> SearchAsync(ProductSearchCriteria criteria);
    }
}
