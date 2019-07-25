using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models.Search;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Models;
using VirtoCommerce.SitemapsModule.Data.Repositories;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public class SitemapSearchService : ISitemapSearchService
    {
        private readonly Func<ISitemapRepository> _repositoryFactory;
        private readonly ISitemapService _sitemapService;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public SitemapSearchService(
            Func<ISitemapRepository> repositoryFactory
            , ISitemapService sitemapService
            , IPlatformMemoryCache platformMemoryCache
            )
        {
            _repositoryFactory = repositoryFactory;
            _sitemapService = sitemapService;
            _platformMemoryCache = platformMemoryCache;
        }

        public virtual async Task<SitemapSearchResult> SearchAsync(SitemapSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var result = AbstractTypeFactory<SitemapSearchResult>.TryCreateInstance();
            using (var repository = _repositoryFactory())
            {
                var sortInfos = BuildSortExpression(criteria);
                var query = BuildQuery(repository, criteria);
              
                result.TotalCount = await query.CountAsync();

                if (criteria.Take > 0)
                {
                    var sitemapIds = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                                .Select(x => x.Id)
                                                .Skip(criteria.Skip).Take(criteria.Take)
                                                .ToArrayAsync();

                    var unorderedResults = await _sitemapService.GetByIdsAsync(sitemapIds, criteria.ResponseGroup);
                    result.Results = unorderedResults.OrderBy(x => Array.IndexOf(sitemapIds, x.Id)).ToArray();
                }
            }
            return result;
        }

        protected virtual IQueryable<SitemapEntity> BuildQuery(ISitemapRepository repository, SitemapSearchCriteria criteria)
        {
            var query = repository.Sitemaps;

            if (!string.IsNullOrEmpty(criteria.StoreId))
            {
                query = query.Where(s => s.StoreId == criteria.StoreId);
            }
            if (!string.IsNullOrEmpty(criteria.Location))
            {
                query = query.Where(s => s.Filename == criteria.Location);
            }
            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(SitemapSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(SitemapEntity.CreatedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }
            return sortInfos;
        }
    }
}
