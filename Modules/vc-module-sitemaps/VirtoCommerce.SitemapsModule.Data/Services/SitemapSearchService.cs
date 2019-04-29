using System;
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

        public SitemapSearchService(Func<ISitemapRepository> repositoryFactory, ISitemapService sitemapService, IPlatformMemoryCache platformMemoryCache)
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
                var query = repository.Sitemaps;

                if (!string.IsNullOrEmpty(criteria.StoreId))
                {
                    query = query.Where(s => s.StoreId == criteria.StoreId);
                }

                if (!string.IsNullOrEmpty(criteria.Location))
                {
                    query = query.Where(s => s.Filename == criteria.Location);
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[]
                    {
                             new SortInfo
                            {
                                SortColumn = ReflectionUtility.GetPropertyName<SitemapEntity>(x => x.CreatedDate),
                                SortDirection = SortDirection.Descending
                            }
                        };
                }

                query = query.OrderBySortInfos(sortInfos);

                result.TotalCount = await query.CountAsync();

                if (criteria.Take > 0)
                {
                    var sitemapIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                    result.Results = (await _sitemapService.GetByIdsAsync(sitemapIds, criteria.ResponseGroup)).AsQueryable().OrderBySortInfos(sortInfos).ToArray();
                }
            }
            return result;
        }

    }
}
