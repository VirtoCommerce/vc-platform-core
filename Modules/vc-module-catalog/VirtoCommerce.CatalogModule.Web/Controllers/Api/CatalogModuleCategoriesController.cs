using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
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
        public async Task<ActionResult<Category>> Get(string id)
        {
            var category = (await _categoryService.GetByIdsAsync(new[] { id }, null)).FirstOrDefault();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        /// <summary>
        /// Gets categories by ids
        /// </summary>
        /// <param name="ids">Categories ids</param>
        ///<param name="respGroup">Response group.</param>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<Category[]>> GetCategoriesByIdsAsync([FromQuery] string[] ids, [FromQuery] string respGroup = null)
        {
            var categories = await _categoryService.GetByIdsAsync(ids, respGroup);

            //TODO
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, categories);

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
        public Task<ActionResult<Category[]>> GetCategoriesByPlentyIds([FromBody] string[] ids, [FromQuery] string respGroup = null)
        {
            return GetCategoriesByIdsAsync(ids, respGroup);
        }

        /// <summary>
        /// Gets the template for a new category.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentCategoryId">The parent category id. (Optional)</param>
        [HttpGet]
        [Route("~/api/catalog/{catalogId}/categories/newcategory")]
        public ActionResult<Category> GetNewCategory(string catalogId, [FromQuery]string parentCategoryId = null)
        {
            var retVal = new Category
            {
                ParentId = parentCategoryId,
                CatalogId = catalogId,
                Code = Guid.NewGuid().ToString().Substring(0, 5),
                SeoInfos = new List<SeoInfo>(),
                IsActive = true
            };

            //TODO
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
        public async Task<IActionResult> CreateOrUpdateCategory([FromBody]Category category)
        {
            if (category.Id == null)
            {
                if (category.SeoInfos == null || !category.SeoInfos.Any())
                {
                    var slugUrl = category.Name.GenerateSlug();
                    if (!string.IsNullOrEmpty(slugUrl))
                    {
                        var catalog = (await _catalogService.GetByIdsAsync(new[] { category.CatalogId })).FirstOrDefault();
                        var defaultLanguage = catalog?.Languages.First(x => x.IsDefault).LanguageCode;
                        category.SeoInfos = new[] { new SeoInfo { LanguageCode = defaultLanguage, SemanticUrl = slugUrl } };
                    }
                }

                //TODO
                //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, coreCategory);

                await _categoryService.SaveChangesAsync(new[] { category });
                return Ok(category);
            }
            else
            {
                //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Update, coreCategory);

                await _categoryService.SaveChangesAsync(new[] { category });
                return NoContent();
            }
        }


        /// <summary>
        /// Deletes the specified categories by id.
        /// </summary>
        /// <param name="ids">The categories ids.</param>
        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> Delete([FromQuery]string[] ids)
        {
            //TODO
            //var categories = _categoryService.GetByIds(ids, Domain.Catalog.Model.CategoryResponseGroup.WithParents);
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, categories);

            await _categoryService.DeleteAsync(ids);
            return NoContent();
        }
    }
}
