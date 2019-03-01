using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.CatalogModule.Data.Search;
using VirtoCommerce.CatalogModule.Web.Converters;
using VirtoCommerce.CatalogModule.Web.Security;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using coreModel = VirtoCommerce.Domain.Catalog.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [RoutePrefix("api/catalog/products")]
    public class CatalogModuleProductsController : CatalogBaseController
    {
        private readonly IItemService _itemsService;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly ICatalogService _catalogService;
        private readonly ICategoryService _categoryService;
        private readonly ISkuGenerator _skuGenerator;
        private readonly IProductAssociationSearchService _productAssociationSearchService;

        public CatalogModuleProductsController(IItemService itemsService, IBlobUrlResolver blobUrlResolver, ICatalogService catalogService, ICategoryService categoryService,
                                               ISkuGenerator skuGenerator, ISecurityService securityService, IPermissionScopeService permissionScopeService, IProductAssociationSearchService productAssociationSearchService)
            : base(securityService, permissionScopeService)
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
        [ResponseType(typeof(webModel.Product))]
        public IHttpActionResult GetProductById(string id, [FromUri] coreModel.ItemResponseGroup respGroup = coreModel.ItemResponseGroup.ItemLarge)
        {
            var item = _itemsService.GetById(id, respGroup);
            if (item == null)
            {
                return NotFound();
            }

            CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, item);

            var retVal = item.ToWebModel(_blobUrlResolver);

            retVal.SecurityScopes = GetObjectPermissionScopeStrings(item);
            return Ok(retVal);
        }

        /// <summary>
        /// Gets products by ids
        /// </summary>
        /// <param name="ids">Item ids</param>
        ///<param name="respGroup">Response group.</param>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(webModel.Product[]))]
        public IHttpActionResult GetProductByIds([FromUri] string[] ids, [FromUri] coreModel.ItemResponseGroup respGroup = coreModel.ItemResponseGroup.ItemLarge)
        {
            var items = _itemsService.GetByIds(ids, respGroup);
            if (items == null)
            {
                return NotFound();
            }

            CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, items);

            var retVal = items.Select(x => x.ToWebModel(_blobUrlResolver)).ToArray();
            foreach (var product in retVal)
            {
                product.SecurityScopes = GetObjectPermissionScopeStrings(product);
            }
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
        [ResponseType(typeof(webModel.Product[]))]
        public IHttpActionResult GetProductByPlentyIds([FromBody] string[] ids, [FromUri] coreModel.ItemResponseGroup respGroup = coreModel.ItemResponseGroup.ItemLarge)
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
        [ResponseType(typeof(webModel.Product))]
        public IHttpActionResult GetNewProductByCatalog(string catalogId)
        {
            CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, new coreModel.Catalog { Id = catalogId });

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
        [ResponseType(typeof(webModel.Product))]
        public IHttpActionResult GetNewProductByCatalogAndCategory(string catalogId, string categoryId)
        {
            var retVal = AbstractTypeFactory<webModel.Product>.TryCreateInstance();
            retVal.CategoryId = categoryId;
            retVal.CatalogId = catalogId;
            retVal.IsActive = true;
            retVal.SeoInfos = Array.Empty<SeoInfo>();            

            CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, retVal.ToModuleModel(_blobUrlResolver));

            if (catalogId != null)
            {
                var catalog = _catalogService.GetById(catalogId);
                retVal.Properties = catalog.Properties.Select(x => x.ToWebModel()).ToList();
            }

            if (categoryId != null)
            {
                var category = _categoryService.GetById(categoryId, coreModel.CategoryResponseGroup.WithProperties);
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
        [ResponseType(typeof(webModel.Product))]
        public IHttpActionResult GetNewVariation(string productId)
        {
            var product = _itemsService.GetById(productId, coreModel.ItemResponseGroup.ItemLarge);
            if (product == null)
            {
                return NotFound();
            }

            CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, product);

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
        [ResponseType(typeof(webModel.Product))]
        public IHttpActionResult CloneProduct(string productId)
        {
            var product = _itemsService.GetById(productId, coreModel.ItemResponseGroup.ItemLarge);
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
        [ResponseType(typeof(webModel.Product))]
        public IHttpActionResult SaveProduct(webModel.Product product)
        {
            var result = InnerSaveProducts(new[] { product }).FirstOrDefault();
            if (result != null)
            {
                return Ok(result.ToWebModel(_blobUrlResolver));
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Create/Update the specified products.
        /// </summary>
        /// <param name="products">The products.</param>
        [HttpPost]
        [Route("batch")]
        [ResponseType(typeof(void))]
        public IHttpActionResult SaveProducts(webModel.Product[] products)
        {
            InnerSaveProducts(products);
            return Ok();
        }


        /// <summary>
        /// Deletes the specified items by id.
        /// </summary>
        /// <param name="ids">The items ids.</param>
        [HttpDelete]
        [Route("")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Delete([FromUri] string[] ids)
        {
            var products = _itemsService.GetByIds(ids, coreModel.ItemResponseGroup.ItemInfo);
            CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, products);

            _itemsService.Delete(ids);
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Return product and product's associations products
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("associations/search")]
        [ResponseType(typeof(webModel.ProductAssociationSearchResult))]
        public IHttpActionResult SearchProductAssociations(ProductAssociationSearchCriteria criteria)
        {
            var searchResult = _productAssociationSearchService.SearchProductAssociations(criteria);
            var result = new webModel.ProductAssociationSearchResult
            {
                Results = searchResult.Results.Select(x => x.ToWebModel(_blobUrlResolver)).ToList(),
                TotalCount = searchResult.TotalCount
            };
            return Ok(result);
        }

        private coreModel.CatalogProduct[] InnerSaveProducts(webModel.Product[] products)
        {
            var toUpdateList = new List<coreModel.CatalogProduct>();
            var toCreateList = new List<coreModel.CatalogProduct>();
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
                            var catalog = _catalogService.GetById(product.CatalogId);
                            var defaultLanguageCode = catalog.Languages.First(x => x.IsDefault).LanguageCode;
                            var seoInfo = new SeoInfo
                            {
                                LanguageCode = defaultLanguageCode,
                                SemanticUrl = slugUrl
                            };
                            moduleProduct.SeoInfos = new[] { seoInfo };
                        }
                    }

                    CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, moduleProduct);
                    toCreateList.Add(moduleProduct);
                }
                else
                {
                    CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Update, moduleProduct);
                    toUpdateList.Add(moduleProduct);
                }
            }

            if (!toCreateList.IsNullOrEmpty())
            {
                _itemsService.Create(toCreateList.ToArray());
            }
            if (!toUpdateList.IsNullOrEmpty())
            {
                _itemsService.Update(toUpdateList.ToArray());
            }

            return toCreateList.Concat(toUpdateList).ToArray();
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
