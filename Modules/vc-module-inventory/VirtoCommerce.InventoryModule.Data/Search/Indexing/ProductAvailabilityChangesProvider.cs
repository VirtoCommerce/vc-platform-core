using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.InventoryModule.Data.Search.Indexing
{
    /// <summary>
    /// Extend product indexation process. Invalidate products as changed when products availability in fulfillment centers updated.
    /// </summary>
    public class ProductAvailabilityChangesProvider : IIndexDocumentChangesProvider
    {
        public const string ChangeLogObjectType = nameof(InventoryEntity);

        private readonly IChangeLogService _changeLogService;
        private readonly IInventoryService _inventoryService;

        public ProductAvailabilityChangesProvider(IChangeLogService changeLogService, IInventoryService inventoryService)
        {
            _changeLogService = changeLogService;
            _inventoryService = inventoryService;
        }

        public async Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            long result;

            startDate = startDate ?? DateTime.MinValue;
            endDate = endDate ?? DateTime.MaxValue;

            // Get changes count from operation log
            result = _changeLogService.FindChangeHistory(ChangeLogObjectType, startDate, endDate).Count();


            return await Task.FromResult(result);
        }

        public virtual async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            var result = new List<IndexDocumentChange>();
            startDate = startDate ?? DateTime.MinValue;
            endDate = endDate ?? DateTime.MaxValue;

                // Get changes from operation log
                var changeLogOperations = _changeLogService.FindChangeHistory(ChangeLogObjectType, startDate, endDate)
                    .Skip((int)skip)
                    .Take((int)take)
                    .ToArray();

            var inventories = await _inventoryService.GetByIdsAsync(
                changeLogOperations.Select(o => o.ObjectId).ToArray(), InventoryResponseGroup.Default.ToString());

                result = changeLogOperations.Join(inventories, o => o.ObjectId,  i => i .Id, (o, i) => new IndexDocumentChange
                {
                    DocumentId = i.ProductId,
                    ChangeType = IndexDocumentChangeType.Modified,
                    ChangeDate = o.ModifiedDate ?? o.CreatedDate,
                }).ToList();
            

            return await Task.FromResult(result);
        }
    }
}
