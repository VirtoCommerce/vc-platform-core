using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/search")]
    public class CatalogModuleIndexedSearchController : Controller
    {
        private readonly IProductIndexedSearchService _productIndexedSearchService;
        private readonly ICategoryIndexedSearchService _categoryIndexedSearchService;

        public CatalogModuleIndexedSearchController(
            IProductIndexedSearchService productIndexedSearchService
            , ICategoryIndexedSearchService categoryIndexedSearchService)
        {
            _productIndexedSearchService = productIndexedSearchService;
            _categoryIndexedSearchService = categoryIndexedSearchService;
        }

        [HttpPost]
        [Route("products")]
        public async Task<ActionResult<ProductIndexedSearchResult>> SearchProducts([FromBody]ProductIndexedSearchCriteria criteria)
        {
            criteria.ObjectType = KnownDocumentTypes.Product;
            var result = await _productIndexedSearchService.SearchAsync(criteria);
            return Ok(result);
        }

        [HttpPost]
        [Route("categories")]
        public async Task<ActionResult<CategoryIndexedSearchResult>> SearchCategories([FromBody]CategoryIndexedSearchCriteria criteria)
        {
            criteria.ObjectType = KnownDocumentTypes.Category;
            var result = await _categoryIndexedSearchService.SearchAsync(criteria);
            return Ok(result);
        }
    }
}
