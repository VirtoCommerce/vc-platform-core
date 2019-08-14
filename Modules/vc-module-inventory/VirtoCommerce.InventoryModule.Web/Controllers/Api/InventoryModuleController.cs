using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.InventoryModule.Core;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.InventoryModule.Web.Controllers.Api
{
    [Route("api")]
    public class InventoryModuleController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IFulfillmentCenterSearchService _fulfillmentCenterSearchService;
        private readonly IFulfillmentCenterService _fulfillmentCenterService;

        public InventoryModuleController(IInventoryService inventoryService, IFulfillmentCenterSearchService fulfillmentCenterSearchService,
                                          IFulfillmentCenterService fulfillmentCenterService)
        {
            _inventoryService = inventoryService;
            _fulfillmentCenterSearchService = fulfillmentCenterSearchService;
            _fulfillmentCenterService = fulfillmentCenterService;
        }

        /// <summary>
        /// Search fulfillment centers registered in the system
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("inventory/fulfillmentcenters/search")]
        public async Task<ActionResult<FulfillmentCenterSearchResult>> SearchFulfillmentCenters([FromBody] FulfillmentCenterSearchCriteria searchCriteria)
        {
            var retVal = await _fulfillmentCenterSearchService.SearchCentersAsync(searchCriteria);
            return Ok(retVal);
        }


        /// <summary>
        /// Find fulfillment center by id
        /// </summary>
        /// <param name="id">fulfillment center id</param>
        [HttpGet]
        [Route("inventory/fulfillmentcenters/{id}")]
        public async Task<ActionResult<FulfillmentCenter>> GetFulfillmentCenter([FromRoute]string id)
        {
            var retVal = await _fulfillmentCenterService.GetByIdsAsync(new[] { id });
            return Ok(retVal.FirstOrDefault());
        }

        /// <summary>
        ///  Save fulfillment center 
        /// </summary>
        /// <param name="center">fulfillment center</param>
        [HttpPut]
        [Route("inventory/fulfillmentcenters")]
        [Authorize(ModuleConstants.Security.Permissions.FulfillmentEdit)]
        public async Task<ActionResult<FulfillmentCenter>> SaveFulfillmentCenter([FromBody]FulfillmentCenter center)
        {
            await _fulfillmentCenterService.SaveChangesAsync(new[] { center });
            return Ok(center);
        }

        /// <summary>
        /// Delete  fulfillment centers registered in the system
        /// </summary>
        [HttpDelete]
        [Route("inventory/fulfillmentcenters")]
        [Route("fulfillment/centers")]
        [Authorize(ModuleConstants.Security.Permissions.FulfillmentDelete)]
        public async Task<ActionResult> DeleteFulfillmentCenters([FromQuery] string[] ids)
        {
            await _fulfillmentCenterService.DeleteAsync(ids);
            return NoContent();
        }

        /// <summary>
        /// Get inventories of products
        /// </summary>
        /// <remarks>Get inventory of products for each fulfillment center.</remarks>
        /// <param name="ids">Products ids</param>
        [HttpGet]
        [Route("inventory/products")]
        public async Task<ActionResult<InventoryInfo[]>> GetProductsInventories([FromQuery] string[] ids)
        {
            var result = new List<InventoryInfo>();
            var allFulfillments = await _fulfillmentCenterSearchService.SearchCentersAsync(new FulfillmentCenterSearchCriteria { Take = int.MaxValue });
            var inventories = await _inventoryService.GetProductsInventoryInfosAsync(ids, InventoryResponseGroup.WithFulfillmentCenter.ToString());
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
        [Route("inventory/products/plenty")]
        public async Task<ActionResult<InventoryInfo[]>> GetProductsInventoriesByPlentyIds([FromQuery] string[] ids)
        {
            return await GetProductsInventories(ids);
        }

        /// <summary>
        /// Get inventories of product
        /// </summary>
        /// <remarks>Get inventories of product for each fulfillment center.</remarks>
        /// <param name="productId">Product id</param>
        [HttpGet]
        [Route("inventory/products/{productId}")]
        public async Task<ActionResult<InventoryInfo[]>> GetProductInventories([FromRoute]string productId)
        {
            return await GetProductsInventories(new[] { productId });
        }

        /// <summary>
        /// Update inventory
        /// </summary>
        /// <remarks>Update given inventory of product.</remarks>
        /// <param name="inventory">Inventory to update</param>
        [HttpPut]
        [Route("inventory/products/{productId}")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<InventoryInfo>> UpdateProductInventory([FromBody]InventoryInfo inventory)
        {
            await _inventoryService.SaveChangesAsync(new[] { inventory });
            return Ok(inventory);
        }
    }
}
