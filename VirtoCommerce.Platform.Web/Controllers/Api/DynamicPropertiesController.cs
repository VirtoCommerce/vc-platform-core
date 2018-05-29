using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/platform/dynamic")]
    //[Authorize(SecurityConstants.Permissions.DynamicPropertiesQuery)]
    public class DynamicPropertiesController : Controller
    {
        private readonly IDynamicPropertyService _service;

        public DynamicPropertiesController(IDynamicPropertyService service)
        {
            _service = service;
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
            var types = _service.GetAvailableObjectTypeNames();
            return Ok(types);
        }

        /// <summary>
        /// Get dynamic properties registered for object type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("types/{typeName}/properties")]
        [ProducesResponseType(typeof(DynamicProperty[]), 200)]
        public IActionResult GetProperties(string typeName)
        {
            var properties = _service.GetProperties(typeName);
            return Ok(properties);
        }

        /// <summary>
        /// Add new dynamic property
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("types/{typeName}/properties")]
        [ProducesResponseType(typeof(DynamicProperty), 200)]
        //[Authorize(SecurityConstants.Permissions.DynamicPropertiesCreate)]
        public IActionResult CreateProperty(string typeName, [FromBody]DynamicProperty property)
        {
            property.Id = null;

            if (string.IsNullOrEmpty(property.ObjectType))
                property.ObjectType = typeName;

            var result = _service.SaveProperties(new[] { property });
            return Ok(result.FirstOrDefault());
        }

        /// <summary>
        /// Update existing dynamic property
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("types/{typeName}/properties/{propertyId}")]
        [ProducesResponseType(typeof(void), 200)]
        //[Authorize(SecurityConstants.Permissions.DynamicPropertiesUpdate)]
        public IActionResult UpdateProperty(string typeName, string propertyId, [FromBody]DynamicProperty property)
        {
            property.Id = propertyId;

            if (string.IsNullOrEmpty(property.ObjectType))
                property.ObjectType = typeName;

            _service.SaveProperties(new[] { property });
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Delete dynamic property
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("types/{typeName}/properties/{propertyId}")]
        [ProducesResponseType(typeof(void), 200)]
        //[Authorize(SecurityConstants.Permissions.DynamicPropertiesDelete)]
        public IActionResult DeleteProperty(string typeName, string propertyId)
        {
            _service.DeleteProperties(new[] { propertyId });
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Get dictionary items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("types/{typeName}/properties/{propertyId}/dictionaryitems")]
        [ProducesResponseType(typeof(DynamicPropertyDictionaryItem[]), 200)]
        public IActionResult GetDictionaryItems(string typeName, string propertyId)
        {
            var items = _service.GetDictionaryItems(propertyId);
            return Ok(items);
        }

        /// <summary>
        /// Add or update dictionary items
        /// </summary>
        /// <remarks>
        /// Fill item ID to update existing item or leave it empty to create a new item.
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route("types/{typeName}/properties/{propertyId}/dictionaryitems")]
        [ProducesResponseType(typeof(void), 200)]
        //[Authorize(SecurityConstants.Permissions.DynamicPropertiesUpdate)]
        public IActionResult SaveDictionaryItems(string typeName, string propertyId, [FromBody]DynamicPropertyDictionaryItem[] items)
        {
            _service.SaveDictionaryItems(propertyId, items);
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Delete dictionary items
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="propertyId"></param>
        /// <param name="ids">IDs of dictionary items to delete.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("types/{typeName}/properties/{propertyId}/dictionaryitems")]
        [ProducesResponseType(typeof(void), 200)]
        //[Authorize(SecurityConstants.Permissions.DynamicPropertiesUpdate)]
        public IActionResult DeleteDictionaryItem(string typeName, string propertyId, [FromBody] string[] ids)
        {
            _service.DeleteDictionaryItems(ids);
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}
