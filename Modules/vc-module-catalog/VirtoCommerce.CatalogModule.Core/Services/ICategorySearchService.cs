using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICategorySearchService
    {
        Task<CategorySearchResult> SearchAsync(CategorySearchCriteria criteria);
    }
}
