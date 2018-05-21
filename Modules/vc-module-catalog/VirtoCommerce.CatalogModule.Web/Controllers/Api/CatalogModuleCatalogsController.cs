using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Security;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Produces("application/json")]
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
        [ProducesResponseType(typeof(GenericSearchResult<Catalog>), 200)]
        public ActionResult GetCatalogs()
        {           
            //TODO:
            //ApplyRestrictionsForCurrentUser(criteria);
            var retVal = new List<Catalog>();
            foreach (var catalog in _catalogService.GetAllCatalogs())
            {
               //webCatalog.SecurityScopes = GetObjectPermissionScopeStrings(catalog);
                retVal.Add(catalog);
            }
            return Ok(retVal.ToArray());
        }

        /// <summary>
        /// Gets Catalog by id.
        /// </summary>
        /// <remarks>Gets Catalog by id with full information loaded</remarks>
        /// <param name="id">The Catalog id.</param>
		[HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Catalog), 200)]
        public ActionResult Get(string id)
        {
            var catalog = _catalogService.GetByIds(new[] { id }).FirstOrDefault();
            if (catalog == null)
            {
                return NotFound();
            }
            //TODO:
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, catalog);

            //TODO:
            //retVal.SecurityScopes = GetObjectPermissionScopeStrings(catalog);
            return Ok(catalog);
        }

        /// <summary>
        /// Gets the template for a new catalog.
        /// </summary>
        /// <remarks>Gets the template for a new common catalog</remarks>
        [HttpGet]
        [Route("getnew")]
        [ProducesResponseType(typeof(Catalog), 200)]
        [Authorize(SecurityConstants.Permissions.Create)]
        public ActionResult GetNewCatalog()
        {
            var retVal = AbstractTypeFactory<Catalog>.TryCreateInstance();

            retVal.Name = "New catalog";
            retVal.Languages = new List<CatalogLanguage>
                {
                    new CatalogLanguage
                    {
                        IsDefault = true,
                        LanguageCode = "en-US"
                    }
                };

            //TODO:
            //retVal.SecurityScopes = GetObjectPermissionScopeStrings(retVal);
            return Ok(retVal);
        }

        /// <summary>
        /// Gets the template for a new virtual catalog.
        /// </summary>
        [HttpGet]
        [Route("getnewvirtual")]
        [ProducesResponseType(typeof(Catalog), 200)]
        [Authorize(SecurityConstants.Permissions.Create)]
        public ActionResult GetNewVirtualCatalog()
        {
            var retVal = AbstractTypeFactory<Catalog>.TryCreateInstance();
            retVal.IsVirtual = true;
            retVal.Languages = new List<CatalogLanguage>
                {
                    new CatalogLanguage
                    {
                        IsDefault = true,
                        LanguageCode = "en-US"
                    }
                };

           //TODO:
           // retVal.SecurityScopes = GetObjectPermissionScopeStrings(retVal);
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
        [ProducesResponseType(typeof(Catalog), 200)]
        [Authorize(SecurityConstants.Permissions.Create)]
        public ActionResult Create([FromBody] Catalog catalog)
        {
            _catalogService.SaveChanges(new [] { catalog });
            //TODO:
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
        [ProducesResponseType(typeof(void), 200)]
        public ActionResult Update([FromBody] Catalog catalog)
        {
            _catalogService.SaveChanges(new[] { catalog });
            //TODO:
            //  CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Update, catalog);
            return Ok();
        }

        /// <summary>
        /// Deletes catalog by id.
        /// </summary>
        /// <remarks>Deletes catalog by id</remarks>
        /// <param name="id">Catalog id.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        public ActionResult Delete(string id)
        {
            var catalog = _catalogService.GetByIds(new[] { id });
            //TODO:
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, catalog);

            _catalogService.Delete(new[] { id });
            return Ok();
        }

    }
}
