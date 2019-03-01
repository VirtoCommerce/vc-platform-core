using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Web.Converters;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using coreModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/categories")]
    public class CatalogModuleCategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICatalogService _catalogService;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public CatalogModuleCategoriesController(ICategoryService categoryService, ICatalogService catalogService, IBlobUrlResolver blobUrlResolver)
        {
            _categoryService = categoryService;
            _catalogService = catalogService;
            _blobUrlResolver = blobUrlResolver;
        }


        /// <summary>
        /// Gets category by id.
        /// </summary>
        /// <param name="id">Category id.</param>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<webModel.Category>> Get(string id)
        {
            var category = (await _categoryService.GetByIdsAsync(new[] { id }, coreModel.CategoryResponseGroup.Full)).FirstOrDefault();

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
        public async Task<ActionResult<webModel.Category[]>> GetCategoriesByIdsAsync([FromQuery] string[] ids, [FromQuery] coreModel.CategoryResponseGroup respGroup = coreModel.CategoryResponseGroup.Full)
        {
            var categories = await _categoryService.GetByIdsAsync(ids, respGroup);

            //TODO
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, categories);

            var retVal = categories.Select(x => x.ToWebModel(_blobUrlResolver)).ToArray();
            //foreach (var category in retVal)
            //{
            //    category.SecurityScopes = GetObjectPermissionScopeStrings(category);
            //}

            return Ok(retVal);
        }

        /// <summary>
        /// Get categories by plenty ids
        /// </summary>
        /// <param name="ids">Categories ids</param>
        /// <param name="respGroup">Response group</param>
        /// <returns></returns>
        [HttpPost]
        [Route("plenty")]
        public Task<ActionResult<webModel.Category[]>> GetCategoriesByPlentyIds([FromBody] string[] ids, [FromQuery] coreModel.CategoryResponseGroup respGroup = coreModel.CategoryResponseGroup.Full)
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
        public ActionResult<webModel.Category> GetNewCategory(string catalogId, [FromQuery]string parentCategoryId = null)
        {
            var retVal = new webModel.Category
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
        public async Task<IActionResult> CreateOrUpdateCategory([FromBody]webModel.Category category)
        {
            var coreCategory = category.ToModuleModel();
            if (coreCategory.Id == null)
            {
                if (coreCategory.SeoInfos == null || !coreCategory.SeoInfos.Any())
                {
                    var slugUrl = category.Name.GenerateSlug();
                    if (!string.IsNullOrEmpty(slugUrl))
                    {
                        var catalog = (await _catalogService.GetByIdsAsync(new[] { category.CatalogId })).FirstOrDefault();
                        var defaultLanguage = catalog.Languages.First(x => x.IsDefault).LanguageCode;
                        coreCategory.SeoInfos = new[] { new SeoInfo { LanguageCode = defaultLanguage, SemanticUrl = slugUrl } };
                    }
                }

                //TODO
                //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, coreCategory);

                await _categoryService.SaveChangesAsync(new[] { coreCategory });
                var retVal = coreCategory.ToWebModel(_blobUrlResolver);
                return Ok(retVal);
            }
            else
            {
                //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Update, coreCategory);

                await _categoryService.SaveChangesAsync(new[] { coreCategory });
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
