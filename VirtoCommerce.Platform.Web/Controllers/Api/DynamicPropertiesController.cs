using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/platform/dynamic")]
    [Authorize(SecurityConstants.Permissions.DynamicPropertiesQuery)]
    public class DynamicPropertiesController : Controller
    {
        private readonly IDynamicPropertyRegistrar _dynamicPropertyRegistrar;
        private readonly IDynamicPropertyService _dynamicPropertyService;
        private readonly IDynamicPropertySearchService _dynamicPropertySearchService;

        public DynamicPropertiesController(IDynamicPropertyRegistrar dynamicPropertyRegistrar, IDynamicPropertyService dynamicPropertyService, IDynamicPropertySearchService dynamicPropertySearchService)
        {
            _dynamicPropertyService = dynamicPropertyService;
            _dynamicPropertySearchService = dynamicPropertySearchService;
            _dynamicPropertyRegistrar = dynamicPropertyRegistrar;
        }

        /// <summary>
        /// Get object types which support dynamic properties
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("types")]
        [ProducesResponseType(typeof(string[]), 200)]
        public IActionResult GetObjectTypes()
        {
            return Ok(_dynamicPropertyRegistrar.AllRegisteredTypeNames);
        }

        /// <summary>
        /// Get dynamic properties registered for object type
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("types/{typeName}/properties")]
        [ProducesResponseType(typeof(DynamicProperty[]), 200)]
        public async Task<IActionResult> Search([FromBody] DynamicPropertySearchCriteria criteria)
        {
            var result = await _dynamicPropertySearchService.SearchDynamicPropertiesAsync(criteria);
            return Ok(result.Results);
        }

        /// <summary>
        /// Add new dynamic property
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("properties")]
        [ProducesResponseType(typeof(DynamicProperty), 200)]
        [Authorize(SecurityConstants.Permissions.DynamicPropertiesCreate)]
        public async Task<IActionResult> CreatePropertyAsync([FromBody]DynamicProperty property)
        {
            var result = await _dynamicPropertyService.SaveDynamicPropertiesAsync(new[] { property });
            return Ok(result.FirstOrDefault());
        }

        /// <summary>
        /// Update existing dynamic property
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("properties")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(SecurityConstants.Permissions.DynamicPropertiesUpdate)]
        public async Task<IActionResult> UpdatePropertyAsync([FromBody]DynamicProperty property)
        {
            await _dynamicPropertyService.SaveDynamicPropertiesAsync(new[] { property });
            return Ok();
        }

        /// <summary>
        /// Delete dynamic property
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("properties")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(SecurityConstants.Permissions.DynamicPropertiesDelete)]
        public async Task<IActionResult> DeletePropertyAsync([FromQuery] string[] propertyIds)
        {
            await _dynamicPropertyService.DeleteDynamicPropertiesAsync(propertyIds);
            return Ok();
        }

        /// <summary>
        /// Get dictionary items
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("dictionaryitems/search")]
        [ProducesResponseType(typeof(DynamicPropertyDictionaryItem[]), 200)]
        public async Task<IActionResult> GetDictionaryItems([FromBody]DynamicPropertyDictionaryItemSearchCriteria criteria)
        {
            var result = await _dynamicPropertySearchService.SearchDictionaryItemsAsync(criteria);
            return Ok(result.Results);
        }

        /// <summary>
        /// Add or update dictionary items
        /// </summary>
        /// <remarks>
        /// Fill item ID to update existing item or leave it empty to create a new item.
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route("dictionaryitems")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(SecurityConstants.Permissions.DynamicPropertiesUpdate)]
        public async Task<IActionResult> SaveDictionaryItemsAsync([FromBody]DynamicPropertyDictionaryItem[] items)
        {
            await _dynamicPropertyService.SaveDictionaryItemsAsync(items);
            return Ok();
        }

        /// <summary>
        /// Delete dictionary items
        /// </summary>
        /// <param name="ids">IDs of dictionary items to delete.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("dictionaryitems")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(SecurityConstants.Permissions.DynamicPropertiesUpdate)]
        public async Task<IActionResult> DeleteDictionaryItemAsync([FromQuery] string[] ids)
        {
            await _dynamicPropertyService.DeleteDictionaryItemsAsync(ids);
            return Ok();
        }
    }
}
