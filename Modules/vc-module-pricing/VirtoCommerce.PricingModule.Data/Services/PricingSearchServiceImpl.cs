using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Search;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.PricingModule.Data.Caching;
using VirtoCommerce.PricingModule.Data.Repositories;

namespace VirtoCommerce.PricingModule.Data.Services
{
    public class PricingSearchServiceImpl : IPricingSearchService
    {
        private readonly Func<IPricingRepository> _repositoryFactory;
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly IPricingService _pricingService;
        private readonly Dictionary<string, string> _pricesSortingAliases = new Dictionary<string, string>();
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public PricingSearchServiceImpl(Func<IPricingRepository> repositoryFactory, IPricingService pricingService,
            IPlatformMemoryCache platformMemoryCache, ICatalogSearchService catalogSearchService)
        {
            _repositoryFactory = repositoryFactory;
            _pricesSortingAliases["prices"] = ReflectionUtility.GetPropertyName<Price>(x => x.List);
            _pricingService = pricingService;
            _platformMemoryCache = platformMemoryCache;
            _catalogSearchService = catalogSearchService;
        }


        #region IPricingSearchService Members

        public virtual Task<GenericSearchResult<Price>> SearchPricesAsync(PricesSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchPricesAsync), criteria.GetCacheKey());
            return _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
                {
                    cacheEntry.AddExpirationToken(PricesCacheRegion.CreateChangeToken());
                    cacheEntry.AddExpirationToken(PricingSearchCacheRegion.CreateChangeToken());

                    var retVal = new GenericSearchResult<Price>();
                    ICollection<CatalogProduct> products = new List<CatalogProduct>();
                    using (var repository = _repositoryFactory())
                    {
                        var query = repository.Prices;

                        if (!criteria.PriceListIds.IsNullOrEmpty())
                        {
                            query = query.Where(x => criteria.PriceListIds.Contains(x.PricelistId));
                        }

                        if (!criteria.ProductIds.IsNullOrEmpty())
                        {
                            query = query.Where(x => criteria.ProductIds.Contains(x.ProductId));
                        }

                        if (!string.IsNullOrEmpty(criteria.Keyword))
                        {
                            var catalogSearchResult = await _catalogSearchService.SearchAsync(new CatalogListEntrySearchCriteria { Keyword = criteria.Keyword, Skip = criteria.Skip, Take = criteria.Take, Sort = criteria.Sort.Replace("product.", string.Empty), ResponseGroup = SearchResponseGroup.WithProducts });
                            var productIds = catalogSearchResult.Products.Select(x => x.Id).ToArray();
                            query = query.Where(x => productIds.Contains(x.ProductId));
                            //preserve resulting products for future assignment to prices
                            products = catalogSearchResult.Products;
                        }

                        if (criteria.ModifiedSince.HasValue)
                        {
                            query = query.Where(x => x.ModifiedDate >= criteria.ModifiedSince);
                        }

                        var sortInfos = criteria.SortInfos.ToArray();
                        if (sortInfos.IsNullOrEmpty())
                        {
                            sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Price>(x => x.List) } };
                        }
                        //Try to replace sorting columns names
                        TryTransformSortingInfoColumnNames(_pricesSortingAliases, sortInfos);


                        query = query.OrderBySortInfos(sortInfos);

                        // TODO: add checks for criteria.Take being greater than 0
                        if (criteria.GroupByProducts)
                        {
                            var groupedQuery = query.GroupBy(x => x.ProductId).OrderBy(x => 1);
                            retVal.TotalCount = await groupedQuery.CountAsync();
                            query = groupedQuery.Skip(criteria.Skip).Take(criteria.Take).SelectMany(x => x);
                        }
                        else
                        {
                            retVal.TotalCount = await query.CountAsync();
                            query = query.Skip(criteria.Skip).Take(criteria.Take);
                        }

                        if (criteria.Take > 0)
                        {
                            var pricesIds = await query.Select(x => x.Id).ToListAsync();
                            retVal.Results = (await _pricingService.GetPricesByIdAsync(pricesIds.ToArray()))
                                .OrderBy(x => pricesIds.IndexOf(x.Id))
                                .ToList();
                        }
                    }
                    return retVal;
                });
        }

        public virtual async Task<GenericSearchResult<Pricelist>> SearchPricelistsAsync(PricelistSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchPricelistsAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(PricingSearchCacheRegion.CreateChangeToken());

                var retVal = new GenericSearchResult<Pricelist>();
                using (var repository = _repositoryFactory())
                {
                    var query = repository.Pricelists;
                    if (!string.IsNullOrEmpty(criteria.Keyword))
                    {
                        query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Description.Contains(criteria.Keyword));
                    }

                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[]
                        {
                            new SortInfo
                            {
                                SortColumn = ReflectionUtility.GetPropertyName<Pricelist>(x => x.Name)
                            }
                        };
                    }

                    query = query.OrderBySortInfos(sortInfos);

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        query = query.Skip(criteria.Skip).Take(criteria.Take);
                        var pricelistsIds = await query.Select(x => x.Id).ToListAsync();
                        retVal.Results = (await _pricingService.GetPricelistsByIdAsync(pricelistsIds.ToArray()))
                            .OrderBy(x => pricelistsIds.IndexOf(x.Id)).ToList();
                    }
                }

                return retVal;
            });
        }

        public virtual async Task<GenericSearchResult<PricelistAssignment>> SearchPricelistAssignmentsAsync(PricelistAssignmentsSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchPricelistAssignmentsAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(PricingSearchCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(PricingCacheRegion.CreateChangeToken());

                var retVal = new GenericSearchResult<PricelistAssignment>();
                using (var repository = _repositoryFactory())
                {
                    var query = repository.PricelistAssignments;

                    if (!criteria.PriceListIds.IsNullOrEmpty())
                    {
                        query = query.Where(x => criteria.PriceListIds.Contains(x.PricelistId));
                    }

                    if (!string.IsNullOrEmpty(criteria.Keyword))
                    {
                        query.Where(x => x.Name.Contains(criteria.Keyword) || x.Description.Contains(criteria.Keyword));
                    }

                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<PricelistAssignment>(x => x.Priority) } };
                    }

                    query = query.OrderBySortInfos(sortInfos);

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        query = query.Skip(criteria.Skip).Take(criteria.Take);

                        var pricelistAssignmentsIds = await query.Select(x => x.Id).ToListAsync();
                        retVal.Results =
                            (await _pricingService.GetPricelistAssignmentsByIdAsync(pricelistAssignmentsIds.ToArray()))
                            .OrderBy(x => pricelistAssignmentsIds.IndexOf(x.Id))
                            .ToList();
                    }
                }
                return retVal;
            });
        }
        #endregion

        private static void TryTransformSortingInfoColumnNames(IDictionary<string, string> transformationMap, SortInfo[] sortingInfos)
        {
            //Try to replace sorting columns names
            foreach (var sortInfo in sortingInfos)
            {
                if (transformationMap.TryGetValue(sortInfo.SortColumn.ToLowerInvariant(), out var newColumnName))
                {
                    sortInfo.SortColumn = newColumnName;
                }
            }
        }
    }
}

