using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.ShippingModule.Data.Caching;
using VirtoCommerce.ShippingModule.Data.Model;
using VirtoCommerce.ShippingModule.Data.Repositories;

namespace VirtoCommerce.ShippingModule.Data.Services
{
    public class ShippingMethodsSearchService : IShippingMethodsSearchService
    {
        private readonly Func<IShippingRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _memCache;
        private readonly IShippingMethodsService _shippingMethodsService;
        private readonly ISettingsManager _settingsManager;

        public ShippingMethodsSearchService(
            Func<IShippingRepository> repositoryFactory,
            IPlatformMemoryCache memCache,
            IShippingMethodsService shippingMethodsService,
            ISettingsManager settingsManager)
        {
            _repositoryFactory = repositoryFactory;
            _memCache = memCache;
            _shippingMethodsService = shippingMethodsService;
            _settingsManager = settingsManager;
        }

        public async Task<ShippingMethodsSearchResult> SearchShippingMethodsAsync(ShippingMethodsSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchShippingMethodsAsync), criteria.GetCacheKey());
            return await _memCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(ShippingCacheRegion.CreateChangeToken());
                var result = AbstractTypeFactory<ShippingMethodsSearchResult>.TryCreateInstance();

                var tmpSkip = 0;
                var tmpTake = 0;

                var sortInfos = BuildSortExpression(criteria);

                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();
                    var query = BuildQuery(repository, criteria);

                    result.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var shippingMethodsIds = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                                           .Select(x => x.Id)
                                                           .Skip(criteria.Skip).Take(criteria.Take)
                                                           .ToArrayAsync();

                        var unorderedResults = await _shippingMethodsService.GetByIdsAsync(shippingMethodsIds, criteria.ResponseGroup);
                        result.Results = unorderedResults.OrderBy(x => Array.IndexOf(shippingMethodsIds, x.Id)).ToArray();
                    }
                }

                //Need to concatenate persistent methods with registered types and still not persisted
                tmpSkip = Math.Min(result.TotalCount, criteria.Skip);
                tmpTake = Math.Min(criteria.Take, Math.Max(0, result.TotalCount - criteria.Skip));
                criteria.Skip -= tmpSkip;
                criteria.Take -= tmpTake;
                if (criteria.Take > 0 && !criteria.WithoutTransient)
                {
                    var transientMethodsQuery = AbstractTypeFactory<ShippingMethod>.AllTypeInfos.Select(x => AbstractTypeFactory<ShippingMethod>.TryCreateInstance(x.Type.Name))
                                                                                  .OfType<ShippingMethod>().AsQueryable();
                    if (!string.IsNullOrEmpty(criteria.Keyword))
                    {
                        transientMethodsQuery = transientMethodsQuery.Where(x => x.Code.Contains(criteria.Keyword));
                    }
                    var allPersistentTypes = result.Results.Select(x => x.GetType()).Distinct();
                    transientMethodsQuery = transientMethodsQuery.Where(x => !allPersistentTypes.Contains(x.GetType()));

                    result.TotalCount += transientMethodsQuery.Count();
                    var transientProviders = transientMethodsQuery.Skip(criteria.Skip).Take(criteria.Take).ToList();

                    foreach (var transientProvider in transientProviders)
                    {
                        await _settingsManager.DeepLoadSettingsAsync(transientProvider);
                    }

                    result.Results = result.Results.Concat(transientProviders).AsQueryable().OrderBySortInfos(sortInfos).ToList();
                }

                return result;
            });
        }

        protected virtual IQueryable<StoreShippingMethodEntity> BuildQuery(IShippingRepository repository, ShippingMethodsSearchCriteria criteria)
        {
            var query = repository.StoreShippingMethods;

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Code.Contains(criteria.Keyword) || x.Id.Contains(criteria.Keyword));
            }

            if (!criteria.StoreId.IsNullOrEmpty())
            {
                query = query.Where(x => x.StoreId == criteria.StoreId);
            }

            if (!criteria.Codes.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.Codes.Contains(x.Code));
            }

            if (!criteria.TaxType.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.TaxType.Contains(x.TaxType));
            }

            if (criteria.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == criteria.IsActive.Value);
            }
            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(ShippingMethodsSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo{ SortColumn = nameof(StoreShippingMethodEntity.Code) }
                };
            }

            return sortInfos;
        }
    }
}
