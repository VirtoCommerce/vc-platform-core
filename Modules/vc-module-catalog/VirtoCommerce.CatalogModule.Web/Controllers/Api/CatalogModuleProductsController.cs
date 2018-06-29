using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/catalog/products")]
    public class CatalogModuleProductsController : Controller
    {
        private readonly IItemService _itemsService;
        private readonly IProductAssociationSearchService _productAssociationSearchService;

        public CatalogModuleProductsController(IItemService itemsService, IProductAssociationSearchService productAssociationSearchService)
        {
            _itemsService = itemsService;
            _productAssociationSearchService = productAssociationSearchService;
        }


        /// <summary>
        /// Gets product by id.
        /// </summary>
        /// <param name="id">Item id.</param>
        ///<param name="respGroup">Response group.</param>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CatalogProduct),200)]
        public ActionResult GetProductById(string id, [FromQuery] string responseGroup = null)
        {
            var result = _itemsService.GetByIds(new[] { id }, responseGroup);          
            //TODO:
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, item);
            //retVal.SecurityScopes = GetObjectPermissionScopeStrings(item);
            return Ok(result);
        }

        /// <summary>
        /// Gets products by ids
        /// </summary>
        /// <param name="ids">Item ids</param>
        ///<param name="respGroup">Response group.</param>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(CatalogProduct[]), 200)]
        public ActionResult GetProductByIds([FromQuery] string[] ids, [FromQuery] string responseGroup = null)
        {
            var result = _itemsService.GetByIds(ids, responseGroup);

            //TODO
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Read, items);

            //var retVal = items.Select(x => x.ToWebModel(_blobUrlResolver)).ToArray();
            //foreach (var product in retVal)
            //{
            //    product.SecurityScopes = GetObjectPermissionScopeStrings(product);
            //}
            return Ok(result);
        }

        /// <summary>
        /// Gets products by plenty ids 
        /// </summary>
        /// <param name="ids">Item ids</param>
        /// <param name="respGroup">Response group.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("plenty")]
        [ProducesResponseType(typeof(CatalogProduct[]), 200)]
        public ActionResult GetProductByPlentyIds([FromBody] string[] ids, [FromQuery] string responseGroup = null)
        {
            return GetProductByIds(ids, responseGroup);
        }


        /// <summary>
        /// Gets the template for a new product (outside of category).
        /// </summary>
        /// <remarks>Use when need to create item belonging to catalog directly.</remarks>
        /// <param name="catalogId">The catalog id.</param>
        [HttpGet]
        [Route("~/api/catalog/{catalogId}/products/getnew")]
        [ProducesResponseType(typeof(CatalogProduct), 200)]
        public ActionResult GetNewProductByCatalog(string catalogId)
        {
            //TODO:
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
        [ProducesResponseType(typeof(CatalogProduct), 200)]
        public ActionResult GetNewProductByCatalogAndCategory(string catalogId, string categoryId)
        {
            var result = new CatalogProduct
            {
                CategoryId = categoryId,
                CatalogId = catalogId,
                IsActive = true,
                SeoInfos = new SeoInfo [] { }
            };

            //TODO:
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, retVal.ToModuleModel(_blobUrlResolver));
            //TODO:
            //Propery.IsManageable and IsReadonly (tests)

            _itemsService.LoadDependencies(new[] { result });
            return Ok(result);
        }


        /// <summary>
        /// Gets the template for a new variation.
        /// </summary>
        /// <param name="productId">The parent product id.</param>
        [HttpGet]
        [Route("{productId}/getnewvariation")]
        [ProducesResponseType(typeof(Variation), 200)]
        public ActionResult GetNewVariation(string productId)
        {
            var product = _itemsService.GetByIds(new[] { productId }).FirstOrDefault();

            //TODO
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, product);

            var result = AbstractTypeFactory<Variation>.TryCreateInstance();

            result.Name = product.Name;
            result.CategoryId = product.CategoryId;
            result.CatalogId = product.CatalogId;
            result.MainProductId = product.MainProductId ?? productId;
            _itemsService.LoadDependencies(new[] { result });
            return Ok(result);
        }


        [HttpGet]
        [Route("{productId}/clone")]
        [ProducesResponseType(typeof(CatalogProduct), 200)]
        public ActionResult CloneProduct(string productId)
        {
            var product = _itemsService.GetByIds(new[] { productId }).FirstOrDefault();
            CatalogProduct result = null;
            if (product != null)
            {
                result = product.GetCopy();
                _itemsService.LoadDependencies(new[] { result });
            }
            return Ok(result);
        }

        /// <summary>
        /// Create/Update the specified product.
        /// </summary>
        /// <param name="product">The product.</param>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(CatalogProduct), 200)]
        public ActionResult SaveProduct(CatalogProduct product)
        {
            _itemsService.SaveChanges(new[] { product });
            return Ok(product);
        }

        /// <summary>
        /// Create/Update the specified products.
        /// </summary>
        /// <param name="products">The products.</param>
        [HttpPost]
        [Route("batch")]
        [ProducesResponseType(200)]
        public ActionResult SaveProducts(CatalogProduct[] products)
        {
            _itemsService.SaveChanges(products);
            return Ok();
        }


        /// <summary>
        /// Deletes the specified items by id.
        /// </summary>
        /// <param name="ids">The items ids.</param>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(200)]
        public ActionResult Delete([FromQuery] string[] ids)
        {
            var products = _itemsService.GetByIds(ids, ItemResponseGroup.ItemInfo.ToString());
            //TODO:
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, products);

            _itemsService.Delete(ids);
            return Ok();
        }

        /// <summary>
        /// Return product and product's associations products
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("associations/search")]
        [ProducesResponseType(typeof(GenericSearchResult<ProductAssociation>), 200)]
        public ActionResult SearchProductAssociations(ProductAssociationSearchCriteria criteria)
        {
            var result = _productAssociationSearchService.SearchProductAssociations(criteria);          
            return Ok(result);
        }

       
    }
}
