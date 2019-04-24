using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Web.Controllers.Api
{
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public class PricingModuleController : Controller
    {
        private readonly IPricingService _pricingService;
        private readonly IPricingSearchService _pricingSearchService;
        private readonly IItemService _itemService;
        private readonly ICatalogService _catalogService;
        private readonly IPricingExtensionManager _extensionManager;
        private readonly IBlobUrlResolver _blobUrlResolver;


        public PricingModuleController(IPricingService pricingService, IItemService itemService, ICatalogService catalogService, IPricingExtensionManager extensionManager, IPricingSearchService pricingSearchService, IBlobUrlResolver blobUrlResolver)
        {
            _extensionManager = extensionManager;
            _pricingService = pricingService;
            _itemService = itemService;
            _catalogService = catalogService;
            _pricingSearchService = pricingSearchService;
            _blobUrlResolver = blobUrlResolver;
        }

        /// <summary>
        /// Evaluate prices by given context
        /// </summary>
        /// <param name="evalContext">Pricing evaluation context</param>
        /// <returns>Prices array</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Price[]), 200)]
        [Route("api/pricing/evaluate")]
        public async Task<IActionResult> EvaluatePrices([FromBody]PriceEvaluationContext evalContext)
        {
            var retVal = (await _pricingService.EvaluateProductPricesAsync(evalContext)).ToArray();

            return Ok(retVal);
        }


        /// <summary>
        /// Evaluate pricelists by given context
        /// </summary>
        /// <param name="evalContext">Pricing evaluation context</param>
        /// <returns>Pricelist array</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Pricelist[]), 200)]
        [Route("api/pricing/pricelists/evaluate")]
        public async Task<IActionResult> EvaluatePriceLists([FromBody]PriceEvaluationContext evalContext)
        {
            var retVal = (await _pricingService.EvaluatePriceListsAsync(evalContext)).ToArray();
            return Ok(retVal);
        }
        /// <summary>
        /// Get pricelist assignment
        /// </summary>
        /// <param name="id">Pricelist assignment id</param>
        [HttpGet]
        [ProducesResponseType(typeof(PricelistAssignment), 200)]
        [Route("api/pricing/assignments/{id}")]
        public async Task<IActionResult> GetPricelistAssignmentById(string id)
        {
            var assignment = (await _pricingService.GetPricelistAssignmentsByIdAsync(new[] { id })).FirstOrDefault();
            return Ok(assignment);
        }

        /// <summary>
        /// Get a new pricelist assignment
        /// </summary>
        /// <remarks>Get a new pricelist assignment object. Create new pricelist assignment, but does not save one.</remarks>
        [HttpGet]
        [ProducesResponseType(typeof(PricelistAssignment), 200)]
        [Route("api/pricing/assignments/new")]
        public IActionResult GetNewPricelistAssignments()
        {
            var result = AbstractTypeFactory<PricelistAssignment>.TryCreateInstance();
            result.Priority = 1;
            result.DynamicExpression = _extensionManager.PriceConditionTree;
            return Ok(result);
        }

        /// <summary>
        /// Get pricelists
        /// </summary>
        /// <remarks>Get all pricelists for all catalogs.</remarks>
        [HttpGet]
        [ProducesResponseType(typeof(GenericSearchResult<Pricelist>), 200)]
        [Route("api/pricing/pricelists")]
        public async Task<IActionResult> SearchPricelists(PricelistSearchCriteria criteria)
        {
            if (criteria == null)
            {
                criteria = new PricelistSearchCriteria();
            }
            var result = await _pricingSearchService.SearchPricelistsAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Search pricelist assignments
        /// </summary>
        /// <remarks>Search price list assignments by given criteria</remarks>
        [HttpGet]
        [ProducesResponseType(typeof(GenericSearchResult<PricelistAssignment>), 200)]
        [Route("api/pricing/assignments")]
        public async Task<IActionResult> SearchPricelistAssignments(PricelistAssignmentsSearchCriteria criteria)
        {
            if (criteria == null)
            {
                criteria = new PricelistAssignmentsSearchCriteria();
            }
            var result = await _pricingSearchService.SearchPricelistAssignmentsAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Search product prices 
        /// </summary>
        /// <remarks>Search product prices</remarks>
        [HttpGet]
        [ProducesResponseType(typeof(GenericSearchResult<ProductPrice>), 200)]
        [Route("api/catalog/products/prices/search")]
        public async Task<IActionResult> SearchProductPrices(PricesSearchCriteria criteria)
        {
            if (criteria == null)
            {
                criteria = new PricesSearchCriteria();
            }
            var result = await _pricingSearchService.SearchPricesAsync(criteria);
            var retVal = new GenericSearchResult<ProductPrice>
            {
                TotalCount = result.TotalCount,
                Results = new List<ProductPrice>()
            };

            var products = await _itemService.GetByIdsAsync(result.Results.Select(x => x.ProductId).Distinct().ToArray(), ItemResponseGroup.ItemInfo.ToString());
            foreach (var productPricesGroup in result.Results.GroupBy(x => x.ProductId))
            {
                var productPrice = new ProductPrice
                {
                    ProductId = productPricesGroup.Key,
                    Prices = productPricesGroup.ToList()
                };
                var product = products.FirstOrDefault(x => x.Id == productPricesGroup.Key);
                if (product != null)
                {
                    if (!product.Images.IsNullOrEmpty())
                    {
                        foreach (var image in product.Images)
                        {
                            image.RelativeUrl = image.Url;
                            image.Url = _blobUrlResolver.GetAbsoluteUrl(image.Url);
                        }
                    }

                    productPrice.Product = product;
                }
                retVal.Results.Add(productPrice);
            }

            return Ok(retVal);
        }

        /// <summary>
        /// Evaluate  product prices
        /// </summary>
        /// <remarks>Get an array of valid product prices for each currency.</remarks>
        /// <param name="productId">Product id</param>
        [HttpGet]
        [ProducesResponseType(typeof(Price[]), 200)]
        [Route("api/products/{productId}/prices")]
        public async Task<IActionResult> EvaluateProductPrices(string productId)
        {
            var priceEvalContext = AbstractTypeFactory<PriceEvaluationContext>.TryCreateInstance();
            priceEvalContext.ProductIds = new[] { productId };

            var product = (await _itemService.GetByIdsAsync(new[] { productId }, ItemResponseGroup.ItemInfo.ToString())).FirstOrDefault();
            if (product != null)
            {
                priceEvalContext.CatalogId = product.CatalogId;
            }
            return await EvaluatePrices(priceEvalContext);
        }

        /// <summary>
        /// Evaluate product prices for demand catalog
        /// </summary>
        /// <remarks>Get an array of valid product prices for each currency.</remarks>
        /// <param name="productId">Product id</param>
        /// <param name="catalogId">Catalog id</param>
        [HttpGet]
        [ProducesResponseType(typeof(Price[]), 200)]
        [Route("api/products/{productId}/{catalogId}/pricesWidget")]
        public async Task<IActionResult> EvaluateProductPricesForCatalog(string productId, string catalogId)
        {
            var priceEvalContext = AbstractTypeFactory<PriceEvaluationContext>.TryCreateInstance();
            priceEvalContext.ProductIds = new[] { productId };
            priceEvalContext.CatalogId = catalogId;

            return await EvaluatePrices(priceEvalContext);
        }

        /// <summary>
        /// Create pricelist assignment
        /// </summary>
        /// <param name="assignment">PricelistAssignment</param>
        [HttpPost]
        [ProducesResponseType(typeof(PricelistAssignment), 200)]
        [Route("api/pricing/assignments")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<IActionResult> CreatePricelistAssignment([FromBody]PricelistAssignment assignment)
        {
            await _pricingService.SavePricelistAssignmentsAsync(new[] { assignment });
            return Ok(assignment);
        }

        /// <summary>
        /// Update pricelist assignment
        /// </summary>
        /// <param name="assignment">PricelistAssignment</param>
        /// <todo>Return no any reason if can't update</todo>
        [HttpPut]
        [ProducesResponseType(typeof(void), 204)]
        [Route("api/pricing/assignments")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdatePriceListAssignment([FromBody]PricelistAssignment assignment)
        {
            await _pricingService.SavePricelistAssignmentsAsync(new[] { assignment });
            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), 204)]
        [Route("api/products/prices")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdateProductsPrices([FromBody]ProductPrice[] productPrices)
        {
            var result = await _pricingSearchService.SearchPricesAsync(new PricesSearchCriteria
            {
                Take = int.MaxValue,
                ProductIds = productPrices.Select(x => x.ProductId).ToArray()
            });
            var targetPricesGroups = result.Results.GroupBy(x => x.PricelistId);
            var sourcePricesGroups = productPrices.SelectMany(x => x.Prices).GroupBy(x => x.PricelistId);

            var changedPrices = new List<Price>();
            var deletedPrices = new List<Price>();

            foreach (var sourcePricesGroup in sourcePricesGroups)
            {
                var targetPricesGroup = targetPricesGroups.FirstOrDefault(x => x.Key == sourcePricesGroup.Key);
                if (targetPricesGroup != null)
                {
                    sourcePricesGroup.ToArray().CompareTo(targetPricesGroup.ToArray(), EqualityComparer<Price>.Default, (state, x, y) =>
                    {
                        switch (state)
                        {
                            case EntryState.Modified:
                            case EntryState.Added:
                                changedPrices.Add(x);
                                break;
                            case EntryState.Deleted:
                                deletedPrices.Add(x);
                                break;
                        }
                    });
                }
                else
                {
                    changedPrices.AddRange(sourcePricesGroup);
                }
            }
            await _pricingService.SavePricesAsync(changedPrices.ToArray());
            if (!deletedPrices.IsNullOrEmpty())
            {
                await _pricingService.DeletePricesAsync(deletedPrices.Select(x => x.Id).ToArray());
            }
            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), 204)]
        [Route("api/products/{productId}/prices")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdateProductPrices([FromBody]ProductPrice productPrice)
        {
            return await UpdateProductsPrices(new[] { productPrice });
        }

        /// <summary>
        /// Get all price lists for product
        /// </summary>
        /// <remarks>Get all price lists for given product.</remarks>
        /// <param name="productId">Product id</param>
        [HttpGet]
        [ProducesResponseType(typeof(Pricelist[]), 200)]
        [Route("api/catalog/products/{productId}/pricelists")]
        public async Task<IActionResult> GetProductPriceLists(string productId)
        {
            var productPrices = (await _pricingSearchService.SearchPricesAsync(new PricesSearchCriteria { Take = int.MaxValue, ProductId = productId })).Results;
            var priceLists = (await _pricingSearchService.SearchPricelistsAsync(new PricelistSearchCriteria { Take = int.MaxValue })).Results;
            foreach (var pricelist in priceLists)
            {
                pricelist.Prices = productPrices.Where(x => x.PricelistId == pricelist.Id).ToList();
            }
            return Ok(priceLists);
        }

        /// <summary>
        /// Get pricelist
        /// </summary>
        /// <param name="id">Pricelist id</param>
        [HttpGet]
        [ProducesResponseType(typeof(Pricelist), 200)]
        [Route("api/pricing/pricelists/{id}")]
        public async Task<IActionResult> GetPriceListById(string id)
        {
            var pricelist = (await _pricingService.GetPricelistsByIdAsync(new[] { id })).FirstOrDefault();
            return Ok(pricelist);
        }


        /// <summary>
        /// Create pricelist
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Pricelist), 200)]
        [Route("api/pricing/pricelists")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<IActionResult> CreatePriceList([FromBody]Pricelist priceList)
        {
            await _pricingService.SavePricelistsAsync(new[] { priceList });
            return Ok(priceList);
        }

        /// <summary>
        /// Update pricelist
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(void), 204)]
        [Route("api/pricing/pricelists")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdatePriceList([FromBody]Pricelist priceList)
        {
            await _pricingService.SavePricelistsAsync(new[] { priceList });
            return NoContent();
        }

        /// <summary>
        /// Delete pricelist assignments
        /// </summary>
        /// <remarks>Delete pricelist assignment by given array of ids.</remarks>
        /// <param name="ids">An array of pricelist assignment ids</param>
        /// <todo>Return no any reason if can't update</todo>
        [HttpDelete]
        [ProducesResponseType(typeof(void), 204)]
        [Route("api/pricing/assignments")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<IActionResult> DeleteAssignments(string[] ids)
        {
            await _pricingService.DeletePricelistsAssignmentsAsync(ids);
            return NoContent();
        }

        /// <summary>
        /// Delete all prices for specified product in specified price list
        /// </summary>
        /// <param name="pricelistId"></param>
        /// <param name="productIds"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(void), 204)]
        [Route("api/pricing/pricelists/{pricelistId}/products/prices")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> DeleteProductPrices(string pricelistId, string[] productIds)
        {
            var result = await _pricingSearchService.SearchPricesAsync(new PricesSearchCriteria { PriceListId = pricelistId, ProductIds = productIds, Take = int.MaxValue });
            await _pricingService.DeletePricesAsync(result.Results.Select(x => x.Id).ToArray());
            return NoContent();
        }

        /// <summary>
        /// Delete price by ids
        /// </summary>
        /// <param name="priceIds"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(void), 204)]
        [Route("api/pricing/products/prices")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<IActionResult> DeleteProductPrice(string[] priceIds)
        {
            await _pricingService.DeletePricesAsync(priceIds);
            return NoContent();
        }

        /// <summary>
        /// Delete pricelists  
        /// </summary>
        /// <remarks>Delete pricelists by given array of pricelist ids.</remarks>
        /// <param name="ids">An array of pricelist ids</param>
        [HttpDelete]
        [ProducesResponseType(typeof(void), 204)]
        [Route("api/pricing/pricelists")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<IActionResult> DeletePricelists(string[] ids)
        {
            await _pricingService.DeletePricelistsAsync(ids);
            return NoContent();
        }
    }
}
