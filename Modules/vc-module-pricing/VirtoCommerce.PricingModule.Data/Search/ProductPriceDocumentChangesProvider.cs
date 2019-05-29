using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.PricingModule.Core;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.PricingModule.Data.Repositories;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.Search
{
    public class ProductPriceDocumentChangesProvider : IIndexDocumentChangesProvider, IPricingChangesService
    {
        public const string ChangeLogObjectType = nameof(Price);
        private static readonly TimeSpan CalendarChangesInterval = TimeSpan.FromDays(1);

        private readonly IChangeLogSearchService _changeLogSearchService;
        private readonly IPricingService _pricingService;
        private readonly ISettingsManager _settingsManager;
        private readonly Func<IPricingRepository> _repositoryFactory;

        public ProductPriceDocumentChangesProvider(IChangeLogSearchService changeLogSearchService, IPricingService pricingService, ISettingsManager settingsManager, Func<IPricingRepository> repositoryFactory)
        {
            _changeLogSearchService = changeLogSearchService;
            _pricingService = pricingService;
            _settingsManager = settingsManager;
            _repositoryFactory = repositoryFactory;
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

        public virtual async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate,
            long skip, long take)
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

                var workSkip = (int)(skip - Math.Min(result.Count, skip));
                var workTake = (int)(take - Math.Min(take, Math.Max(0, result.Count - skip)));

                //Re-index calendar prices only once per defined time interval
                var lastIndexDate = _settingsManager.GetValue(ModuleConstants.Settings.General.IndexationDatePricingCalendar.Name,
                        (DateTime?)null) ?? DateTime.MinValue;
                if (DateTime.UtcNow - lastIndexDate > CalendarChangesInterval && startDate != null && endDate != null)
                {
                    var calendarChanges =
                        await GetCalendarChangesAsync(startDate.Value, endDate.Value, workSkip, workTake);

                    if (workTake > 0)
                    {
                        _settingsManager.SetValue(ModuleConstants.Settings.General.IndexationDatePricingCalendar.Name, DateTime.UtcNow);
                        result.AddRange(calendarChanges);
                    }
                }
            }
            return result;
        }

        #region IPricingDocumentChangesProvider Members
        public virtual async Task<IList<IndexDocumentChange>> GetCalendarChangesAsync(DateTime? startDate, DateTime? endDate, int skip = 0, int take = 0)
        {
            IList<IndexDocumentChange> result = null;

            using (var repository = _repositoryFactory())
            {
                //Return calendar changes only for prices that have at least one specific date range.
                var priceLists = await repository.PricelistAssignments
                    .Where(x => x.StartDate != null || x.EndDate != null)
                    .Where(x => (x.StartDate == null || x.StartDate <= endDate) && (x.EndDate == null || x.EndDate > startDate))
                    .Select(x => x.PricelistId)
                    .Distinct()
                    .ToArrayAsync();

                if (priceLists.Any())
                {
                    result = repository.Prices.Where(x => priceLists.Contains(x.PricelistId))
                        .Select(x => x.Id)
                        .OrderBy(x => x)
                        .Skip(skip)
                        .Take(take)
                        .Select(x => new IndexDocumentChange { DocumentId = x, ChangeType = IndexDocumentChangeType.Modified })
                        .ToList();
                }
            }
            return result;
        }
        #endregion


        protected virtual async Task<IDictionary<string, string>> GetProductIdsAsync(string[] priceIds)
        {
            // TODO: How to get product for deleted price?
            var prices = await _pricingService.GetPricesByIdAsync(priceIds);
            var result = prices.ToDictionary(p => p.Id, p => p.ProductId);
            return result;
        }
    }
}
