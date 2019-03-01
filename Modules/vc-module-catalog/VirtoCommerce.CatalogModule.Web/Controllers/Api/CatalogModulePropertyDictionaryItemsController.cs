using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.CatalogModule.Web.Converters;
using VirtoCommerce.CatalogModule.Web.Security;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Web.Security;
using moduleModel = VirtoCommerce.Domain.Catalog.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [RoutePrefix("api/catalog/dictionaryitems")]
    public class CatalogModulePropertyDictionaryItemsController : CatalogBaseController
    {
        private readonly IProperyDictionaryItemSearchService _propertyDictionarySearchService;
        private readonly IProperyDictionaryItemService _propertyDictionaryService;

        public CatalogModulePropertyDictionaryItemsController(ISecurityService securityService, IPermissionScopeService permissionScopeService, IProperyDictionaryItemSearchService propertyDictionarySearchService,
                                                             IProperyDictionaryItemService propertyDictionaryService)
            : base(securityService, permissionScopeService)
        {
            _propertyDictionarySearchService = propertyDictionarySearchService;
            _propertyDictionaryService = propertyDictionaryService;
        }

        /// <summary>
        /// Search property dictionary items
        /// </summary>
        /// <param name="criteria">The search criteria</param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [ResponseType(typeof(moduleModel.PropertyDictionaryItem[]))]
        [CheckPermission(Permission = CatalogPredefinedPermissions.Read)]
        public IHttpActionResult SearchPropertyDictionaryItems(PropertyDictionaryItemSearchCriteria criteria)
        {
            var result = _propertyDictionarySearchService.Search(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Creates or updates the specified property dictionary items
        /// </summary>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = CatalogPredefinedPermissions.Create)]
        public IHttpActionResult SaveChanges(moduleModel.PropertyDictionaryItem[] propertyDictItems)
        {
            _propertyDictionaryService.SaveChanges(propertyDictItems);
            return Ok();
        }

        /// <summary>
        /// Delete property dictionary items by ids
        /// </summary>
        /// <param name="ids">The identifiers of objects that needed to be deleted</param>
        [HttpDelete]
        [Route("")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = CatalogPredefinedPermissions.Delete)]
        public IHttpActionResult Delete([FromUri] string[] ids)
        {
            _propertyDictionaryService.Delete(ids);
            return Ok();
        }
    }
}
