using System;
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

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/properties")]
    public class CatalogModulePropertiesController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly ICategoryService _categoryService;
        private readonly ICatalogService _catalogService;
        private readonly IPropertyDictionaryItemSearchService _propertyDictionarySearchService;
        private readonly IAuthorizationService _authorizationService;
        //Workaround: Bad design to use repository in the controller layer, need to extend in the future IPropertyService.Delete with new parameter DeleteAllValues
        public CatalogModulePropertiesController(
            IPropertyService propertyService
            , ICategoryService categoryService
            , ICatalogService catalogService
            , IPropertyDictionaryItemSearchService propertyDictionarySearchService
            , IAuthorizationService authorizationService)
        {
            _propertyService = propertyService;
            _categoryService = categoryService;
            _catalogService = catalogService;
            _propertyDictionarySearchService = propertyDictionarySearchService;
            _authorizationService = authorizationService;
        }


        /// <summary>
        /// Gets all dictionary values that specified property can have.
        /// </summary>
        /// <param name="propertyId">The property id.</param>
        /// <param name="keyword">The keyword. (Optional)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{propertyId}/values")]
        [Obsolete("Use POST api/catalog/properties/dictionaryitems/search instead")]
        public async Task<ActionResult<PropertyDictionaryItem[]>> GetPropertyValues(string propertyId, [FromQuery]string keyword = null)
        {
            var dictValues = await _propertyDictionarySearchService.SearchAsync(new PropertyDictionaryItemSearchCriteria { Keyword = keyword, PropertyIds = new[] { propertyId }, Take = int.MaxValue });

            return Ok(dictValues.Results);
        }


        /// <summary>
        /// Gets property metainformation by id.
        /// </summary>
        /// <param name="propertyId">The property id.</param>
		[HttpGet]
        [Route("{propertyId}")]
        public async Task<ActionResult<Property>> GetProperty(string propertyId)
        {
            var property = (await _propertyService.GetByIdsAsync(new[] { propertyId })).FirstOrDefault();
            if (property == null)
            {
                return NotFound();
            }
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, property, new CatalogAuthorizationRequirement(ModuleConstants.Security.Permissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            return Ok(property);
        }


        /// <summary>
        /// Gets the template for a new catalog property.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        [HttpGet]
        [Route("~/api/catalog/{catalogId}/properties/getnew")]
        public async Task<ActionResult<Property>> GetNewCatalogProperty(string catalogId)
        {
            var catalog = (await _catalogService.GetByIdsAsync(new[] { catalogId })).FirstOrDefault();
            var retVal = new Property
            {
                Id = Guid.NewGuid().ToString(),
                IsNew = true,
                CatalogId = catalog?.Id,
                Name = "new property",
                Type = PropertyType.Catalog,
                ValueType = PropertyValueType.ShortText,
                Attributes = new List<PropertyAttribute>(),
                DisplayNames = catalog?.Languages.Select(x => new PropertyDisplayName { LanguageCode = x.LanguageCode }).ToList()
            };

            return Ok(retVal);
        }


        /// <summary>
        /// Gets the template for a new category property.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        [HttpGet]
        [Route("~/api/catalog/categories/{categoryId}/properties/getnew")]
        public async Task<ActionResult<Property>> GetNewCategoryProperty(string categoryId)
        {
            var category = (await _categoryService.GetByIdsAsync(new[] { categoryId }, CategoryResponseGroup.Info.ToString())).FirstOrDefault();
            var retVal = new Property
            {
                Id = Guid.NewGuid().ToString(),
                IsNew = true,
                CategoryId = categoryId,
                CatalogId = category?.CatalogId,
                Name = "new property",
                Type = PropertyType.Category,
                ValueType = PropertyValueType.ShortText,
                Attributes = new List<PropertyAttribute>(),
                DisplayNames = category?.Catalog.Languages.Select(x => new PropertyDisplayName { LanguageCode = x.LanguageCode }).ToList()
            };

            return Ok(retVal);
        }


        /// <summary>
        /// Creates or updates the specified property.
        /// </summary>
        /// <remarks>If property.IsNew == True, a new property is created. It's updated otherwise</remarks>
        /// <param name="property">The property.</param>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> SaveProperty([FromBody]Property property)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, property, new CatalogAuthorizationRequirement(ModuleConstants.Security.Permissions.Update));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            await _propertyService.SaveChangesAsync(new[] { property });

            return NoContent();
        }


        /// <summary>
        /// Deletes property by id.
        /// </summary>
        /// <param name="id">The property id.</param>
        /// <param name="doDeleteValues">Flag indicating to remove property values from objects as well</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        public async Task<ActionResult> DeleteProperty(string id, bool doDeleteValues = false)
        {
            var property = await _propertyService.GetByIdsAsync(new[] { id });

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, property, new CatalogAuthorizationRequirement(ModuleConstants.Security.Permissions.Delete));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }
            await _propertyService.DeleteAsync(new[] { id }, doDeleteValues);
            return NoContent();
        }
    }
}
