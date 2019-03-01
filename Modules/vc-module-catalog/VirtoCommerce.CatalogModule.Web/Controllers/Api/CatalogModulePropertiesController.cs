using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Web.Converters;
using VirtoCommerce.CatalogModule.Web.Security;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Platform.Core.Security;
using moduleModel = VirtoCommerce.Domain.Catalog.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [RoutePrefix("api/catalog/properties")]
    public class CatalogModulePropertiesController : CatalogBaseController
    {
        private readonly IPropertyService _propertyService;
        private readonly ICategoryService _categoryService;
        private readonly ICatalogService _catalogService;
        private readonly IProperyDictionaryItemSearchService _propertyDictionarySearchService;
        //Workaround: Bad design to use repository in the controller layer, need to extend in the future IPropertyService.Delete with new parameter DeleteAllValues
        private readonly Func<ICatalogRepository> _repositoryFactory;
        public CatalogModulePropertiesController(IPropertyService propertyService, ICategoryService categoryService, ICatalogService catalogService,
                                                 ISecurityService securityService, IPermissionScopeService permissionScopeService, Func<ICatalogRepository> repositoryFactory,
                                                 IProperyDictionaryItemSearchService propertyDictionarySearchService)
            : base(securityService, permissionScopeService)
        {
            _propertyService = propertyService;
            _categoryService = categoryService;
            _catalogService = catalogService;
            _repositoryFactory = repositoryFactory;
            _propertyDictionarySearchService = propertyDictionarySearchService;
        }


        /// <summary>
        /// Gets all dictionary values that specified property can have.
        /// </summary>
        /// <param name="propertyId">The property id.</param>
        /// <param name="keyword">The keyword. (Optional)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{propertyId}/values")]
        [ResponseType(typeof(webModel.PropertyDictionaryValue[]))]
        [Obsolete("Use POST api/catalog/properties/dictionaryitems/search instead")]
        public IHttpActionResult GetPropertyValues(string propertyId, [FromUri]string keyword = null)
        {
            var dictValues = _propertyDictionarySearchService.Search(new moduleModel.Search.PropertyDictionaryItemSearchCriteria { SearchPhrase = keyword, PropertyIds = new[] { propertyId }, Take = int.MaxValue }).Results;

            return Ok(dictValues.Select(x => new webModel.PropertyDictionaryValue { Id = x.Id, Alias = x.Alias, ValueId = x.PropertyId, Value = x.Alias }).ToArray());
        }


        /// <summary>
        /// Gets property metainformation by id.
        /// </summary>
        /// <param name="propertyId">The property id.</param>
		[HttpGet]
        [Route("{propertyId}")]
        [ResponseType(typeof(webModel.Property))]
        public IHttpActionResult Get(string propertyId)
        {
            var property = _propertyService.GetById(propertyId);
            if (property == null)
            {
                return NotFound();
            }
            CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, property);

            var retVal = property.ToWebModel();
            retVal.IsManageable = true;
            return Ok(retVal);
        }


        /// <summary>
        /// Gets the template for a new catalog property.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        [HttpGet]
        [Route("~/api/catalog/{catalogId}/properties/getnew")]
        [ResponseType(typeof(webModel.Property))]
        public IHttpActionResult GetNewCatalogProperty(string catalogId)
        {
            var catalog = _catalogService.GetById(catalogId);
            var retVal = new webModel.Property
            {
                Id = Guid.NewGuid().ToString(),
                IsNew = true,
                CatalogId = catalog.Id,
                Name = "new property",
                Type = moduleModel.PropertyType.Catalog,
                ValueType = moduleModel.PropertyValueType.ShortText,
                Attributes = new List<webModel.PropertyAttribute>(),
                DisplayNames = catalog.Languages.Select(x => new moduleModel.PropertyDisplayName { LanguageCode = x.LanguageCode }).ToList()
            };

            CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, retVal.ToCoreModel());

            return Ok(retVal);
        }


        /// <summary>
        /// Gets the template for a new category property.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        [HttpGet]
        [Route("~/api/catalog/categories/{categoryId}/properties/getnew")]
        [ResponseType(typeof(webModel.Property))]
        public IHttpActionResult GetNewCategoryProperty(string categoryId)
        {
            var category = _categoryService.GetById(categoryId, Domain.Catalog.Model.CategoryResponseGroup.Info);
            var retVal = new webModel.Property
            {
                Id = Guid.NewGuid().ToString(),
                IsNew = true,
                CategoryId = categoryId,
                CatalogId = category.CatalogId,
                Name = "new property",
                Type = moduleModel.PropertyType.Category,
                ValueType = moduleModel.PropertyValueType.ShortText,
                Attributes = new List<webModel.PropertyAttribute>(),
                DisplayNames = category.Catalog.Languages.Select(x => new moduleModel.PropertyDisplayName { LanguageCode = x.LanguageCode }).ToList()
            };

            CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, retVal.ToCoreModel());

            return Ok(retVal);
        }


        /// <summary>
        /// Creates or updates the specified property.
        /// </summary>
        /// <remarks>If property.IsNew == True, a new property is created. It's updated otherwise</remarks>
        /// <param name="property">The property.</param>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(void))]
        public IHttpActionResult CreateOrUpdateProperty(webModel.Property property)
        {
            var moduleProperty = property.ToCoreModel();

            if (property.IsNew)
            {
                CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, moduleProperty);

                _propertyService.Create(moduleProperty);
            }
            else
            {
                CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Update, moduleProperty);

                _propertyService.Update(new[] { moduleProperty });
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Deletes property by id.
        /// </summary>
        /// <param name="id">The property id.</param>
        /// <param name="doDeleteValues">Flag indicating to remove property values from objects as well</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Delete(string id, bool doDeleteValues = false)
        {
            var property = _propertyService.GetById(id);

            CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, property);

            if (doDeleteValues)
            {
                //TODO: Move this logic in the IPropertyService
                using (var repository = _repositoryFactory())
                {
                    repository.RemoveAllPropertyValues(id);
                    repository.UnitOfWork.Commit();
                }
            }

            _propertyService.Delete(new[] { id });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
