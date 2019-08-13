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
    public class ContentPublicationsSearchService : IContentPublicationsSearchService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IDynamicContentService _dynamicContentService;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public ContentPublicationsSearchService(Func<IMarketingRepository> repositoryFactory, IDynamicContentService dynamicContentService,
            IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _dynamicContentService = dynamicContentService;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<DynamicContentPublicationSearchResult> SearchContentPublicationsAsync(DynamicContentPublicationSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchContentPublicationsAsync), criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(DynamicContentPublicationCacheRegion.CreateChangeToken());
                var retVal = AbstractTypeFactory<DynamicContentPublicationSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    var sortInfos = BuildSortExpression(criteria);
                    var query = BuildQuery(criteria, repository);

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                             .Select(x => x.Id)
                                             .Skip(criteria.Skip).Take(criteria.Take)
                                             .ToArrayAsync();

                        retVal.Results = (await _dynamicContentService.GetPublicationsByIdsAsync(ids))
                            .OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                    }
                }
                return retVal;
            });
        }

        protected virtual IList<SortInfo> BuildSortExpression(DynamicContentPublicationSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(DynamicContentPublication.Priority),
                        SortDirection = SortDirection.Ascending
                    }
                };
            }

            return sortInfos;
        }

        protected virtual IQueryable<DynamicContentPublishingGroupEntity> BuildQuery(DynamicContentPublicationSearchCriteria criteria, IMarketingRepository repository)
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
            return query;
        }
    }
}
