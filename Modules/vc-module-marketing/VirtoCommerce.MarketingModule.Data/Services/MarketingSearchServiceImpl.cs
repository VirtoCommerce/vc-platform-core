using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent.Search;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Caching;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class MarketingSearchServiceImpl : IDynamicContentSearchService, IPromotionSearchService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IDynamicContentService _dynamicContentService;
        private readonly IPromotionService _promotionService;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public MarketingSearchServiceImpl(Func<IMarketingRepository> repositoryFactory, IDynamicContentService dynamicContentService, IPromotionService promotionService
            , IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _dynamicContentService = dynamicContentService;
            _promotionService = promotionService;
            _platformMemoryCache = platformMemoryCache;
        }

        #region IPromotionSearchService Members
        public virtual async Task<PromotionSearchResult> SearchPromotionsAsync(PromotionSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchPromotionsAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PromotionCacheRegion.CreateChangeToken());
                var retVal = AbstractTypeFactory<PromotionSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    var query = GetPromotionsQuery(repository, criteria);

                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Promotion>(x => x.Priority), SortDirection = SortDirection.Descending } };
                    }
                    query = query.OrderBySortInfos(sortInfos);

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var ids = await query.Select(x => x.Id)
                            .Skip(criteria.Skip)
                            .Take(criteria.Take).ToArrayAsync();
                        var promotions = await _promotionService.GetPromotionsByIdsAsync(ids);
                        retVal.Results = promotions.OrderBy(p => ids.ToList().IndexOf(p.Id)).ToList();
                    }
                }
                return retVal;
            });
        }

        protected virtual IQueryable<Model.PromotionEntity> GetPromotionsQuery(IMarketingRepository repository, PromotionSearchCriteria criteria)
        {
            var query = repository.Promotions;

            if (!string.IsNullOrEmpty(criteria.Store))
            {
                query = query.Where(x => !x.Stores.Any() || x.Stores.Any(s => s.StoreId == criteria.Store));
            }

            if (!criteria.StoreIds.IsNullOrEmpty())
            {
                query = query.Where(x => !x.Stores.Any() || x.Stores.Any(s => criteria.StoreIds.Contains(s.StoreId)));
            }

            if (criteria.OnlyActive)
            {
                var now = DateTime.UtcNow;
                query = query.Where(x => x.IsActive && (x.StartDate == null || now >= x.StartDate) && (x.EndDate == null || x.EndDate >= now));
            }
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Description.Contains(criteria.Keyword));
            }

            return query;
        }

        #endregion

        #region IDynamicContentSearchService Members
        public async Task<DynamicContentItemSearchResult> SearchContentItemsAsync(DynamicContentItemSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchContentItemsAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(DynamicContentItemCacheRegion.CreateChangeToken());
                var retVal = AbstractTypeFactory<DynamicContentItemSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    var query = repository.Items;
                    if (!string.IsNullOrEmpty(criteria.FolderId))
                    {
                        query = query.Where(x => x.FolderId == criteria.FolderId);
                    }
                    if (!string.IsNullOrEmpty(criteria.Keyword))
                    {
                        query = query.Where(q => q.Name.Contains(criteria.Keyword));
                    }

                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<DynamicContentItem>(x => x.Name), SortDirection = SortDirection.Ascending } };
                    }
                    query = query.OrderBySortInfos(sortInfos);

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var ids = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        retVal.Results = await _dynamicContentService.GetContentItemsByIdsAsync(ids);
                    }

                }
                return retVal;
            });
        }

        public async Task<DynamicContentPlaceSearchResult> SearchContentPlacesAsync(DynamicContentPlaceSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchContentPlacesAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(DynamicContentPlaceCacheRegion.CreateChangeToken());
                var retVal = AbstractTypeFactory<DynamicContentPlaceSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    var query = repository.Places;
                    if (!string.IsNullOrEmpty(criteria.FolderId))
                    {
                        query = query.Where(x => x.FolderId == criteria.FolderId);
                    }
                    if (!string.IsNullOrEmpty(criteria.Keyword))
                    {
                        query = query.Where(q => q.Name.Contains(criteria.Keyword));
                    }
                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<DynamicContentPlace>(x => x.Name), SortDirection = SortDirection.Ascending } };
                    }
                    query = query.OrderBySortInfos(sortInfos);
                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var ids = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        retVal.Results = await _dynamicContentService.GetPlacesByIdsAsync(ids);
                    }
                }
                return retVal;
            });
        }

        public async Task<DynamicContentPublicationSearchResult> SearchContentPublicationsAsync(DynamicContentPublicationSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchContentPublicationsAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(DynamicContentPublicationCacheRegion.CreateChangeToken());
                var retVal = AbstractTypeFactory<DynamicContentPublicationSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    var query = repository.PublishingGroups;
                    if (!string.IsNullOrEmpty(criteria.Store))
                    {
                        query = query.Where(x => x.StoreId == criteria.Store);
                    }
                    if (criteria.OnlyActive)
                    {
                        query = query.Where(x => x.IsActive == true);
                    }
                    if (criteria.StartDate != null)
                    {
                        query = query.Where(x => x.StartDate == null || criteria.StartDate >= x.StartDate);
                    }
                    if (criteria.EndDate != null)
                    {
                        query = query.Where(x => x.EndDate == null || x.EndDate >= criteria.EndDate);
                    }
                    if (!string.IsNullOrEmpty(criteria.PlaceName))
                    {
                        query = query.Where(x => x.ContentPlaces.Any(y => y.ContentPlace.Name == criteria.PlaceName));
                    }
                    if (!string.IsNullOrEmpty(criteria.Keyword))
                    {
                        query = query.Where(q => q.Name.Contains(criteria.Keyword));
                    }
                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<DynamicContentPublication>(x => x.Priority), SortDirection = SortDirection.Ascending } };
                    }
                    query = query.OrderBySortInfos(sortInfos);

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var ids = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        retVal.Results = await _dynamicContentService.GetPublicationsByIdsAsync(ids);
                    }
                }
                return retVal;
            });
        }

        public async Task<DynamicContentFolderSearchResult> SearchFoldersAsync(DynamicContentFolderSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchFoldersAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(DynamicContentFolderCacheRegion.CreateChangeToken());
                var retVal = AbstractTypeFactory<DynamicContentFolderSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    var query = repository.Folders.Where(x => x.ParentFolderId == criteria.FolderId);
                    if (!string.IsNullOrEmpty(criteria.Keyword))
                    {
                        query = query.Where(q => q.Name.Contains(criteria.Keyword));
                    }
                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<DynamicContentFolder>(x => x.Name), SortDirection = SortDirection.Ascending } };
                    }

                    query = query.OrderBySortInfos(sortInfos);
                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var folderIds = await query.Select(x => x.Id).ToArrayAsync();
                        retVal.Results = await _dynamicContentService.GetFoldersByIdsAsync(folderIds);
                    }
                }
                return retVal;
            });
        }
        #endregion

    }
}
