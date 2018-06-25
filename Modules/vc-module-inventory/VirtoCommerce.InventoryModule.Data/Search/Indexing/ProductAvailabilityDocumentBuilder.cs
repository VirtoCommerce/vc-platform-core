using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.InventoryModule.Data.Search.Indexing
{
    /// <summary>
    /// Extend product indexation process and provides available_in field for indexed products
    /// </summary>
    public class ProductAvailabilityDocumentBuilder : IIndexDocumentBuilder
    {
        private readonly IInventoryService _inventoryService;

        public ProductAvailabilityDocumentBuilder(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var now = DateTime.UtcNow;
            var result = new List<IndexDocument>();
            var inventoriesGroupByProduct = await _inventoryService.GetProductsInventoryInfosAsync(documentIds);
            foreach (var productInventories in inventoriesGroupByProduct.GroupBy(x => x.ProductId))
            {
                var document = new IndexDocument(productInventories.Key);
                foreach (var inventory in productInventories)
                {                   
                    if (inventory.IsAvailableOn(now))
                    {
                        document.Add(new IndexDocumentField("available_in", inventory.FulfillmentCenterId.ToLowerInvariant()) { IsRetrievable = true, IsFilterable = true, IsCollection = true });
                    }
                    result.Add(document);
                }
            }
            return await Task.FromResult(result);
        }
    }
}
