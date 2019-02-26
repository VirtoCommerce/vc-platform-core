using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.CatalogModule.Core2.Model.Search;
using VirtoCommerce.CatalogModule.Core2.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web2.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/catalog/properties")]
    public class CatalogModulePropertiesController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IPropertySearchService _propertySearchService;

        public CatalogModulePropertiesController(IPropertyService propertyService, IPropertySearchService propertySearchService)
        {
            _propertyService = propertyService;
            _propertySearchService = propertySearchService;
        }

        /// <summary>
        /// Gets all dictionary values that specified property can have.
        /// </summary>
        /// <param name="propertyId">The property id.</param>
        /// <param name="keyword">The keyword. (Optional)</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{propertyId}/values/search")]
        [ProducesResponseType(typeof(GenericSearchResult<PropertyValue>), 200)]
        public ActionResult GetPropertyValues([FromBody] PropertyDictionaryValueSearchCriteria criteria)
        {
            //Need to return PropertyValue as it's more convenient in UI
            var result = _propertySearchService.SearchPropertyDictionaryValues(criteria);     
            return Ok(result);
        }


        /// <summary>
        /// Gets property metainformation by id.
        /// </summary>
        /// <param name="propertyId">The property id.</param>
		[HttpGet]
        [Route("{propertyId}")]
        [ProducesResponseType(typeof(Property), 200)]
        public ActionResult GetById(string propertyId)
        {
            var result = _propertyService.GetByIds(new[] { propertyId });
            //TODO:
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, property);
            //TODO:
            // retVal.IsManageable = true;
            return Ok(result);
        }

        /// <summary>
        /// Creates or updates the specified property.
        /// </summary>
        /// <remarks>If property.IsNew == True, a new property is created. It's updated otherwise</remarks>
        /// <param name="property">The property.</param>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Property), 200)]
        public ActionResult SaveProperty([FromBody] Property property)
        {
            //TODO:
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, moduleProperty);
            _propertyService.SaveChanges(new[] { property });
            return Ok(property);
        }


        /// <summary>
        /// Deletes property by id.
        /// </summary>
        /// <param name="id">The property id.</param>
        /// <param name="doDeleteValues">Flag indicating to remove property values from objects as well</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(200)]
        public ActionResult Delete(string id, bool doDeleteValues = false)
        {
            var property = _propertyService.GetByIds(new[] { id });

            //TODO:
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, property);

            _propertyService.Delete(new[] { id } , doDeleteValues);
            return Ok();
        }


    }
}
