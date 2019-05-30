using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.TaxModule.Data.Caching;
using VirtoCommerce.TaxModule.Data.Model;
using VirtoCommerce.TaxModule.Data.Repositories;

namespace VirtoCommerce.TaxModule.Data.Services
{
    public class TaxProviderSearchService : ITaxProviderSearchService
    {
        private readonly Func<ITaxRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _memCache;
        private readonly ITaxProviderService _taxProviderService;
        private readonly ISettingsManager _settingManager;

        public TaxProviderSearchService(Func<ITaxRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, ITaxProviderService taxProviderService, ISettingsManager settingManager)
        {
            _repositoryFactory = repositoryFactory;
            _memCache = platformMemoryCache;
            _taxProviderService = taxProviderService;
            _settingManager = settingManager;
        }

        public virtual async Task<TaxProviderSearchResult> SearchTaxProvidersAsync(TaxProviderSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchTaxProvidersAsync", criteria.GetCacheKey());
            return await _memCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(TaxCacheRegion.CreateChangeToken());
                var result = AbstractTypeFactory<TaxProviderSearchResult>.TryCreateInstance();

                var sortInfos = GetSortInfos(criteria);

                var tmpSkip = 0;
                var tmpTake = 0;

                using (var repository = _repositoryFactory())
                {
                    var query = GetQuery(repository, criteria, sortInfos);

                    result.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var providerIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        result.Results = (await _taxProviderService.GetByIdsAsync(providerIds, criteria.ResponseGroup)).AsQueryable().OrderBySortInfos(sortInfos).ToList();
                    }
                }
                tmpSkip = Math.Min(result.TotalCount, criteria.Skip);
                tmpTake = Math.Min(criteria.Take, Math.Max(0, result.TotalCount - criteria.Skip));

                criteria.Skip = criteria.Skip - tmpSkip;
                criteria.Take = criteria.Take - tmpTake;
                if (criteria.Take > 0)
                {
                    var transientProvidersQuery = AbstractTypeFactory<TaxProvider>.AllTypeInfos.Select(x => AbstractTypeFactory<TaxProvider>.TryCreateInstance(x.Type.Name))
                                                                                  .OfType<TaxProvider>().AsQueryable();
                    if (!string.IsNullOrEmpty(criteria.Keyword))
                    {
                        transientProvidersQuery = transientProvidersQuery.Where(x => x.Code.Contains(criteria.Keyword));
                    }
                    var allPersistentProvidersTypes = result.Results.Select(x => x.GetType()).Distinct();
                    transientProvidersQuery = transientProvidersQuery.Where(x => !allPersistentProvidersTypes.Contains(x.GetType()));

                    result.TotalCount += transientProvidersQuery.Count();
                    var transientProviders = transientProvidersQuery.Skip(criteria.Skip).Take(criteria.Take).ToList();

                    foreach (var transientProvider in transientProviders)
                    {
                        await _settingManager.DeepLoadSettingsAsync(transientProvider);
                    }

                    result.Results = result.Results.Concat(transientProviders).AsQueryable().OrderBySortInfos(sortInfos).ToList();
                }
                return result;
            });
        }

        protected virtual IQueryable<StoreTaxProviderEntity> GetQuery(ITaxRepository repository, TaxProviderSearchCriteria criteria, IEnumerable<SortInfo> sortInfos)
        {
            var query = repository.StoreTaxProviders;
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Code.Contains(criteria.Keyword) || x.Id.Contains(criteria.Keyword));
            }
            if (!criteria.StoreId.IsNullOrEmpty())
            {
                query = query.Where(x => x.StoreId == criteria.StoreId);
            }
            query = query.OrderBySortInfos(sortInfos);
            return query;
        }

        protected virtual IList<SortInfo> GetSortInfos(TaxProviderSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = "Name"
                    }
                };
            }

            return sortInfos;
        }
    }
}
