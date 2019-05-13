using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Model;
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
        public const string ChangeLogObjectType = nameof(InventoryInfo);

        private readonly IChangeLogSearchService _changeLogSearchService;
        private readonly IInventoryService _inventoryService;

        public ProductAvailabilityChangesProvider(IChangeLogSearchService changeLogSearchService, IInventoryService inventoryService)
        {
            _changeLogSearchService = changeLogSearchService;
            _inventoryService = inventoryService;
        }

        public async Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            long result;

            var criteria = new ChangeLogSearchCriteria
            {
                ObjectType = ChangeLogObjectType,
                StartDate = startDate,
                EndDate = endDate,
                Take = 0
            };
            // Get changes count from operation log
            result = (await _changeLogSearchService.SearchAsync(criteria)).TotalCount;
            return result;
        }

        public virtual async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            var result = new List<IndexDocumentChange>();

            var criteria = new ChangeLogSearchCriteria
            {
                ObjectType = ChangeLogObjectType,
                StartDate = startDate,
                EndDate = endDate,
                Skip = (int)skip,
                Take = (int)take
            };

            // Get changes from operation log
            var operations = (await _changeLogSearchService.SearchAsync(criteria)).Results;

            var inventories = await _inventoryService.GetByIdsAsync(operations.Select(o => o.ObjectId).ToArray(), InventoryResponseGroup.Default.ToString());

            result = operations.Join(inventories, o => o.ObjectId, i => i.Id, (o, i) => new IndexDocumentChange
            {
                DocumentId = i.ProductId,
                ChangeType = IndexDocumentChangeType.Modified,
                ChangeDate = o.ModifiedDate ?? o.CreatedDate,
            }).ToList();


            return await Task.FromResult(result);
        }
    }
}
