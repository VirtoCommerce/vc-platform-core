using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.CatalogModule.Core2.Services;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web2.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/catalog/categories")]
    public class CatalogModuleCategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICatalogService _catalogService;

        public CatalogModuleCategoriesController(ICategoryService categoryService, ICatalogService catalogService)
        {
            _categoryService = categoryService;
            _catalogService = catalogService;
        }

        /// <summary>
        /// Gets category by id.
        /// </summary>
        /// <param name="id">Category id.</param>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Category), 200)]
        public ActionResult Get(string id, [FromQuery] string respGroup = null)
        {
            var category = _categoryService.GetByIds(new[] { id }, respGroup);
            return Ok(category);
        }

        /// <summary>
        /// Gets categories by ids
        /// </summary>
        /// <param name="ids">Categories ids</param>
        ///<param name="respGroup">Response group.</param>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(Category[]), 200)]
        public ActionResult GetCategoriesByIds([FromQuery] string[] ids, [FromQuery] string respGroup = null)
        {
            var categories = _categoryService.GetByIds(ids, respGroup);

            //TODO:
            // CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, categories);           
            //foreach (var category in retVal)
            //{
            //    category.SecurityScopes = GetObjectPermissionScopeStrings(category);
            //}

            return Ok(categories);
        }

        /// <summary>
        /// Get categories by plenty ids
        /// </summary>
        /// <param name="ids">Categories ids</param>
        /// <param name="respGroup">Response group</param>
        /// <returns></returns>
        [HttpPost]
        [Route("plenty")]
        [ProducesResponseType(typeof(Category[]), 200)]
        public ActionResult GetCategoriesByPlentyIds([FromBody] string[] ids, [FromQuery] string respGroup = null)
        {
            return GetCategoriesByIds(ids, respGroup);
        }

        /// <summary>
        /// Gets the template for a new category.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentCategoryId">The parent category id. (Optional)</param>
        [HttpGet]
        [Route("~/api/catalog/{catalogId}/categories/newcategory")]
        [ProducesResponseType(typeof(Category), 200)]
        public ActionResult GetNewCategory(string catalogId, [FromQuery]string parentCategoryId = null)
        {
            var retVal = AbstractTypeFactory<Category>.TryCreateInstance();

            retVal.ParentId = parentCategoryId;
            retVal.CatalogId = catalogId;
            retVal.Code = Guid.NewGuid().ToString().Substring(0, 5);
            retVal.SeoInfos = new List<SeoInfo>();
            retVal.IsActive = true;

            //TODO:
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, retVal.ToModuleModel());
            //retVal.SecurityScopes = GetObjectPermissionScopeStrings(retVal.ToModuleModel());

            return Ok(retVal);
        }


        /// <summary>
        /// Creates or updates the specified category.
        /// </summary>
        /// <remarks>If category.id is null, a new category is created. It's updated otherwise</remarks>
        /// <param name="category">The category.</param>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        public ActionResult SaveCategory([FromBody] Category category)
        {
            if (category.IsTransient())
            {
                if (category.SeoInfos == null || !category.SeoInfos.Any())
                {
                    var slugUrl = category.Name.GenerateSlug();
                    if (!string.IsNullOrEmpty(slugUrl))
                    {
                        var catalog = _catalogService.GetByIds(new[] { category.CatalogId }).FirstOrDefault();
                        var defaultLanguage = catalog.Languages.First(x => x.IsDefault).LanguageCode;
                        category.SeoInfos = new[] { new SeoInfo { LanguageCode = defaultLanguage, SemanticUrl = slugUrl } };
                    }
                }
            }
            //TODO:
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, coreCategory);
            _categoryService.SaveChanges(new[] { category });
            return Ok(category);
        }


        /// <summary>
        /// Deletes the specified categories by id.
        /// </summary>
        /// <param name="ids">The categories ids.</param>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        public ActionResult Delete([FromQuery]string[] ids)
        {
            var categories = _categoryService.GetByIds(ids, CategoryResponseGroup.WithParents.ToString());
            //TODO:
            // CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, categories);

            _categoryService.Delete(ids);
            return Ok();
        }

    }
}
