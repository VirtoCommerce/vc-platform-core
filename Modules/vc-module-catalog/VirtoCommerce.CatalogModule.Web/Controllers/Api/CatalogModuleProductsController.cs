using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Web.Converters;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using coreModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/products")]
    public class CatalogModuleProductsController : Controller
    {
        private readonly IItemService _itemsService;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly ICatalogService _catalogService;
        private readonly ICategoryService _categoryService;
        private readonly ISkuGenerator _skuGenerator;
        private readonly IProductAssociationSearchService _productAssociationSearchService;

        public CatalogModuleProductsController(IItemService itemsService, IBlobUrlResolver blobUrlResolver, ICatalogService catalogService, ICategoryService categoryService,
                                               ISkuGenerator skuGenerator, IProductAssociationSearchService productAssociationSearchService)
        {
            _itemsService = itemsService;
            _categoryService = categoryService;
            _blobUrlResolver = blobUrlResolver;
            _catalogService = catalogService;
            _skuGenerator = skuGenerator;
            _productAssociationSearchService = productAssociationSearchService;
        }


        /// <summary>
        /// Gets product by id.
        /// </summary>
        /// <param name="id">Item id.</param>
        ///<param name="respGroup">Response group.</param>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<webModel.Product>> GetProductById(string id, [FromQuery] coreModel.ItemResponseGroup respGroup = coreModel.ItemResponseGroup.ItemLarge)
        {
            var item = await _itemsService.GetByIdAsync(id, respGroup);
            if (item == null)
            {
                return NotFound();
            }

            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, item);

            var retVal = item.ToWebModel(_blobUrlResolver);

            //retVal.SecurityScopes = GetObjectPermissionScopeStrings(item);
            return Ok(retVal);
        }

        /// <summary>
        /// Gets products by ids
        /// </summary>
        /// <param name="ids">Item ids</param>
        ///<param name="respGroup">Response group.</param>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<webModel.Product[]>> GetProductByIds([FromQuery] string[] ids, [FromQuery] coreModel.ItemResponseGroup respGroup = coreModel.ItemResponseGroup.ItemLarge)
        {
            var items = await _itemsService.GetByIdsAsync(ids, respGroup);
            if (items == null)
            {
                return NotFound();
            }

            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, items);

            var retVal = items.Select(x => x.ToWebModel(_blobUrlResolver)).ToArray();
            //foreach (var product in retVal)
            //{
            //    product.SecurityScopes = GetObjectPermissionScopeStrings(product);
            //}
            return Ok(retVal);
        }

        /// <summary>
        /// Gets products by plenty ids 
        /// </summary>
        /// <param name="ids">Item ids</param>
        /// <param name="respGroup">Response group.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("plenty")]
        public Task<ActionResult<webModel.Product[]>> GetProductByPlentyIds([FromBody] string[] ids, [FromQuery] coreModel.ItemResponseGroup respGroup = coreModel.ItemResponseGroup.ItemLarge)
        {
            return GetProductByIds(ids, respGroup);
        }


        /// <summary>
        /// Gets the template for a new product (outside of category).
        /// </summary>
        /// <remarks>Use when need to create item belonging to catalog directly.</remarks>
        /// <param name="catalogId">The catalog id.</param>
        [HttpGet]
        [Route("~/api/catalog/{catalogId}/products/getnew")]
        public Task<ActionResult<webModel.Product>> GetNewProductByCatalog(string catalogId)
        {
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, new coreModel.Catalog { Id = catalogId });

            return GetNewProductByCatalogAndCategory(catalogId, null);
        }


        /// <summary>
        /// Gets the template for a new product (inside category).
        /// </summary>
        /// <remarks>Use when need to create item belonging to catalog category.</remarks>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="categoryId">The category id.</param>
        [HttpGet]
        [Route("~/api/catalog/{catalogId}/categories/{categoryId}/products/getnew")]
        public async Task<ActionResult<webModel.Product>> GetNewProductByCatalogAndCategory(string catalogId, string categoryId)
        {
            var retVal = AbstractTypeFactory<webModel.Product>.TryCreateInstance();
            retVal.CategoryId = categoryId;
            retVal.CatalogId = catalogId;
            retVal.IsActive = true;
            retVal.SeoInfos = Array.Empty<SeoInfo>();

            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, retVal.ToModuleModel(_blobUrlResolver));

            if (catalogId != null)
            {
                var catalog = (await _catalogService.GetByIdsAsync(new[] { catalogId })).FirstOrDefault();
                retVal.Properties = catalog.Properties.Select(x => x.ToWebModel()).ToList();
            }

            if (categoryId != null)
            {
                var category = (await _categoryService.GetByIdsAsync(new[] { categoryId }, coreModel.CategoryResponseGroup.WithProperties)).FirstOrDefault();
                retVal.Properties = category.Properties.Select(x => x.ToWebModel()).ToList();
            }


            foreach (var property in retVal.Properties)
            {
                property.Values = new List<webModel.PropertyValue>();
                property.IsManageable = true;
                property.IsReadOnly = property.Type != coreModel.PropertyType.Product && property.Type != coreModel.PropertyType.Variation;
            }


            retVal.Code = _skuGenerator.GenerateSku(retVal.ToModuleModel(null));

            return Ok(retVal);
        }


        /// <summary>
        /// Gets the template for a new variation.
        /// </summary>
        /// <param name="productId">The parent product id.</param>
        [HttpGet]
        [Route("{productId}/getnewvariation")]
        public async Task<ActionResult<webModel.Product>> GetNewVariation(string productId)
        {
            var product = await _itemsService.GetByIdAsync(productId, coreModel.ItemResponseGroup.ItemLarge);
            if (product == null)
            {
                return NotFound();
            }

            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, product);

            var mainWebProduct = product.ToWebModel(_blobUrlResolver);

            var newVariation = AbstractTypeFactory<webModel.Product>.TryCreateInstance();

            newVariation.Name = product.Name;
            newVariation.CategoryId = product.CategoryId;
            newVariation.CatalogId = product.CatalogId;
            newVariation.TitularItemId = product.MainProductId ?? productId;
            newVariation.Properties = mainWebProduct.Properties.Where(x => x.Type == coreModel.PropertyType.Variation).ToList();

            foreach (var property in newVariation.Properties)
            {
                // Mark variation property as required
                if (property.Type == coreModel.PropertyType.Variation)
                {
                    property.Required = true;
                    property.Values.Clear();
                }

                property.IsManageable = true;
            }


            newVariation.Code = _skuGenerator.GenerateSku(newVariation.ToModuleModel(null));
            return Ok(newVariation);
        }


        [HttpGet]
        [Route("{productId}/clone")]
        public async Task<ActionResult<webModel.Product>> CloneProduct(string productId)
        {
            var product = await _itemsService.GetByIdAsync(productId, coreModel.ItemResponseGroup.ItemLarge);
            if (product == null)
            {
                return NotFound();
            }

            // Generate new SKUs and remove SEO records for product and its variations
            product.Code = _skuGenerator.GenerateSku(product);
            product.SeoInfos.Clear();

            foreach (var variation in product.Variations)
            {
                variation.Code = _skuGenerator.GenerateSku(variation);
                variation.SeoInfos.Clear();
            }

            var result = product.ToWebModel(_blobUrlResolver);

            // Clear ID for all related entities except properties
            var allEntities = result.GetFlatObjectsListWithInterface<IEntity>();
            foreach (var entity in allEntities)
            {
                var property = entity as webModel.Property;
                if (property == null)
                {
                    entity.Id = null;
                }
            }

            return Ok(result);
        }

        /// <summary>
        /// Create/Update the specified product.
        /// </summary>
        /// <param name="product">The product.</param>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<webModel.Product>> SaveProduct([FromBody] webModel.Product product)
        {
            var result = (await InnerSaveProducts(new[] { product })).FirstOrDefault();
            if (result != null)
            {
                return Ok(result.ToWebModel(_blobUrlResolver));
            }
            return NoContent();
        }

        /// <summary>
        /// Create/Update the specified products.
        /// </summary>
        /// <param name="products">The products.</param>
        [HttpPost]
        [Route("batch")]
        public async Task<IActionResult> SaveProducts([FromBody] webModel.Product[] products)
        {
            await InnerSaveProducts(products);
            return Ok();
        }


        /// <summary>
        /// Deletes the specified items by id.
        /// </summary>
        /// <param name="ids">The items ids.</param>
        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            //var products = await _itemsService.GetByIdsAsync(ids, coreModel.ItemResponseGroup.ItemInfo);
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, products);

            await _itemsService.DeleteAsync(ids);
            return NoContent();
        }

        /// <summary>
        /// Return product and product's associations products
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("associations/search")]
        public ActionResult<webModel.ProductAssociationSearchResult> SearchProductAssociations([FromBody] ProductAssociationSearchCriteria criteria)
        {
            var searchResult = _productAssociationSearchService.SearchProductAssociations(criteria);
            var result = new webModel.ProductAssociationSearchResult
            {
                Results = searchResult.Results.Select(x => x.ToWebModel(_blobUrlResolver)).ToList(),
                TotalCount = searchResult.TotalCount
            };
            return Ok(result);
        }


        private async Task<coreModel.CatalogProduct[]> InnerSaveProducts(webModel.Product[] products)
        {
            var toSaveList = new List<coreModel.CatalogProduct>();
            var catalogs = await _catalogService.GetByIdsAsync(products.Select(pr => pr.CatalogId).Distinct().ToArray());
            foreach (var product in products)
            {
                var moduleProduct = product.ToModuleModel(_blobUrlResolver);
                if (moduleProduct.IsTransient())
                {
                    if (moduleProduct.SeoInfos == null || !moduleProduct.SeoInfos.Any())
                    {
                        var slugUrl = GenerateProductDefaultSlugUrl(product);
                        if (!string.IsNullOrEmpty(slugUrl))
                        {
                            var catalog = catalogs.FirstOrDefault(c => c.Id.EqualsInvariant(product.CatalogId));
                            var defaultLanguageCode = catalog.Languages.First(x => x.IsDefault).LanguageCode;
                            var seoInfo = new SeoInfo
                            {
                                LanguageCode = defaultLanguageCode,
                                SemanticUrl = slugUrl
                            };
                            moduleProduct.SeoInfos = new[] { seoInfo };
                        }
                    }

                    //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, moduleProduct);
                }

                toSaveList.Add(moduleProduct);
            }

            if (!toSaveList.IsNullOrEmpty())
            {
                await _itemsService.SaveChangesAsync(toSaveList.ToArray());
            }

            return toSaveList.ToArray();
        }

        private string GenerateProductDefaultSlugUrl(webModel.Product product)
        {
            var retVal = new List<string>
            {
                product.Name
            };
            if (product.Properties != null)
            {
                foreach (var property in product.Properties.Where(x => x.Type == coreModel.PropertyType.Variation && x.Values != null))
                {
                    retVal.AddRange(property.Values.Select(x => x.PropertyName + "-" + x.Value));
                }
            }
            return string.Join(" ", retVal).GenerateSlug();
        }
    }
}
