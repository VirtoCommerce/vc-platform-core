using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.InventoryModule.Data.Model;
using VirtoCommerce.InventoryModule.Data.Repositories;
using VirtoCommerce.Platform.Core.ChangeLog;

namespace VirtoCommerce.InventoryModule.Data.Search.Indexing
{
    /// <summary>
    /// Extend product indexation process. Invalidate products as changed when products availability in fulfillment centers updated.
    /// </summary>
    public class ProductAvailabilityChangesProvider : IIndexDocumentChangesProvider
    {
        public const string ChangeLogObjectType = nameof(InventoryEntity);

        private readonly IChangeLogService _changeLogService;
        private readonly Func<IInventoryRepository> _inventoryRepositoryFactory;

        public ProductAvailabilityChangesProvider(IChangeLogService changeLogService, Func<IInventoryRepository> inventoryRepositoryFactory)
        {
            _changeLogService = changeLogService;
            _inventoryRepositoryFactory = inventoryRepositoryFactory;
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

                var inventories = GetInventoryByIds(changeLogOperations.Select(o => o.ObjectId).ToArray());

                result = changeLogOperations.Join(inventories, o => o.ObjectId,  i => i .Id, (o, i) => new IndexDocumentChange
                {
                    DocumentId = i.Sku,
                    ChangeType = IndexDocumentChangeType.Modified,
                    ChangeDate = o.ModifiedDate ?? o.CreatedDate,
                }).ToList();
            

            return await Task.FromResult(result);
        }

        protected virtual IEnumerable<InventoryEntity> GetInventoryByIds(string[] inventoryIds)
        {
            // TODO: How to get product for deleted completeness entry?
            using (var repository = _inventoryRepositoryFactory())
            {
                // TODO: Replace with service after GetById will be implemented
                var inventories = repository.Inventories.Where(i => inventoryIds.Contains(i.Id)).ToArray();
                return inventories;
            }
        }
    }
}
