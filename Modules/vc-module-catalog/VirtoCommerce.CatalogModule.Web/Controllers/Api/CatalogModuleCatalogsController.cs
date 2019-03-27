using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/catalogs")]
    public class CatalogModuleCatalogsController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogModuleCatalogsController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        /// <summary>
        /// Get Catalogs list
        /// </summary>
        /// <remarks>Get common and virtual Catalogs list with minimal information included. Returns array of Catalog</remarks>
		[HttpGet]
        [Route("")]
        public async Task<ActionResult<Catalog[]>> GetCatalogs(string sort = null, int skip = 0, int take = 20)
        {

            //TODO
            //ApplyRestrictionsForCurrentUser(criteria);

            var stores = await _catalogService.GetCatalogsListAsync();
            var sortInfos = !string.IsNullOrEmpty(sort) ? SortInfo.Parse(sort) : new[] { new SortInfo { SortColumn = "Name" } };
            var result = stores.Skip(skip).Take(take).AsQueryable().OrderBySortInfos(sortInfos);

            return Ok(result);
        }

        /// <summary>
        /// Gets Catalog by id.
        /// </summary>
        /// <remarks>Gets Catalog by id with full information loaded</remarks>
        /// <param name="id">The Catalog id.</param>
		[HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Catalog>> Get(string id)
        {
            var catalog = (await _catalogService.GetByIdsAsync(new[] { id })).FirstOrDefault();
            if (catalog == null)
            {
                return NotFound();
            }
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, catalog);

            //var retVal = catalog.ToWebModel();

            //retVal.SecurityScopes = GetObjectPermissionScopeStrings(catalog);

            return Ok(catalog);
        }

        /// <summary>
        /// Gets the template for a new catalog.
        /// </summary>
        /// <remarks>Gets the template for a new common catalog</remarks>
        [HttpGet]
        [Route("getnew")]
        [Authorize(ModuleConstants.Security.Permissions.CatalogCreate)]
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

            //retVal.SecurityScopes = GetObjectPermissionScopeStrings(retVal);

            return Ok(retVal);
        }

        /// <summary>
        /// Gets the template for a new virtual catalog.
        /// </summary>
        [HttpGet]
        [Route("getnewvirtual")]
        [Authorize(ModuleConstants.Security.Permissions.CatalogCreate)]
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
            //retVal.SecurityScopes = GetObjectPermissionScopeStrings(retVal);
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
        [Authorize(ModuleConstants.Security.Permissions.CatalogCreate)]
        public async Task<ActionResult<Catalog>> Create([FromBody]Catalog catalog)
        {
            await _catalogService.SaveChangesAsync(new[] { catalog });
            //Need for UI permission checks
            //retVal.SecurityScopes = GetObjectPermissionScopeStrings(newCatalog);
            return Ok(catalog);
        }

        /// <summary>
        /// Updates the specified catalog.
        /// </summary>
        /// <remarks>Updates the specified catalog.</remarks>
        /// <param name="catalog">The catalog.</param>
        [HttpPut]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.CatalogUpdate)]
        public async Task<IActionResult> Update([FromBody]Catalog catalog)
        {
            await _catalogService.SaveChangesAsync(new[] { catalog });
            return NoContent();
        }

        /// <summary>
        /// Deletes catalog by id.
        /// </summary>
        /// <remarks>Deletes catalog by id</remarks>
        /// <param name="id">Catalog id.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(ModuleConstants.Security.Permissions.CatalogDelete)]
        public async Task<IActionResult> Delete(string id)
        {
            //TODO
            //var catalog = (await _catalogService.GetByIdsAsync(new [] { id})).FirstOrDefault();
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, catalog);

            await _catalogService.DeleteAsync(new[] { id });
            return NoContent();
        }
    }
}
