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
    public class FolderSearchService: IFolderSearchService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IDynamicContentService _dynamicContentService;

        public FolderSearchService(Func<IMarketingRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache,
            IDynamicContentService dynamicContentService)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _dynamicContentService = dynamicContentService;
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
                    var sortInfos = GetSortInfos(criteria);
                    var query = GetQuery(criteria, repository, sortInfos);

                    retVal.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0)
                    {
                        var ids = await query.Select(x => x.Id).ToArrayAsync();
                        retVal.Results = (await _dynamicContentService.GetFoldersByIdsAsync(ids))
                            .OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                    }
                }

                return retVal;
            });
        }

        protected virtual IList<SortInfo> GetSortInfos(DynamicContentFolderSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = ReflectionUtility.GetPropertyName<DynamicContentFolder>(x => x.Name),
                        SortDirection = SortDirection.Ascending
                    }
                };
            }

            return sortInfos;
        }

        protected virtual IQueryable<DynamicContentFolderEntity> GetQuery(DynamicContentFolderSearchCriteria criteria,
            IMarketingRepository repository, IList<SortInfo> sortInfos)
        {
            var query = repository.Folders.Where(x => x.ParentFolderId == criteria.FolderId);
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(q => q.Name.Contains(criteria.Keyword));
            }

            query = query.OrderBySortInfos(sortInfos);
            return query;
        }
    }
}
