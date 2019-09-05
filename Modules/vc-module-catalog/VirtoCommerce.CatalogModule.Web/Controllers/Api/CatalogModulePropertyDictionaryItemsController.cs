using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/dictionaryitems")]
    public class CatalogModulePropertyDictionaryItemsController : Controller
    {
        private readonly IPropertyDictionaryItemSearchService _propertyDictionarySearchService;
        private readonly IPropertyDictionaryItemService _propertyDictionaryService;

        public CatalogModulePropertyDictionaryItemsController(IPropertyDictionaryItemSearchService propertyDictionarySearchService,
                                                             IPropertyDictionaryItemService propertyDictionaryService)
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
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<PropertyDictionaryItem[]>> SearchPropertyDictionaryItems([FromBody]PropertyDictionaryItemSearchCriteria criteria)
        {
            var result = await _propertyDictionarySearchService.SearchAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Creates or updates the specified property dictionary items
        /// </summary>
        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult> SaveChanges([FromBody]PropertyDictionaryItem[] propertyDictItems)
        {
            await _propertyDictionaryService.SaveChangesAsync(propertyDictItems);
            return Ok();
        }

        /// <summary>
        /// Delete property dictionary items by ids
        /// </summary>
        /// <param name="ids">The identifiers of objects that needed to be deleted</param>
        [HttpDelete]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> DeletePropertyDictionaryItems([FromQuery] string[] ids)
        {
            await _propertyDictionaryService.DeleteAsync(ids);
            return Ok();
        }
    }
}
