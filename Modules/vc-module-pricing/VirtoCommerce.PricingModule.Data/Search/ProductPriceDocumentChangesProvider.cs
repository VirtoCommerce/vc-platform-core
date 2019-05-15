using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.Search
{
    public class ProductPriceDocumentChangesProvider : IIndexDocumentChangesProvider
    {
        public const string ChangeLogObjectType = nameof(Price);

        private readonly IChangeLogSearchService _changeLogSearchService;
        private readonly IPricingService _pricingService;

        public ProductPriceDocumentChangesProvider(IChangeLogSearchService changeLogSearchService, IPricingService pricingService)
        {
            _changeLogSearchService = changeLogSearchService;
            _pricingService = pricingService;
        }

        public virtual async Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            long result;

            if (startDate == null && endDate == null)
            {
                // We don't know the total products count
                result = 0L;
            }
            else
            {
                var criteria = new ChangeLogSearchCriteria
                {
                    ObjectType = ChangeLogObjectType,
                    StartDate = startDate,
                    EndDate = endDate,
                    Take = 0
                };
                // Get changes count from operation log
                result = (await _changeLogSearchService.SearchAsync(criteria)).TotalCount;
            }

            return result;
        }

        public virtual async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            IList<IndexDocumentChange> result;

            if (startDate == null && endDate == null)
            {
                result = null;
            }
            else
            {
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


                var priceIds = operations.Select(c => c.ObjectId).ToArray();
                var priceIdsAndProductIds = await GetProductIdsAsync(priceIds);

                result = operations
                    .Where(o => priceIdsAndProductIds.ContainsKey(o.ObjectId))
                    .Select(o => new IndexDocumentChange
                    {
                        DocumentId = priceIdsAndProductIds[o.ObjectId],
                        ChangeDate = o.ModifiedDate ?? o.CreatedDate,
                        ChangeType = IndexDocumentChangeType.Modified,
                    })
                    .ToArray();
            }

            return result;
        }


        protected virtual async Task<IDictionary<string, string>> GetProductIdsAsync(string[] priceIds)
        {
            // TODO: How to get product for deleted price?
            var prices = await _pricingService.GetPricesByIdAsync(priceIds);
            var result = prices.ToDictionary(p => p.Id, p => p.ProductId);
            return result;
        }
    }
}
