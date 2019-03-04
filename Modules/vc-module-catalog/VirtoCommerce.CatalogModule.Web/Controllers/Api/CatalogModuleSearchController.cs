using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/search")]
    public class CatalogModuleSearchController : Controller
    {
        private readonly ICatalogSearchService _searchService;
        private readonly IProductSearchService _productSearchService;
        private readonly ICategorySearchService _categorySearchService;

        public CatalogModuleSearchController(ICatalogSearchService searchService, IProductSearchService productSearchService, ICategorySearchService categorySearchService)
        {
            _searchService = searchService;
            _productSearchService = productSearchService;
            _categorySearchService = categorySearchService;
        }


        /// <summary>
        /// Searches for the items by complex criteria
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<SearchResult>> Search([FromBody]CatalogListEntrySearchCriteria criteria)
        {
            //ApplyRestrictionsForCurrentUser(coreModelCriteria);
            var serviceResult = await _searchService.SearchAsync(criteria);

            return Ok(serviceResult);
        }

        [HttpPost]
        [Route("products")]
        public async Task<ActionResult<ProductSearchResult>> SearchProducts([FromBody]ProductSearchCriteria criteria)
        {
            criteria.ObjectType = KnownDocumentTypes.Product;
            var result = await _productSearchService.SearchAsync(criteria);
            return Ok(result);
        }

        [HttpPost]
        [Route("categories")]
        public async Task<ActionResult<CategorySearchResult>> SearchCategories([FromBody]CategorySearchCriteria criteria)
        {
            criteria.ObjectType = KnownDocumentTypes.Category;
            var result = await _categorySearchService.SearchAsync(criteria);
            return Ok(result);
        }
    }
}
