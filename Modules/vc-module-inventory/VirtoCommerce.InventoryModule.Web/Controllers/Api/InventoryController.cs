using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Web.Security;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.InventoryModule.Web.Controllers.Api
{
    [Route("api/inventory")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IInventorySearchService _inventorySearchService;
        private readonly IFulfillmentCenterSearchService _fulfillmentCenterSearchService;
        private readonly IFulfillmentCenterService _fulfillmentCenterService;

        public InventoryController(IInventoryService inventoryService, IFulfillmentCenterSearchService fulfillmentCenterSearchService,
                                          IInventorySearchService inventorySearchService, IFulfillmentCenterService fulfillmentCenterService)
        {
            _inventoryService = inventoryService;
            _fulfillmentCenterSearchService = fulfillmentCenterSearchService;
            _fulfillmentCenterService = fulfillmentCenterService;
            _inventorySearchService = inventorySearchService;
        }

        /// <summary>
        /// Search fulfillment centers registered in the system
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GenericSearchResult<FulfillmentCenter>), 200)]
        [Route("fulfillmentcenters/search")]
        public async Task<IActionResult> SearchFulfillmentCenters([FromBody] FulfillmentCenterSearchCriteria searchCriteria)
        {
            var retVal = await _fulfillmentCenterSearchService.SearchCentersAsync(searchCriteria);
            return Ok(retVal);
        }


        /// <summary>
        /// Find fulfillment center by id
        /// </summary>
        /// <param name="id">fulfillment center id</param>
        [HttpGet]
        [ProducesResponseType(typeof(FulfillmentCenter), 200)]
        [Route("fulfillmentcenters/{id}")]
        public async Task<IActionResult> GetFulfillmentCenter(string id)
        {
            var retVal = await _fulfillmentCenterService.GetByIdsAsync(new[] { id });
            return Ok(retVal.FirstOrDefault());
        }

        /// <summary>
        ///  Save fulfillment center 
        /// </summary>
        /// <param name="center">fulfillment center</param>
        [HttpPut]
        [ProducesResponseType(typeof(FulfillmentCenter), 200)]
        [Route("fulfillmentcenters")]
        [Authorize(InventoryPredefinedPermissions.FulfillmentEdit)]
        public async Task<IActionResult> SaveFulfillmentCenter(FulfillmentCenter center)
        {
            await _fulfillmentCenterService.SaveChangesAsync(new[] { center });
            return Ok(center);
        }

        /// <summary>
        /// Delete  fulfillment centers registered in the system
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(void), 200)]
        [Route("fulfillmentcenters")]
        [Authorize(InventoryPredefinedPermissions.FulfillmentDelete)]
        public async Task<IActionResult> DeleteFulfillmentCenters([FromQuery] string[] ids)
        {
            await _fulfillmentCenterService.DeleteAsync(ids);
            return Ok();
        }


        /// <summary>
        /// Get inventories of products
        /// </summary>
        /// <remarks>Get inventory of products for each fulfillment center.</remarks>
        /// <param name="ids">Products ids</param>
        [HttpGet]
        [Route("products")]
        [ProducesResponseType(typeof(InventoryInfo[]), 200)]
        public async Task<IActionResult> GetProductsInventories([FromQuery] string[] ids)
        {
            var result = new List<InventoryInfo>();
            var allFulfillments = await _fulfillmentCenterSearchService.SearchCentersAsync(new FulfillmentCenterSearchCriteria { Take = int.MaxValue });
            var inventories = await _inventoryService.GetProductsInventoryInfosAsync(ids);
            foreach (var productId in ids)
            {
                foreach (var fulfillment in allFulfillments.Results)
                {
                    var inventory = inventories.FirstOrDefault(x => x.ProductId == productId && x.FulfillmentCenterId == fulfillment.Id);
                    if (inventory == null)
                    {
                        inventory = AbstractTypeFactory<InventoryInfo>.TryCreateInstance();
                        inventory.ProductId = productId;
                        inventory.FulfillmentCenter = fulfillment;
                        inventory.FulfillmentCenterId = fulfillment.Id;
                    }
                    result.Add(inventory);
                }
            }

            return Ok(result.ToArray());
        }

        /// <summary>
        /// Get inventories of products
        /// </summary>
        /// <remarks>Get inventory of products for each fulfillment center.</remarks>
        /// <param name="ids">Products ids</param>
        [HttpPost]
        [Route("products/plenty")]
        [ProducesResponseType(typeof(InventoryInfo[]), 200)]
        public async Task<IActionResult> GetProductsInventoriesByPlentyIds([FromBody] string[] ids)
        {
            return await GetProductsInventories(ids);
        }

        /// <summary>
        /// Get inventories of product
        /// </summary>
        /// <remarks>Get inventories of product for each fulfillment center.</remarks>
        /// <param name="productId">Product id</param>
        [HttpGet]
        [Route("products/{productId}")]
        [ProducesResponseType(typeof(InventoryInfo[]), 200)]
        public async Task<IActionResult> GetProductInventories(string productId)
        {
            return await GetProductsInventories(new[] { productId });
        }

        /// <summary>
        /// Upsert inventory
        /// </summary>
        /// <remarks>Upsert (add or update) given inventory of product.</remarks>
        /// <param name="inventory">Inventory to upsert</param>
        [HttpPut]
        [Route("products/{productId}")]
        [ProducesResponseType(typeof(InventoryInfo), 200)]
        [Authorize(InventoryPredefinedPermissions.Update)]
        public async Task<IActionResult> UpsertProductInventory(InventoryInfo inventory)
        {
            var result = await _inventoryService.UpsertInventoryAsync(inventory);
            return Ok(result);
        }
    }
}
