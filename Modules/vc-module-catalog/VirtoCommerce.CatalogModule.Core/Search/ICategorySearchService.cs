using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Core.Search
{
    public interface ICategorySearchService
    {
        Task<CategorySearchResult> SearchAsync(CategorySearchCriteria criteria);
    }
}
