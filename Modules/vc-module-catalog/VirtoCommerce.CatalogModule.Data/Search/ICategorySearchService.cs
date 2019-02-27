using System.Threading.Tasks;
using VirtoCommerce.Domain.Catalog.Model.Search;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public interface ICategorySearchService
    {
        Task<CategorySearchResult> SearchAsync(CategorySearchCriteria criteria);
    }
}
