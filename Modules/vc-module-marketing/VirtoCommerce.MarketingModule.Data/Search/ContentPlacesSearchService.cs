using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent.Search;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Caching;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Search
{
    public class ContentPlacesSearchService: IContentPlacesSearchService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IDynamicContentService _dynamicContentService;

        public ContentPlacesSearchService(Func<IMarketingRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IDynamicContentService dynamicContentService)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _dynamicContentService = dynamicContentService;
        }

        public async Task<DynamicContentPlaceSearchResult> SearchContentPlacesAsync(DynamicContentPlaceSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchContentPlacesAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(DynamicContentPlaceCacheRegion.CreateChangeToken());
                var result = AbstractTypeFactory<DynamicContentPlaceSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    var sortInfos = BuildSearchExpression(criteria);
                    var query = BuildQuery(criteria, repository);

                    result.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x=>x.Id)
                                             .Select(x => x.Id).Skip(criteria.Skip)
                                             .Take(criteria.Take)
                                             .ToArrayAsync();

                        result.Results = (await _dynamicContentService.GetPlacesByIdsAsync(ids))
                            .OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                    }
                }
                return result;
            });
        }

        protected virtual IList<SortInfo> BuildSearchExpression(DynamicContentPlaceSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(DynamicContentPlace.Name),
                        SortDirection = SortDirection.Ascending
                    }
                };
            }

            return sortInfos;
        }

        protected virtual IQueryable<DynamicContentPlaceEntity> BuildQuery(DynamicContentPlaceSearchCriteria criteria, IMarketingRepository repository)
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
            return query;
        }
    }
}
