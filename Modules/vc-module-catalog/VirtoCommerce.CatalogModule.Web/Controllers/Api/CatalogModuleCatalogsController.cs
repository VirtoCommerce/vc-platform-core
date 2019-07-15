using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Web.Authorization;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/catalogs")]
    public class CatalogModuleCatalogsController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly IAuthorizationService _authorizationService;

        public CatalogModuleCatalogsController(
            ICatalogService catalogService
            , ICatalogSearchService catalogSearchService
            , IAuthorizationService authorizationService
            )
        {
            _catalogService = catalogService;
            _catalogSearchService = catalogSearchService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Get Catalogs list
        /// </summary>
        /// <remarks>Get common and virtual Catalogs list with minimal information included. Returns array of Catalog</remarks>
		[HttpGet]
        [Route("")]
        public async Task<ActionResult<Catalog[]>> GetCatalogs(string sort = null, int skip = 0, int take = 20)
        {
            var criteria = AbstractTypeFactory<CatalogSearchCriteria>.TryCreateInstance();
            criteria.Sort = sort;
            criteria.Skip = skip;
            criteria.Take = take;

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, criteria, new CatalogAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            var result = await _catalogSearchService.SearchCatalogsAsync(criteria);
            return Ok(result.Results);
        }

        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<CatalogSearchResult>> SearchCatalogs([FromBody] CatalogSearchCriteria criteria)
        {
            var result = await _catalogSearchService.SearchCatalogsAsync(criteria);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, criteria, new CatalogAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets Catalog by id.
        /// </summary>
        /// <remarks>Gets Catalog by id with full information loaded</remarks>
        /// <param name="id">The Catalog id.</param>
		[HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Catalog>> GetCatalog(string id)
        {
            var catalog = (await _catalogService.GetByIdsAsync(new[] { id }, CatalogResponseGroup.Full.ToString())).FirstOrDefault();
         
            if (catalog == null)
            {
                return NotFound();
            }
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, catalog, new CatalogAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }
            return Ok(catalog);
        }

        /// <summary>
        /// Gets the template for a new catalog.
        /// </summary>
        /// <remarks>Gets the template for a new common catalog</remarks>
        [HttpGet]
        [Route("getnew")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public ActionResult<Catalog> GetNewCatalog()
        {
            var retVal = new Catalog
            {
                Name = "New catalog",
                Languages = new List<CatalogLanguage>
                {
                    new CatalogLanguage
                    {
                        IsDefault = true,
                        LanguageCode = "en-US"
                    }
                }
            };
            return Ok(retVal);
        }

        /// <summary>
        /// Gets the template for a new virtual catalog.
        /// </summary>
        [HttpGet]
        [Route("getnewvirtual")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public ActionResult<Catalog> GetNewVirtualCatalog()
        {
            var retVal = new Catalog
            {
                Name = "New virtual catalog",
                IsVirtual = true,
                Languages = new List<CatalogLanguage>
                {
                    new CatalogLanguage
                    {
                        IsDefault = true,
                        LanguageCode = "en-US"
                    }
                }
            };
            return Ok(retVal);
        }

        /// <summary>
        /// Creates the specified catalog.
        /// </summary>
        /// <remarks>Creates the specified catalog</remarks>
        /// <param name="catalog">The catalog to create</param>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
		[HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<Catalog>> CreateCatalog([FromBody]Catalog catalog)
        {
            await _catalogService.SaveChangesAsync(new[] { catalog });
            return Ok(catalog);
        }

        /// <summary>
        /// Updates the specified catalog.
        /// </summary>
        /// <remarks>Updates the specified catalog.</remarks>
        /// <param name="catalog">The catalog.</param>
        [HttpPut]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult> UpdateCatalog([FromBody]Catalog catalog)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, catalog, new CatalogAuthorizationRequirement(ModuleConstants.Security.Permissions.Update));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }
            await _catalogService.SaveChangesAsync(new[] { catalog });
            return Ok(catalog);
        }

        /// <summary>
        /// Deletes catalog by id.
        /// </summary>
        /// <remarks>Deletes catalog by id</remarks>
        /// <param name="id">Catalog id.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> DeleteCatalog(string id)
        {
            var catalog = (await _catalogService.GetByIdsAsync(new [] { id})).FirstOrDefault();
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, catalog, new CatalogAuthorizationRequirement(ModuleConstants.Security.Permissions.Delete));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            await _catalogService.DeleteAsync(new[] { id });
            return NoContent();
        }
    }
}
